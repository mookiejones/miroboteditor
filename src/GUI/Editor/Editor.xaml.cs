// 4/11/2013 Registering Syntax highlighting was moved to TextEditorOptions. It only needs to be loaded one time.

// 4/23/2013 Changed the save routine. Something with the internal save function was saving weird characters in it. The should resolve the situation by only saving the Test 
// String on my own.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using miRobotEditor.Commands;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using miRobotEditor.Classes;
using System.Collections.ObjectModel;
using miRobotEditor.Controls;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.ViewModel;
using ICSharpCode.AvalonEdit.Editing;
namespace miRobotEditor.GUI
{
    
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    [Localizable(false)]
// ReSharper disable RedundantExtendsListEntry
    public partial class Editor : TextEditor, INotifyPropertyChanged
// ReSharper restore RedundantExtendsListEntry
    {
        #region Constructor
    
        public Editor()
        {
            InitializeComponent();
            _iconBarMargin = new IconBarMargin(_iconBarManager = new IconBarManager());
            InitializeMyControl();
            MouseHoverStopped += delegate { _toolTip.IsOpen = false; };
        }

        #endregion

        #region ViewModel Properties

        public int Line { get { return TextArea.Caret.Column; } }

        /// <summary>
        /// Used for displaying position in status bar
        /// </summary>
        public int Column { get { return TextArea.Caret.Column; } }
        /// <summary>
        /// Used for displaying position in status bar
        /// </summary>
        public int Offset { get { return TextArea.Caret.Offset; } }
 
        public new string Text { get { return base.Text; } set { base.Text = value; } }

        public string Title { get { return Path.GetFileName(Filename); }  }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Editor), new PropertyMetadata((obj, args) => { var target = (Editor)obj; target.Text = (string)args.NewValue;}));


        private EDITORTYPE _editortype;
    	public EDITORTYPE EditorType {get{return _editortype;}set{_editortype =value; OnPropertyChanged("EditorType");}}

        private IVariable _selectedVariable;
        public IVariable SelectedVariable { get { return _selectedVariable; } set { _selectedVariable = value; SelectText(_selectedVariable); OnPropertyChanged("SelectedVariable"); } }

        private String _filename = string.Empty;
        public string Filename { get { return _filename; } set { _filename = value; OnPropertyChanged("Filename"); OnPropertyChanged("Title"); } }

        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; OnPropertyChanged("FileLanguage"); } }


        readonly ObservableCollection<IVariable> _variables = new ObservableCollection<IVariable>();
        readonly ReadOnlyObservableCollection<IVariable> _readonlyVariables = null;
        public ReadOnlyObservableCollection<IVariable> Variables { get { return _readonlyVariables ?? new ReadOnlyObservableCollection<IVariable>(_variables); } }



        private string _fileSave = string.Empty;
        public string FileSave { get { return _fileSave; } set { _fileSave = value; OnPropertyChanged("FileSave"); } }

        #endregion

        #region Commands

        private  RelayCommand _undoCommand;
        public  ICommand UndoCommand
        {
            get
            {
                return _undoCommand ?? (_undoCommand = new RelayCommand(param => Undo(), param => (CanUndo)));
            }
        }

        private  RelayCommand _redoCommand;
        public  ICommand RedoCommand
        {
            get
            {
                return _redoCommand ?? (_redoCommand = new RelayCommand(param => Redo(), param => (CanRedo)));
            }
        }

        private RelayCommand _saveCommand;
        public  ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(p =>Save(), p => CanSave())); }
        }

        private  RelayCommand _saveAsCommand;
        public  ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(p => SaveAs(), p => CanSave())); }
        }

        private static RelayCommand _replaceCommand;
        public  ICommand ReplaceCommand
        {
            get { return _replaceCommand ?? (_replaceCommand = new RelayCommand(p => Replace(), p => true)); }
        }

        private RelayCommand _variableDoubleClickCommand;
        public ICommand VariableDoubleClickCommand
        {
            get { return _variableDoubleClickCommand ?? (_variableDoubleClickCommand = new RelayCommand(p => SelectText((IVariable)((ListViewItem)p).Content), p => p != null)); }
        }

        private RelayCommand _gotoCommand;
        public ICommand GotoCommand
        {
            get { return _gotoCommand ?? (_gotoCommand = new RelayCommand(p => Goto(), p =>(!String.IsNullOrEmpty(Text)))); }
        }
        private RelayCommand _openAllFoldsCommand;
        public ICommand OpenAllFoldsCommand
        {
            get { return _openAllFoldsCommand ?? (_openAllFoldsCommand = new RelayCommand(p => ChangeFoldStatus(false), p => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _toggleCommentCommand;
        public ICommand ToggleCommentCommand
        {
            get { return _toggleCommentCommand ?? (_toggleCommentCommand = new RelayCommand(p => ToggleComment(), p => (!String.IsNullOrEmpty(FileLanguage.CommentChar)))); }
        }


        private RelayCommand _toggleFoldsCommand;
        public ICommand ToggleFoldsCommand
        {
            get { return _toggleFoldsCommand ?? (_toggleFoldsCommand = new RelayCommand(p => ToggleFolds(), p => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _toggleAllFoldsCommand;
        public ICommand ToggleAllFoldsCommand
        {
            get { return _toggleAllFoldsCommand ?? (_toggleAllFoldsCommand = new RelayCommand(p => ToggleAllFolds(), p => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _closeAllFoldsCommand;
        public ICommand CloseAllFoldsCommand
        {
            get { return _closeAllFoldsCommand ?? (_closeAllFoldsCommand = new RelayCommand(p => ChangeFoldStatus(true), p => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }

        private RelayCommand _findCommand;
        public ICommand FindCommand
        {
            get { return _findCommand ?? (_findCommand = new RelayCommand(p => ChangeFoldStatus(true), p => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _reloadCommand;
        public ICommand ReloadCommand
        {
            get { return _reloadCommand ?? (_reloadCommand = new RelayCommand(p => Reload(), p => true)); }
        }

        private RelayCommand _showDefinitionsCommand;
        public ICommand ShowDefinitionsCommand
        {
            get { return _showDefinitionsCommand ?? (_showDefinitionsCommand = new RelayCommand(p => ShowDefinitions(), p => (_foldingManager != null ))); }
        }

        private RelayCommand _cutCommand;
        public ICommand CutCommand
        {
            get { return _cutCommand ?? (_cutCommand = new RelayCommand(p => Cut(), p => (Text.Length>0))); }
        }
        private RelayCommand _copyCommand;
        public ICommand CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new RelayCommand(p => Cut(), p => (Text.Length > 0))); }
        }
        private RelayCommand _pasteCommand;
        public ICommand PasteCommand
        {
            get { return _pasteCommand ?? (_pasteCommand = new RelayCommand(p => Paste(), p => (Clipboard.ContainsText()))); }
        }

        private RelayCommand _functionWindowClickCommand;
        public ICommand FunctionWindowClickCommand
        {
            get { return _functionWindowClickCommand ?? (_functionWindowClickCommand = new RelayCommand(param => OpenFunctionItem(param), param => true)); }
        }

        private RelayCommand _changeIndentCommand;
        public ICommand ChangeIndentCommand
        {
            get { return _changeIndentCommand ?? (_changeIndentCommand = new RelayCommand(param => ChangeIndent(param), param => true)); }
        }
        #endregion

        #region Private Members

        private readonly IconBarManager _iconBarManager;
        private readonly IconBarMargin _iconBarMargin;


        /// <summary>
        /// Records last key For Multiple Key presses
        /// </summary>
        private KeyEventArgs _lastKeyUpArgs;

        /// <summary>
        //  For Zooming When Scrolling Text
        /// </summary>
        private const int LogicListFontSizeMax = 50;

        /// <summary>
        ///  For Zooming When Scrolling Text
        /// </summary>
        private const int LogicListFontSizeMin = 10;

        #endregion
       

        private void OpenFunctionItem(object parameter)
        {
            var i = (IVariable)((ListViewItem)parameter).Content;
            SelectText(i);
        }


        void InitializeMyControl()
        {
            TextArea.LeftMargins.Insert(0, _iconBarMargin);
            TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(TextArea));

            AddBindings();
            TextArea.TextEntered += TextEntered;
            TextArea.TextEntering += TextEntering;
            TextArea.Caret.PositionChanged += CaretPositionChanged;
            DataContext = this;
        }
        
        #region CaretPositionChanged - Bracket Highlighting

        private readonly MyBracketSearcher _bracketSearcher = new MyBracketSearcher();
        private BracketHighlightRenderer _bracketRenderer;

        /// <summary>
        /// Highlights matching brackets.
        /// </summary>
// ReSharper disable UnusedParameter.Local
        private void HighlightBrackets(object sender, EventArgs e)
// ReSharper restore UnusedParameter.Local
        {
            /*
             * Special case: ITextEditor.Language guarantees that it never returns null.
             * In this case however it can be null, since this code may be called while the document is loaded.
             * ITextEditor.Language gets set in CodeEditorAdapter.FileNameChanged, which is called after
             * loading of the document has finished.
             * */


            var bracketSearchResult = _bracketSearcher.SearchBracket(Document, TextArea.Caret.Offset);
            _bracketRenderer.SetHighlight(bracketSearchResult);
        }


     
        private void CaretPositionChanged(object sender, EventArgs e)
        {
            var s = sender as Caret;

            UpdateLineTransformers();
            if (s != null)
            {

            OnPropertyChanged("Line");
            OnPropertyChanged("Column");
            OnPropertyChanged("Offset");
            FileSave = !String.IsNullOrEmpty(Filename)? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture): String.Empty;

                
            }


            HighlightBrackets(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public  void UpdateLineTransformers()
        {
            // Clear the Current Renderers
            TextArea.TextView.BackgroundRenderers.Clear();
            var textEditorOptions = Options as EditorOptions;

            if (textEditorOptions != null && textEditorOptions.HighlightCurrentLine)
                TextArea.TextView.BackgroundRenderers.Add(new BackgroundRenderer(Document.GetLineByOffset(CaretOffset)));

            if (_bracketRenderer == null)
                _bracketRenderer = new BracketHighlightRenderer(TextArea.TextView);
            else
                TextArea.TextView.BackgroundRenderers.Add(_bracketRenderer);
        }
        #endregion

        #region Overrides

        protected override void OnKeyUp(KeyEventArgs e)
        {

            if (_lastKeyUpArgs == null)
            {
                _lastKeyUpArgs = e;
                return;
            }

            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Control:
                    switch (e.Key)
                    {
                        case Key.O:
                            if (!(String.IsNullOrEmpty(FileLanguage.CommentChar)))
                                ToggleFolds();
                            break;
                    }
                    break;
            }


            // save argument for next event
            _lastKeyUpArgs = e;

            // call base handler
            base.OnKeyUp(e);

        }

        #endregion


        private void AddBookMark(int lineNumber, string imgpath)
        {
            var bitmap = Utilities.LoadBitmap(imgpath);
            var bmi = new BookmarkImage(bitmap);
            _iconBarManager.Bookmarks.Add(new ClassMemberBookmark(lineNumber, bmi));
        }



        //TODO Signal Path for KUKARegex currently displays linear motion
        private void FindMatches(Regex matchstring, string imgPath)
        {
            // Dont Include Empty Values
            if (String.IsNullOrEmpty(matchstring.ToString())) return;

            var m = matchstring.Match(Text.ToLower());
            while (m.Success)
            {
                _variables.Add(new Variable
                                  {   Declaration=m.Groups[0].ToString(),
                                      Offset = m.Index,
                                      Type = m.Groups[1].ToString(),
                                      Name = m.Groups[2].ToString(),
                                      Value=m.Groups[3].ToString(),
                                      Path = Filename,
                                      Icon = Utilities.LoadBitmap(imgPath)
                                  } );
                var d = Document.GetLineByOffset(m.Index);
                AddBookMark(d.LineNumber, imgPath);
                m = m.NextMatch();
            }
        }

        /// <summary>
        /// Find info for bookmark
        /// <remarks>Need to make sure Correct Priority is set. Whatever is set first will overwrite anything after</remarks>
        /// </summary>
        private void FindBookmarkMembers()
        {

            // Return if FileLanguage doesnt exist yet
            if (FileLanguage == null) return;
            _iconBarManager.Bookmarks.Clear();
              _variables.Clear();
            FindMatches(FileLanguage.MethodRegex, Global.ImgMethod);
            FindMatches(FileLanguage.StructRegex, Global.ImgStruct);
            FindMatches(FileLanguage.FieldRegex, Global.ImgField);
            FindMatches(FileLanguage.SignalRegex, Global.ImgSignal);
            FindMatches(FileLanguage.EnumRegex, Global.ImgEnum);
            FindMatches(FileLanguage.XYZRegex, Global.ImgXyz);         

        }
        
        #region Editor.Bindings

        private void AddBindings()
        {
            var inputBindings = TextArea.InputBindings;
            inputBindings.Add(new KeyBinding(ApplicationCommands.Find, Key.F, ModifierKeys.Control));
            inputBindings.Add(new KeyBinding(ApplicationCommands.Replace, Key.R, ModifierKeys.Control));
        }

        protected override bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            switch (managerType.Name)
            {
                case "TextChanged":
                    FindBookmarkMembers();
                    //IsModified = true;
                    UpdateFolds();
                    break;
            }
            return base.ReceiveWeakEvent(managerType, sender, e);
        }

		
        public void ChangeIndent(object param)
        {
        	
        	
        	try{
        		
        	
   		        var increase = Convert.ToBoolean(param);

        		var start = Document.GetLineByOffset(SelectionStart);
                var end = Document.GetLineByOffset(SelectionStart + SelectionLength);
                var positions = 0;
                using (Document.RunUpdate())
                {
                    for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                    {
                        var currentline = GetLine(line.LineNumber);
                        var rgx = new Regex(@"(^[\s]+)");

                        var m = rgx.Match(currentline);
                        if (m.Success)
                            positions = m.Groups[1].Length;

                     
                        if (increase)
                        	  Document.Insert(line.Offset + positions, " ");
                        else{
                        	   positions = positions > 1 ? positions - 1 : positions;                        	  
                        if (positions >= 1)
                            Document.Replace(line.Offset, currentline.Length, currentline.Substring(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageViewModel.AddError("Editor.ChangeIndent",ex);
            }
        }


        /// <summary>
        /// Evaluates each line in selection and Comments/Uncomments "Each Line"
        /// </summary>
        private void ToggleComment()
        {
            //No point in commenting if I dont know the Language
            if (FileLanguage == null) return;

            // Get Comment to insert
            var start = Document.GetLineByOffset(SelectionStart);
            var end = Document.GetLineByOffset(SelectionStart + SelectionLength);

            using (Document.RunUpdate())
            {
                for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                {
                    var currentline = GetLine(line.LineNumber);

                    // Had to put in comment offset for Fanuc 
                    if (FileLanguage.IsLineCommented(currentline))
                        Document.Insert(FileLanguage.CommentOffset(currentline) + line.Offset,
                                        FileLanguage.CommentChar);
                    else
                    {
                        var replacestring = FileLanguage.CommentReplaceString(currentline);
                        Document.Replace(line.Offset, currentline.Length, replacestring);
                    }
                }
            }
        }

 
        public bool CanSave()
        {
            return File.Exists(Filename) ? IsModified : IsModified;
        }

        void Replace()
        {
            // Make sure we update the Editor _instance           
            FindAndReplaceForm.Instance = new FindAndReplaceForm{Left = Mouse.GetPosition(this).X, Top = Mouse.GetPosition(this).Y};
            FindAndReplaceForm.Instance.Show();
        }


        void Goto()
        {
            var vm = new GotoViewModel(this);
            var gtd = new GotoDialog {DataContext = vm};
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            gtd.ShowDialog().GetValueOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed

        }


        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            if (!File.Exists(file.FullName)) return false;
            try
            {
                stream = file.Open(FileMode.Open,FileAccess.Read,FileShare.None);

            }
            catch (IOException ex)
            {
                MessageViewModel.AddError("File is locked!",ex);
                
                return true;
            }
            finally
            {
                if (stream!=null)
                stream.Close();
            }
            return false;
        }


        
        string GetFilename()
        {
        	  var ofd = new Microsoft.Win32.SaveFileDialog {Title = "Save As", Filter = "All _files(*.*)|*.*"};

            if (!String.IsNullOrEmpty(Filename))
            {
                ofd.FileName = Filename;
                ofd.Filter += String.Format("|Current Type (*{0})|*{0}", Path.GetExtension(Filename));
                ofd.FilterIndex = 2;
                ofd.DefaultExt = Path.GetExtension(Filename);
            }

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            ofd.ShowDialog().GetValueOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
            
            return ofd.FileName;
            	
        }

        public void SaveAs()
        {
            Filename = GetFilename();

            var islocked = IsFileLocked(new FileInfo(Filename));

            if (islocked)
                return;

            File.WriteAllText(Filename, Text);

            OnPropertyChanged("Title");

            MessageViewModel.Add(new OutputWindowMessage { Title = "File Saved", Description = Filename, Icon = null });
        }
        
        
        public void Save()
        {
            //_watcher.EnableRaisingEvents = false;
            // _watcher.Text = Text;
            
            if (String.IsNullOrEmpty(Filename))
            	Filename = GetFilename();

            if (IsFileLocked(new FileInfo(Filename))) return;
                
                File.WriteAllText(Filename,Text);
			
            FileSave = File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture);
            IsModified = false;
            //_watcher.EnableRaisingEvents = true;
            // Suspend the calling thread until the file has been deleted. 
        }

        #endregion

        protected override void OnOptionChanged(PropertyChangedEventArgs e)
        {

            base.OnOptionChanged(e);

            Console.WriteLine(e.PropertyName);
            switch (e.PropertyName)
            {
                case "EnableFolding":
                    UpdateFolds();
                    break;
            }
         
        }

        

        public void SetHighlighting()
        {
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(Filename));
        }


        #region Code Completion


        public static readonly DependencyProperty CompletionWindowProperty = DependencyProperty.Register("CompletionWindow", typeof(CompletionWindow), typeof(Editor));
        public CompletionWindow CompletionWindow { get { return (CompletionWindow)GetValue(CompletionWindowProperty); } set { SetValue(CompletionWindowProperty, value); } }

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (FileLanguage == null | FileLanguage is LanguageBase) return;

            var currentword = FindWord();
            // Dont Show Completion window until there are 3 Characters
            if (currentword != null && (String.IsNullOrEmpty(currentword)) | currentword.Length < 3) return;

            CompletionWindow = new CompletionWindow(TextArea);
           // FileLanguage.CompletionList(this, currentword, CompletionWindow.CompletionList.CompletionData);
            var items = GetCompletionItems();
            foreach (var item in items)
            {
                CompletionWindow.CompletionList.CompletionData.Add(item);
            }

            CompletionWindow.Closed += delegate { CompletionWindow = null; };
            CompletionWindow.CloseWhenCaretAtBeginning = true;
            CompletionWindow.CompletionList.SelectItem(currentword);
            if (CompletionWindow.CompletionList.SelectedItem != null)
            CompletionWindow.Show();

            if (IsModified)
                UpdateFolds();
        }


        #region Code Completion
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public IList<ICompletionData> CompletionData { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
        IEnumerable<ICompletionData> GetCompletionItems()
        {
            var items = new List<ICompletionData>();

            items.AddRange(HighlightList());
            items.AddRange(LocalCompletionList());

            return items.ToArray();
        }

        public IList<ICompletionData> HighlightList()
        {
            var items = new List<CodeCompletion>();

            foreach (var item in from rule in SyntaxHighlighting.MainRuleSet.Rules select rule.Regex.ToString() into parseString let start = parseString.IndexOf(">", StringComparison.Ordinal) + 1 let end = parseString.LastIndexOf(")", StringComparison.Ordinal) select parseString.Substring(start, end - start) into parseString1 select parseString1.Split('|') into spl from item in spl.Where(t => !String.IsNullOrEmpty(t)).Select(t => new CodeCompletion(t.Replace("\\b", ""))).Where(item => !items.Contains(item) && char.IsLetter(item.Text, 0)) select item)
            {
                items.Add(item);
            }

            return items.ToArray();
        }

        IEnumerable<ICompletionData> LocalCompletionList()
        {
            return (from v in Variables where (v.Type != "def") && (v.Type != "deffct") select new CodeCompletion(v.Name) {Image = v.Icon}).Cast<ICompletionData>().ToArray();
        }
// ReSharper disable UnusedMember.Local
        IList<ICompletionData> ObjectBrowserCompletionList()
// ReSharper restore UnusedMember.Local
        {
            var items = new List<ICompletionData>();

            return items.ToArray();
        }
       

        #endregion

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length <= 0 || CompletionWindow == null) return;
            if (!char.IsLetterOrDigit(e.Text[0]))
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                CompletionWindow.CompletionList.RequestInsertion(e);
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        #endregion

        #region Search Replace Section

        public void ReplaceAll()
        {
            var r = FindReplaceViewModel.Instance.RegexPattern;
            var m = r.Match(Text);
            while (m.Success)
            {
                Document.GetLineByOffset(m.Index);
                r.Replace(FindReplaceViewModel.Instance.LookFor, FindReplaceViewModel.Instance.ReplaceWith, m.Index);
                m = m.NextMatch();
            }
        }

        public void ReplaceText()
        {
            FindText();
            SelectedText = SelectedText.Replace(SelectedText, FindReplaceViewModel.Instance.ReplaceWith);
        }

        public void FindText()
        {
            var nIndex = Text.IndexOf(FindReplaceViewModel.Instance.LookFor, CaretOffset, StringComparison.Ordinal);
            if (nIndex > -1)
            {

                Document.GetLineByOffset(nIndex);
                JumpTo(new Variable {Offset = nIndex});
                SelectionStart = nIndex;
                SelectionLength = FindReplaceViewModel.Instance.LookFor.Length;
            }
            else
            {
                FindReplaceViewModel.Instance.SearchResult = "No Results Found, Starting Search from Beginning";
                CaretOffset = 0;
            }

        }

        public void JumpTo(IVariable i)
        {
            
            var c = Document.GetLocation(Convert.ToInt32(i.Offset));

            ScrollTo(c.Line, c.Column);
            SelectionStart = Convert.ToInt32(i.Offset);
            SelectionLength = i.Value.Length;
            Focus();
            if (EditorOptions.Instance.EnableAnimations)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)DisplayCaretHighlightAnimation);
        }
        
        
        private void DisplayCaretHighlightAnimation()
        {

            if (TextArea == null)
                return;

            var layer = AdornerLayer.GetAdornerLayer(TextArea.TextView);

            if (layer == null)
                return;

            var adorner = new CaretHighlightAdorner(TextArea);
            layer.Add(adorner);
        }

 
    
        public void SelectText(IVariable var)
        {
        	if (var.Name == null) throw new ArgumentNullException("var");

            var d = Document.GetLineByOffset(var.Offset);
            TextArea.Caret.BringCaretToView();
            CaretOffset = d.Offset;
            ScrollToLine(d.LineNumber);


            var f = _foldingManager.GetFoldingsAt(d.Offset);
            if (f.Count > 0)
            {
            var fs = f[0];
            fs.IsFolded = false;
            }
            
            
        	
        	
        	FindText(var.Offset,var.Name);
        	JumpTo(var);
        }
        void FindText(int startOffset, string text)
        {
        	var start = Text.IndexOf(text,startOffset,StringComparison.OrdinalIgnoreCase);        	
        	SelectionStart =  start;
        	SelectionLength= text.Length;
         }
        
        public void FindText(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            SelectionStart = Text.IndexOf(text, CaretOffset, StringComparison.Ordinal);
        }

        public void ShowFindDialog()
        {
            FindAndReplaceForm.Instance.ShowDialog();
        }

        #endregion


        private void EditorPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var textEditorOptions = Options as EditorOptions;

            if (!textEditorOptions.MouseWheelZoom) return;
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            if (e.Delta <= 0 || !(FontSize < LogicListFontSizeMax))
            {
                FontSize -= 1;
            }
            else if ((e.Delta > 0) && (FontSize > LogicListFontSizeMin))
            {
                FontSize += 1;
            }
            e.Handled = true;
        }

        #region Folding Section

        private FoldingManager _foldingManager;
        private AbstractFoldingStrategy _foldingStrategy;
	

        [Localizable(false)]
        private void UpdateFolds()
        {
            var textEditorOptions = Options as EditorOptions;
            var foldingEnabled = textEditorOptions != null && textEditorOptions.EnableFolding;

            //if (File == null) return;
            if (SyntaxHighlighting == null)
                _foldingStrategy = null;


            // If Filename is null then return. no need for folding
            if (!File.Exists(Filename)) return;


            // Get XML Folding
            if (Path.GetExtension(Filename) == ".xml")
                _foldingStrategy = new XmlFoldingStrategy();
            else if (FileLanguage != null)
                _foldingStrategy = FileLanguage.FoldingStrategy;

            if (_foldingStrategy != null && foldingEnabled)
            {
                if (_foldingManager == null)
                    _foldingManager = FoldingManager.Install(TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, Document);
                RegisterFoldTitles();
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }



        /// <summary>
        /// Writes Titles for When the fold is closed
        /// </summary>
        private void RegisterFoldTitles()
        {

            if ((DocumentViewModel.Instance.FileLanguage is LanguageBase) && (Path.GetExtension(Filename) == ".xml")) return;

            foreach (var section in _foldingManager.AllFoldings)
                section.Title = DocumentViewModel.Instance.FileLanguage.FoldTitle(section, Document);
        }

        private string GetLine(int idx)
        {
        	var line = Document.GetLineByNumber(idx);
        	return Document.GetText(line.Offset,line.Length);
        }

        public string FindWord()
        {

            var line = GetLine(TextArea.Caret.Line);
            var search = line;
            char[] terminators = {' ', '=', '(', ')', '[', ']', '<', '>', '\r', '\n'};


            // Are there any terminators in the line?
            var end = line.IndexOfAny(terminators, TextArea.Caret.Column - 1);
            if (end > -1)
                search = (line.Substring(0, end));

            var start = search.LastIndexOfAny(terminators) + 1;

            if (start > -1)
                search = search.Substring(start).Trim();

            return search;
        }
        private bool GetCurrentFold(TextViewPosition loc)
        {
            var off = Document.GetOffset(loc.Location);

            var f= _foldingManager.GetFoldingsAt(off);
            if (f.Count == 0)
                return false;
            _toolTip = new ToolTip
            {
                Style = (Style)FindResource("FoldToolTipStyle"),
                DataContext = f,
                PlacementTarget = this,
                IsOpen = true
            };



    //         foreach (var fld in _foldingManager.AllFoldings)
    //         {
    //
    //             if (fld.StartOffset <= off && off <= fld.EndOffset && fld.IsFolded)
    //             {
    //             	toolTip = new System.Windows.Controls.ToolTip
    //             	{
    //             		Style = (Style)FindResource("FoldToolTipStyle"),
    //             		DataContext = fld,
    //             		PlacementTarget = this,
    //             		IsOpen=true
    //             	};
    //
    //                 
    //                 return true;
    //                
    //                // e.Handled = true;
    //             }
    //     }
                return true;
        }
        private void Mouse_OnHover(object sender, MouseEventArgs e)
        {
            if (_foldingManager == null) return;

            //UpdateFolds();
            var tvp =  GetPositionFromPoint(e.GetPosition(this));


            if (tvp.HasValue)
              e.Handled =  GetCurrentFold((TextViewPosition)tvp);


            //TODO _variables
            //    toolTip.PlacementTarget = this;
            //    // required for property inheritance 
            //    toolTip.Content = wordhover; 
            //    pos.ToString();
            //    toolTip.IsOpen = true;
            //    e.Handled = true;



            // Is Current Line a Variable?
            // ToolTip t = FileLanguage.variables.VariableToolTip(GetLine(pos.Value.Line));
            //   if (t != null)
            //   {
            //       t.PlacementTarget = this;
            //       t.IsOpen = true;
            //       e.Handled = true;
            //       disposeToolTip = false;
            //       return;
            //   }
            //
        }


        
        private void ToggleFolds()
        {
            if (_foldingManager == null) return;
            // Look for folding on this line: 
            var folding =
                _foldingManager.GetNextFolding(TextArea.Document.GetOffset(TextArea.Caret.Line,
                                                                           TextArea.Caret.Column));
            if (folding == null || Document.GetLineByOffset(folding.StartOffset).LineNumber != TextArea.Caret.Line)
            {
                // no folding found on current line: find innermost folding containing the caret
                folding = _foldingManager.GetFoldingsContaining(TextArea.Caret.Offset).LastOrDefault();
            }
            if (folding != null)
            {
                folding.IsFolded = !folding.IsFolded;
            }
        }

        private void ToggleAllFolds()
        {
            if (_foldingManager == null) return;
            foreach (var fm in _foldingManager.AllFoldings)
                fm.IsFolded = !fm.IsFolded;
        }

        void ChangeFoldStatus(bool isFolded)
        {
            foreach (var fm in _foldingManager.AllFoldings)
                fm.IsFolded = isFolded;
        }


        private void ShowDefinitions()
        {
            if (_foldingManager == null) return;
            foreach (var fm in _foldingManager.AllFoldings)
                fm.IsFolded = fm.Tag is NewFolding;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected  void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region FileWatcher


// ReSharper disable UnusedMember.Local
        private void SetWatcher()
// ReSharper restore UnusedMember.Local
        {
            var dir = Path.GetDirectoryName(Filename);
            var dirExists = dir != null && Directory.Exists(dir);
            //TODO Reimplement this
            // ReSharper disable RedundantJumpStatement
            if (!dirExists) return;
            // ReSharper restore RedundantJumpStatement

            // Only Watch For Module and Not individual files. This prevents from Reloading twice
        }




        public void Reload()
        {
            var answer = MessageBox.Show("Are you sure you want to reload file?", "Reload file", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (!(answer == MessageBoxResult.OK | (!IsModified))) return;
            Load(Filename);
            UpdateFolds();
        }





        #endregion



        [Localizable(false)]
        private void InsertSnippet()
                {
                    
#pragma warning disable 168
                    var loopCounter = new SnippetReplaceableTextElement {Text = "i"};
#pragma warning restore 168

                    var snippet = new Snippet
                                      {
                                          Elements =
                                              {
                                                  new SnippetTextElement {Text = "for "},
                                                  new SnippetReplaceableTextElement {Text = "item"},
                                                  new SnippetTextElement {Text = " in range("},
                                                  new SnippetReplaceableTextElement {Text = "from"},
                                                  new SnippetTextElement {Text = ", "},
                                                  new SnippetReplaceableTextElement {Text = "to"},
                                                  new SnippetTextElement {Text = ", "},
                                                  new SnippetReplaceableTextElement {Text = "step"},
                                                  new SnippetTextElement {Text = "):backN\t"},
                                                  new SnippetSelectionElement()
                                              }
                                      };
                    snippet.Insert(TextArea);


                }
        

        private void TextEditorGotFocus(object sender, RoutedEventArgs e)
        {
        	
        	DocumentViewModel.Instance.TextBox = this;

            
            OnPropertyChanged("Line");
            OnPropertyChanged("Column");
            OnPropertyChanged("Offset");
            OnPropertyChanged("RobotType");
            FileSave = !String.IsNullOrEmpty(Filename)? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture): String.Empty;

        }


        private ToolTip _toolTip = new ToolTip();

      
    }

    /// <summary>
    /// Animated rectangle around the caret.
    /// </summary>
    sealed class CaretHighlightAdorner : Adorner
    {
        readonly Pen _pen;
        readonly RectangleGeometry _geometry;

        public CaretHighlightAdorner(TextArea textArea)
            : base(textArea.TextView)
        {
            var min = textArea.Caret.CalculateCaretRectangle();
            min.Offset(-textArea.TextView.ScrollOffset);

            var max = min;
            var size = Math.Max(min.Width, min.Height) * 0.25;
            max.Inflate(size, size);

            _pen = new Pen(TextBlock.GetForeground(textArea.TextView).Clone(), 1);

            _geometry = new RectangleGeometry(min, 2, 2);
            _geometry.BeginAnimation(RectangleGeometry.RectProperty, new RectAnimation(min, max, new Duration(TimeSpan.FromMilliseconds(300))) { AutoReverse = true });
            _pen.Brush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200))) { BeginTime = TimeSpan.FromMilliseconds(450) });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, _pen, _geometry);
        }
    }
    /// <summary>
    /// Dont think im using this
    /// </summary>
    public class LineColorizer : DocumentColorizingTransformer
    {
        public string TextToSelect { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.Length == 0)
                return;

            if (line.Offset < StartOffset || line.Offset > EndOffset)
                return;

            var start = line.Offset > StartOffset ? line.Offset : StartOffset;
            var end = EndOffset > line.EndOffset ? line.EndOffset : EndOffset;

            ChangeLinePart(start, end, element => element.TextRunProperties.SetBackgroundBrush(Brushes.Red));

        }
    }
    public enum EDITORTYPE { SOURCE, DATA };

 
}

