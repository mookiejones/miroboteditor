// 4/11/2013 Registering Syntax highlighting was moved to TextEditorOptions. It only needs to be loaded one time.

// 4/23/2013 Changed the save routine. Something with the internal save function was saving weird characters in it. The should resolve the situation by only saving the Test 
// String on my own.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using miRobotEditor.Commands;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using AvalonDock.Layout;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using miRobotEditor.Classes;
using System.Collections.ObjectModel;
using miRobotEditor.Controls;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.ViewModel;
namespace miRobotEditor.GUI
{
  
    
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    [Localizable(false)]
    public partial class Editor : TextEditor, IDisposable,INotifyPropertyChanged
    {
        #region Constructor

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
        public Editor()
        {
            InitializeComponent();
            _iconBarMargin = new IconBarMargin(_iconBarManager = new IconBarManager());
            InitializeMyControl();
            MouseHoverStopped += delegate { toolTip.IsOpen = false; };
        }

        #endregion

        #region ViewModel Properties
        private EDITORTYPE _editortype;
    	public EDITORTYPE EditorType {get{return _editortype;}set{_editortype =value; OnPropertyChanged("EditorType");}}

        private IVariable _selectedVariable = null;
        public IVariable SelectedVariable { get { return _selectedVariable; } set { _selectedVariable = value; OnPropertyChanged("SelectedVariable"); } }

        private String _filename = string.Empty;
        public string Filename { get { return _filename; } set { _filename = value; OnPropertyChanged("Filename"); } }

        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; OnPropertyChanged("FileLanguage"); } }

        private ObservableCollection<IVariable> _variables = new ObservableCollection<IVariable>();
        public ObservableCollection<IVariable> Variables { get { return _variables; } set { _variables = value;  } }




        public bool IsModified
        {
            get { return base.IsModified; }
            set { base.IsModified = value; OnPropertyChanged("IsModified");} 
        }
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
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand((p) =>Save(), (p) => CanSave())); }
        }

        private RelayCommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new RelayCommand((p) => SaveAs(), (p) => CanSave())); }
        }

        private RelayCommand _replaceCommand;
        public ICommand ReplaceCommand
        {
            get { return _replaceCommand ?? (_replaceCommand = new RelayCommand((p) => Replace(), (p) => true)); }
        }


        private RelayCommand _variableDoubleClickCommand;
        public ICommand VariableDoubleClickCommand
        {
            get { return _variableDoubleClickCommand ?? (_variableDoubleClickCommand = new RelayCommand((p) => SelectText((IVariable)((ListViewItem)p).Content), (p) => p != null)); }
        }

        private RelayCommand _gotoCommand;
        public ICommand GotoCommand
        {
            get { return _gotoCommand ?? (_gotoCommand = new RelayCommand((p) => Goto(), (p) =>(!String.IsNullOrEmpty(Text)))); }
        }
        private RelayCommand _openAllFoldsCommand;
        public ICommand OpenAllFoldsCommand
        {
            get { return _openAllFoldsCommand ?? (_openAllFoldsCommand = new RelayCommand((p) => ChangeFoldStatus(false), (p) => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _toggleCommentCommand;
        public ICommand ToggleCommentCommand
        {
            get { return _toggleCommentCommand ?? (_toggleCommentCommand = new RelayCommand((p) => ToggleComment(), (p) => (!String.IsNullOrEmpty(FileLanguage.CommentChar)))); }
        }


        private RelayCommand _toggleFoldsCommand;
        public ICommand ToggleFoldsCommand
        {
            get { return _toggleFoldsCommand ?? (_toggleFoldsCommand = new RelayCommand((p) => ToggleFolds(), (p) => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _toggleAllFoldsCommand;
        public ICommand ToggleAllFoldsCommand
        {
            get { return _toggleAllFoldsCommand ?? (_toggleAllFoldsCommand = new RelayCommand((p) => ToggleAllFolds(), (p) => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _closeAllFoldsCommand;
        public ICommand CloseAllFoldsCommand
        {
            get { return _closeAllFoldsCommand ?? (_closeAllFoldsCommand = new RelayCommand((p) => ChangeFoldStatus(true), (p) => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }

        private RelayCommand _findCommand;
        public ICommand FindCommand
        {
            get { return _findCommand ?? (_findCommand = new RelayCommand((p) => ChangeFoldStatus(true), (p) => ((_foldingManager != null) && (_foldingManager.AllFoldings.Any())))); }
        }
        private RelayCommand _reloadCommand;
        public ICommand ReloadCommand
        {
            get { return _reloadCommand ?? (_reloadCommand = new RelayCommand((p) => Reload(), (p) => true)); }
        }

        private RelayCommand _showDefinitionsCommand;
        public ICommand ShowDefinitionsCommand
        {
            get { return _showDefinitionsCommand ?? (_showDefinitionsCommand = new RelayCommand((p) => ShowDefinitions(), (p) => (_foldingManager != null ))); }
        }

        private RelayCommand _cutCommand;
        public ICommand CutCommand
        {
            get { return _cutCommand ?? (_cutCommand = new RelayCommand((p) => Cut(), (p) => (Text.Length>0))); }
        }
        private RelayCommand _copyCommand;
        public ICommand CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new RelayCommand((p) => Cut(), (p) => (Text.Length > 0))); }
        }
        private RelayCommand _pasteCommand;
        public ICommand PasteCommand
        {
            get { return _pasteCommand ?? (_pasteCommand = new RelayCommand((p) => Paste(), (p) => (Clipboard.ContainsText()))); }
        }

        private RelayCommand _FunctionWindowClickCommand;
        public ICommand FunctionWindowClickCommand
        {
            get { return _FunctionWindowClickCommand ?? (_FunctionWindowClickCommand = new Commands.RelayCommand(param => OpenFunctionItem(param), param => true)); }
        }

        private RelayCommand _changeIndentCommand;
        public ICommand ChangeIndentCommand
        {
            get { return _changeIndentCommand ?? (_changeIndentCommand = new Commands.RelayCommand(param => ChangeIndent(param), param => true)); }
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
            var i = (IVariable)((System.Windows.Controls.ListViewItem)parameter).Content;
            DummyDoc.Instance.TextBox.SelectText(i);
        }
        
        
        #region CaretPositionChanged - Bracket Highlighting

        private readonly MyBracketSearcher _bracketSearcher = new MyBracketSearcher();
        private BracketHighlightRenderer _bracketRenderer;

        /// <summary>
        /// Highlights matching brackets.
        /// </summary>
        private void HighlightBrackets(object sender, EventArgs e)
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
            var s = sender as ICSharpCode.AvalonEdit.Editing.Caret;

            UpdateLineTransformers();
            if (s != null)
            {
            	
                StatusBarViewModel.Instance.Line = s.Line;
                StatusBarViewModel.Instance.Column = s.Column;

            OnPropertyChanged("Line");
            OnPropertyChanged("Column");
            FileSave = !String.IsNullOrEmpty(Filename)? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture): String.Empty;

                
            }
            StatusBarViewModel.Instance.Offset = CaretOffset;
            OnPropertyChanged("Offset");



            HighlightBrackets(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLineTransformers()
        {
            // Clear the Current Renderers
            TextArea.TextView.BackgroundRenderers.Clear();
            var textEditorOptions = Options as TextEditorOptions;
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

         // Need to Add Host

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
                Variables.Add(new Variable
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
   //         if (Variables == null)
                Variables = new ObservableCollection<IVariable>();
      //      else
       //     Variables.Clear();
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
            var commandBindings = TextArea.CommandBindings;
            var inputBindings = TextArea.InputBindings;
            inputBindings.Add(new KeyBinding(ApplicationCommands.Find, Key.F, ModifierKeys.Control));
            inputBindings.Add(new KeyBinding(ApplicationCommands.Replace, Key.R, ModifierKeys.Control));
        }
		

		
        public void ChangeIndent(object param)
        {
        	
        	
        	try{
        		
        	
   		        bool increase = Convert.ToBoolean(param);

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
                MessageViewModel.AddError(ex);
            }
        }
        
     
        /// <summary>
        /// Evaluates each line in selection and Comments/Uncomments "Each Line"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        	if (File.Exists(Filename))
        		 return IsModified;
        	else
        		 return IsModified;
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
            var gtd = new GotoDialog();
            gtd.DataContext = vm;
            gtd.ShowDialog().GetValueOrDefault();

        }


        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open,FileAccess.Read,FileShare.None);

            }
            catch (IOException ex)
            {
                var msg = new OutputWindowMessage{Title="Editor",Description= String.Format("File is locked! \r\n {0}",ex.ToString())};
                MessageViewModel.Instance.Messages.Add(msg);
                
                return true;
            }
            finally
            {
                if (stream!=null)
                stream.Close();
            }
            return false;
        }


        public void SaveAs()
        {       	
        		Filename= GetFilename();

                var islocked = IsFileLocked(new FileInfo(Filename));

                if (islocked) 
                    return;
                
                File.WriteAllText(Filename,Text);

                var p = DummyDoc.Instance.Host as LayoutDocument;
                if (p != null) p.Title = Path.GetFileNameWithoutExtension(Filename);
                // MainWindow.Instance.RecentFileList.InsertFile(Filename);
            MessageViewModel.Add(new OutputWindowMessage{Title = "File Saved", Description = Filename, Icon = null});
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

            ofd.ShowDialog().GetValueOrDefault();
            
            return ofd.FileName;
            	
        }
        public void Save()
        {
            //_watcher.EnableRaisingEvents = false;
            // _watcher.Text = Text;
            
            if (String.IsNullOrEmpty(Filename))
            	Filename = GetFilename();

            if (IsFileLocked(new FileInfo(Filename))) return;
                
                File.WriteAllText(Filename,Text);
			
            StatusBarViewModel.Instance.FileSave = File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture);
            IsModified = false;
            //_watcher.EnableRaisingEvents = true;
            // Suspend the calling thread until the file has been deleted. 
        }

        #endregion

        protected override void OnOptionChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableFolding":
                    UpdateFolds();
                    break;
            }
            base.OnOptionChanged(e);
        }

        

        public void SetHighlighting()
        {
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(Filename));
        }

        public void UpdateVisualText()
        {
        	
        	
            EditorTextChanged(null, null);
            if (FileLanguage != null && (!(FileLanguage is LanguageBase)))
            {
                var syn = HighlightingManager.Instance.GetDefinition(FileLanguage.RobotType.ToString());
                SyntaxHighlighting = syn;
            }
        }

        private void EditorTextChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Filename");
            IsModified = true;
            FindBookmarkMembers();
            UpdateFolds();
        }

        #region Code Completion

        private CompletionWindow _completionWindow;

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (FileLanguage == null | FileLanguage is LanguageBase) return;

            var currentword = FindWord();
            // Dont Show Completion window until there are 3 Characters
            if (currentword != null && (String.IsNullOrEmpty(currentword)) | currentword.Length < 3) return;

            _completionWindow = new CompletionWindow(TextArea);

            FileLanguage.CompletionList(this,currentword, _completionWindow.CompletionList.CompletionData);

            _completionWindow.Closed += delegate { _completionWindow = null; };
            _completionWindow.CloseWhenCaretAtBeginning = true;
            _completionWindow.CompletionList.SelectItem(currentword);
            if (_completionWindow.CompletionList.SelectedItem != null)
                _completionWindow.Show();

            if (IsModified)
                UpdateFolds();
        }


        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
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

                var d = Document.GetLineByOffset(nIndex);
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
            if (TextEditorOptions.Instance.EnableAnimations)
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
        	if (var.Name == null) throw new ArgumentNullException("text");

            var d = DummyDoc.Instance.TextBox.Document.GetLineByOffset(var.Offset);
            TextArea.Caret.BringCaretToView();
            CaretOffset = d.Offset;
            ScrollToLine(d.LineNumber);


            var f = _foldingManager.GetFoldingsAt(d.Offset);
            if (f.Count > 0)
            {
            var fs = f[0];
            fs.IsFolded = false;
            }
            
            
        	
        	
        	this.FindText(var.Offset,var.Name);
        	this.JumpTo(var);
        }
        void FindText(int StartOffset, string text)
        {
        	var start = Text.IndexOf(text,StartOffset,StringComparison.OrdinalIgnoreCase);        	
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
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
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
        }

        #region Folding Section

        private FoldingManager _foldingManager;
        private AbstractFoldingStrategy _foldingStrategy;

        [Localizable(false)]
        private void UpdateFolds()
        {
            var textEditorOptions = Options as TextEditorOptions;
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
            if ((DummyDoc.Instance.FileLanguage is LanguageBase) && (Path.GetExtension(Filename) == ".xml")) return;

            foreach (var section in _foldingManager.AllFoldings)
            {
                section.Title = DummyDoc.Instance.FileLanguage.FoldTitle(section, Document);
            }
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

             foreach (var fld in _foldingManager.AllFoldings)
             {
    
                 if (fld.StartOffset <= off && off <= fld.EndOffset && fld.IsFolded)
                 {
                 	toolTip = new System.Windows.Controls.ToolTip
                 	{
                 		Style = (Style)FindResource("FoldToolTipStyle"),
                 		DataContext = fld,
                 		PlacementTarget = this,
                 		IsOpen=true
                 	};
    
                     
                     return true;
                    
                    // e.Handled = true;
                 }
         }
                return false;
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
            if (_foldingManager != null)
            {
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
        }


        private void ToggleAllFolds()
        {
            if (_foldingManager != null)
            {
                foreach (var fm in _foldingManager.AllFoldings)
                    fm.IsFolded = !fm.IsFolded;
            }
        }

        void ChangeFoldStatus(bool isFolded)
        {
            foreach (var fm in _foldingManager.AllFoldings)
                fm.IsFolded = isFolded;
        }


        private void ShowDefinitions()
        {
            if (_foldingManager != null)
            {
                foreach (var fm in _foldingManager.AllFoldings)
                	fm.IsFolded = fm.Tag is NewFolding;
            }
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


        private void SetWatcher()
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
            if (answer == MessageBoxResult.OK |(!IsModified))
            {
                Load(Filename);
                UpdateFolds();
            }
        }





        #endregion


        //TODO Setup Snippets


        
        private void InsertSnippet(object sender, KeyEventArgs e)
                {
                    
                    var loopCounter = new SnippetReplaceableTextElement {Text = "i"};

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
        	
        	//DummyDocViewModel.Instance.TextBox = this;

            if (File.Exists(Filename))
                StatusBarViewModel.Instance.Name = Filename;

            switch (FileLanguage.RobotType)
            {
                case Typlanguage.Fanuc:
                case Typlanguage.ABB:
                case Typlanguage.KUKA:
                case Typlanguage.KAWASAKI:
                    StatusBarViewModel.Instance.Robot = FileLanguage.RobotType.ToString();
                    break;
                default:
                    StatusBarViewModel.Instance.Robot = String.Empty;
                    break;
            }
           
            
            OnPropertyChanged("Line");
            OnPropertyChanged("Column");
            OnPropertyChanged("Offset");
            FileSave = !String.IsNullOrEmpty(Filename)? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture): String.Empty;


            //HACK Trying to fix the Function Window
            FindBookmarkMembers();
            
        }

        public int Line{get{return TextArea.Caret.Line;}}
        public int Column{get{return TextArea.Caret.Column;}}
        public int Offset{get{return TextArea.Caret.Offset;}}
        

        private string _fileSave = string.Empty;
        public string FileSave{get{return _fileSave;}set{_fileSave=value;OnPropertyChanged("FileSave");}}
        
        
        public void Dispose()
        {
            if (_iconBarMargin != null)
                _iconBarMargin.Dispose();
        }

        private System.Windows.Controls.ToolTip toolTip = new System.Windows.Controls.ToolTip();
        

        private void lbMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var i = (IVariable)((System.Windows.Controls.ListViewItem)sender).Content;
            SelectText(i);
        }
    }

    public enum EDITORTYPE { SOURCE, DATA };
}

