using System;
using System.ComponentModel;
using System.Globalization;
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
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using miRobotEditor.Controls;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;

namespace miRobotEditor.GUI
{
    public delegate void UpdateFunctionEventHandler(object sender, FunctionEventArgs e);

    
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    [Localizable(false)]
    public partial class Editor : TextEditor,IDisposable
    {
        /// <summary>
        /// Used for Highlighting Background
        /// </summary>
        private class XBackgroundRenderer:IBackgroundRenderer
        {
            private readonly DocumentLine _line;
            public XBackgroundRenderer(DocumentLine line)
            {
                _line=line;
            }
            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                textView.EnsureVisualLines();

                if (!_line.IsDeleted)
                {
                    var segment = new TextSegment {StartOffset = _line.Offset, EndOffset = _line.EndOffset};
                    foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
                    {
                        drawingContext.DrawRoundedRectangle(Brushes.Yellow, new Pen(Brushes.Red, 0),
                                                            new Rect(r.Location,
                                                                     new Size(textView.ActualWidth, r.Height)), 3, 3);
                    }
                }
            }

            public KnownLayer Layer { get; private set; }
        }

        #region Static Members
        public static readonly RoutedCommand Goto = new RoutedCommand();
        public static new readonly RoutedCommand Save = new RoutedCommand();
        public static readonly RoutedCommand ToggleFolding = new RoutedCommand();
        public static readonly RoutedCommand ToggleAllFolding = new RoutedCommand();
        public static readonly RoutedCommand ShowDefinitions = new RoutedCommand();
        public static readonly RoutedCommand OpenAllFolds = new RoutedCommand();
        public static readonly RoutedCommand CloseAllFolds = new RoutedCommand();
        public static readonly RoutedCommand IncreaseIndent = new RoutedCommand();
        public static readonly RoutedCommand DecreaseIndent = new RoutedCommand();
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

        #region Public Events

        public event UpdateFunctionEventHandler UpdateFunctions;

        #endregion

        #region Properties
        public event EventHandler ReloadFile;

        public void OnReloadFile(EventArgs e)
        {
            EventHandler handler = ReloadFile;
            if (handler != null) handler(this, e);
        }

      //  public FileInfo File { get; private set; }


        public string FileName { get; set; }



        #endregion

        #region Constructor

        public Editor()
        {           
//            InitializeComponent();
            this.Options = TextEditorOptions.Instance;
            this.ShowLineNumbers = true;
            RegisterSyntaxHighlighting();            
            _iconBarMargin = new IconBarMargin(_iconBarManager = new IconBarManager());
            TextArea.LeftMargins.Insert(0, _iconBarMargin);

             TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(TextArea));

            AddBindings();

            TextArea.TextEntered += TextEntered;          
            TextArea.TextEntering += TextEntering;
            TextArea.Caret.PositionChanged += CaretPositionChanged;
            DataContext = this;
        }
        #endregion

       
        #region CaretPositionChanged - Bracket Highlighting

        readonly MyBracketSearcher _bracketSearcher = new MyBracketSearcher();
        BracketHighlightRenderer _bracketRenderer;

		/// <summary>
		/// Highlights matching brackets.
		/// </summary>
		void HighlightBrackets(object sender, EventArgs e)
		{
			/*
			 * Special case: ITextEditor.Language guarantees that it never returns null.
			 * In this case however it can be null, since this code may be called while the document is loaded.
			 * ITextEditor.Language gets set in CodeEditorAdapter.FileNameChanged, which is called after
			 * loading of the document has finished.
			 * */
			
			
					var bracketSearchResult = _bracketSearcher.SearchBracket(Document ,this.TextArea.Caret.Offset);
						this._bracketRenderer.SetHighlight(bracketSearchResult);
		}
		
		
		#endregion
		
  
		void CaretPositionChanged(object sender, EventArgs e)
        {
            var s = sender as ICSharpCode.AvalonEdit.Editing.Caret;
            
            UpdateLineTransformers();
            if (s != null)
            {
                StatusBar.Instance.Line = s.Line.ToString(CultureInfo.InvariantCulture);
                StatusBar.Instance.Column =  s.Column.ToString(CultureInfo.InvariantCulture);
            }
            StatusBar.Instance.Offset = CaretOffset.ToString(CultureInfo.InvariantCulture);
            
            
            HighlightBrackets(sender,e);
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateLineTransformers()
        {
            // Clear the Current Renderers
            TextArea.TextView.BackgroundRenderers.Clear();
            var textEditorOptions = Options as TextEditorOptions;
            if (textEditorOptions != null && textEditorOptions.HighlightCurrentLine)
            TextArea.TextView.BackgroundRenderers.Add(new XBackgroundRenderer(Document.GetLineByOffset(CaretOffset)));
          if (_bracketRenderer==null)
          _bracketRenderer = new BracketHighlightRenderer(TextArea.TextView);
          else
          TextArea.TextView.BackgroundRenderers.Add(_bracketRenderer);            
        }


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
                            if (CanToggleFolding())
                                ExecuteToggleFolding(this, null);
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


        #region Syntax Highlighting
        /// <summary>
        /// Loads all of syntax Highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        [Localizable(false)]
        private void Register(string name, string[] ext)
        {
            var filename = String.Format("miRobotEditor.Controls.SyntaxHighlighting.{0}Highlight.xshd", name);
            using (Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                //KUKAHighlight.xshd
                IHighlightingDefinition customHighlighting;
                using (var reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }

                HighlightingManager.Instance.RegisterHighlighting(name, ext, customHighlighting);
            }

        }

        private void RegisterSyntaxHighlighting()
        {
            Register("KUKA", Languages.KUKA.Ext.ToArray());
            Register("KAWASAKI", Kawasaki.EXT.ToArray());
            Register("Fanuc", Fanuc.EXT.ToArray());
            Register("ABB", ABB.EXT.ToArray());         
        }

        #endregion

        private void AddBookMark(int lineNumber, string imgpath)
        {
            var bitmap = Utilities.LoadBitmap(imgpath);
            var bmi = new BookmarkImage(bitmap);
            _iconBarManager.Bookmarks.Add(new ClassMemberBookmark(lineNumber, bmi));
        }

        private void FindMatches(Regex matchstring, string imgPath)
        {
            // Dont Include Empty Values
            if (String.IsNullOrEmpty(matchstring.ToString())) return;

            var m = matchstring.Match(Text.ToLower());
            while (m.Success)
            {
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
            if (DummyDoc.Instance.FileLanguage == null) return;
            _iconBarManager.Bookmarks.Clear();

            FindMatches(DummyDoc.Instance.FileLanguage.MethodRegex, Global.ImgMethod);
            FindMatches(DummyDoc.Instance.FileLanguage.StructRegex, Global.ImgStruct);
            FindMatches(DummyDoc.Instance.FileLanguage.FieldRegex, Global.ImgField);
            FindMatches(DummyDoc.Instance.FileLanguage.SignalRegex, Global.ImgSignal);
            FindMatches(DummyDoc.Instance.FileLanguage.EnumRegex, Global.ImgEnum);
            FindMatches(DummyDoc.Instance.FileLanguage.XYZRegex, Global.ImgXyz);

            if (DummyDoc.Instance.FileLanguage is Languages.KUKA)
                FindMatches(new Regex("DECL [a-zA-Z0-9_$]+", (RegexOptions) 3), Global.ImgValue);
        }

        #region Editor.Bindings

        private void AddBindings()
        {
            var commandBindings = TextArea.CommandBindings;
            commandBindings.Add(new CommandBinding(ApplicationCommands.Find));
            commandBindings.Add(new CommandBinding(ApplicationCommands.Replace));
            commandBindings.Add(new CommandBinding(ApplicationCommands.Print));
            commandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview));
            commandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs));
            commandBindings.Add(new CommandBinding(Goto, ExecuteGoto, CanGoto));
            commandBindings.Add(new CommandBinding(Save, ExecuteSave, CanSave));
            commandBindings.Add(new CommandBinding(ToggleFolding, ExecuteToggleFolding, CanToggleFolding));
            commandBindings.Add(new CommandBinding(ToggleAllFolding, ExecuteToggleAllFolding, CanToggleAllFolding));
            commandBindings.Add(new CommandBinding(ShowDefinitions, ExecuteShowDefinitions, CanShowDefinitions));
            commandBindings.Add(new CommandBinding(OpenAllFolds, ExecuteOpenAllFolds, CanOpenAllFolds));
            commandBindings.Add(new CommandBinding(CloseAllFolds, ExecuteCloseAllFolds, CanCloseAllFolds));
        //    commandBindings.Add(new CommandBinding(ToggleComment, ExecuteToggleComment, CanToggleComment));
            commandBindings.Add(new CommandBinding(IncreaseIndent, ExecuteIncreaseIndent, CanIncreaseIndent));
            commandBindings.Add(new CommandBinding(DecreaseIndent, ExecuteDecreaseIndent, CanDecreaseIndent));
            var inputBindings = TextArea.InputBindings;
            inputBindings.Add(new KeyBinding(ApplicationCommands.Find, Key.F, ModifierKeys.Control));
            inputBindings.Add(new KeyBinding(ApplicationCommands.Replace, Key.R, ModifierKeys.Control));
       //     inputBindings.Add(new KeyBinding(ToggleComment, Key.C, (int) ModifierKeys.Shift + ModifierKeys.Control));
        }

       
        private void CanDecreaseIndent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TextArea.IndentationStrategy != null;
        }

        private void CanIncreaseIndent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TextArea.IndentationStrategy != null;
        }
        
        private void ExecuteDecreaseIndent(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
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

                        positions = positions > 1 ? positions - 1 : positions;

                        if (positions > 1)
                            Document.Replace(line.Offset, currentline.Length, currentline.Substring(1));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageViewModel.Instance.AddError(ex);
            }

        }

        private void ExecuteIncreaseIndent(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var start = Document.GetLineByOffset(SelectionStart);
                var end = Document.GetLineByOffset(SelectionStart + SelectionLength);
                var positions = 0;
                using (Document.RunUpdate())
                {
                    for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                    {
                        var currentline = GetLine(line.LineNumber);
                        var rgx = new Regex(@"(^[\s]+)");

                        Match m = rgx.Match(currentline);
                        if (m.Success)
                            positions = m.Groups[1].Length;
                            Document.Insert(line.Offset + positions , " ");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageViewModel.Instance.AddError(ex);
            }
        }

        /// <summary>
        /// Evaluates each line in selection and Comments/Uncomments "Each Line"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteToggleComment(object sender, ExecutedRoutedEventArgs e)
        {
            //No point in commenting if I dont know the Language
            if (DummyDoc.Instance.FileLanguage == null) return;

            // Get Comment to insert
            var start = Document.GetLineByOffset(SelectionStart);
            var end = Document.GetLineByOffset(SelectionStart + SelectionLength);

            using (Document.RunUpdate())
            {
                for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                {
                    var currentline = GetLine(line.LineNumber);
                    DummyDoc.Instance.FileLanguage.IsLineCommented(currentline);

                    // Had to put in comment offset for Fanuc 
                    if (!DummyDoc.Instance.FileLanguage.IsLineCommented(currentline))
                        Document.Insert(DummyDoc.Instance.FileLanguage.CommentOffset(currentline) + line.Offset, DummyDoc.Instance.FileLanguage.CommentChar);
                    else
                    {
                        var replacestring = DummyDoc.Instance.FileLanguage.CommentReplaceString(currentline);
                        Document.Replace(line.Offset, currentline.Length, replacestring);
                    }
                }
            }
        }

        private void CanToggleComment(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DummyDoc.Instance.FileLanguage != null)
                e.CanExecute = !String.IsNullOrEmpty(DummyDoc.Instance.FileLanguage.CommentChar);
        }

        private void CanReplace(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
           
                e.CanExecute = (File.Exists(FileName) && IsModified) ? true : false;            
        }

        private void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Text.Length > 0;
        }

        private void ReplaceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Make sure we update the Editor _instance           
            FindAndReplaceForm.Instance = new FindAndReplaceForm
                                              {Left = Mouse.GetPosition(this).X, Top = Mouse.GetPosition(this).Y};
            FindAndReplaceForm.Instance.Show();

        }

        private void CanGoto(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void ExecuteGoto(object sender, ExecutedRoutedEventArgs e)
        {
            new GotoDialog {Editor = this};
        }


        private void ExecuteSaveAs(object target, ExecutedRoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.SaveFileDialog {Title = "Save As", Filter = "All _files(*.*)|*.*"};

            if (!String.IsNullOrEmpty(FileName))
            {
                ofd.FileName = FileName;
                ofd.Filter += String.Format("|Current Type (*{0})|*{0}", Path.GetExtension(FileName) );
                ofd.FilterIndex = 2;
                ofd.DefaultExt = Path.GetExtension(FileName);
            }

            ofd.ShowDialog();

            if (!String.IsNullOrEmpty(ofd.FileName))
            {
                FileName = ofd.FileName;
                Save(FileName);
                var p = DummyDoc.Instance.Host as LayoutDocument;
                if (p != null) p.Title = Path.GetFileNameWithoutExtension(FileName);
                MainWindow.Instance.RecentFileList.InsertFile(FileName);
                MessageViewModel.Instance.Messages.Add(new OutputWindowMessage { Title = "_file Saved", Description = FileName, Icon = null });
            }
        }

        private void ExecuteSave(object target, ExecutedRoutedEventArgs e)
        {
            //_watcher.EnableRaisingEvents = false;
           // _watcher.Text = Text;
                Save(FileName);

                StatusBar.Instance.FileSave = System.IO.File.GetLastWriteTime(FileName).ToString(CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Initialize Internal Properties
        /// </summary>
        public new void Load(string filename)
        {
            FileName = filename;
            base.Load(filename);
            SetWatcher();           
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(filename));
         }

        public void UpdateVisualText()
        {
              EditorTextChanged(null, null);
              if (DummyDoc.Instance.FileLanguage != null && (!(DummyDoc.Instance.FileLanguage is LanguageBase)))
              {
                  var syn = HighlightingManager.Instance.GetDefinition(DummyDoc.Instance.FileLanguage.RobotType.ToString());
                  SyntaxHighlighting = syn;
              }
        }

        private void EditorTextChanged(object sender, EventArgs e)
        {                      
            RaiseUpdate(null,new DependencyPropertyChangedEventArgs());
        }

        #region Code Completion
        private CompletionWindow _completionWindow;

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (DummyDoc.Instance.FileLanguage == null | DummyDoc.Instance.FileLanguage is LanguageBase) return;

            var currentword = FindWord();
            // Dont Show Completion window until there are 3 Characters
            if (currentword != null && (String.IsNullOrEmpty(currentword)) |currentword.Length<3)return;

            _completionWindow = new CompletionWindow(TextArea);

            DummyDoc.Instance.FileLanguage.CompletionList(currentword, _completionWindow.CompletionList.CompletionData);
            _completionWindow.Closed += delegate { _completionWindow = null; };
            _completionWindow.CloseWhenCaretAtBeginning = true;
            _completionWindow.CompletionList.SelectItem(currentword);
            if (_completionWindow.CompletionList.SelectedItem != null)
            _completionWindow.Show();
            
             if (IsModified)
            RaiseUpdate(null,new DependencyPropertyChangedEventArgs());
            
         
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

        public void SelectText(int start, string text)
        {
            try
            {

            if (text == null) throw new ArgumentNullException("text");

            var d = Document.GetLineByOffset(start);
            TextArea.Caret.BringCaretToView();
            CaretOffset = d.Offset;
                JumpTo(d.LineNumber,0);
            if (_foldingManager != null)
            {
                var f = _foldingManager.GetFoldingsAt(d.Offset);
                if (f.Count > 0)
                {
                    var fs = f[0];
                    fs.IsFolded = false;
                }
            }
            }
            catch (Exception ex)
            {

                MessageViewModel.Instance.AddError(ex);
            }
        }
        public void FindText()
        {

            int nIndex = Text.IndexOf(FindReplaceViewModel.Instance.LookFor, CaretOffset, StringComparison.Ordinal);
            if (nIndex > -1)
            {
               
                DocumentLine d = Document.GetLineByOffset(nIndex);
//                ScrollTo(d.LineNumber, 0);
                JumpTo(d.LineNumber,0);
                SelectionStart = nIndex;
                SelectionLength = FindReplaceViewModel.Instance.LookFor.Length;
            }
            else
            {
                FindReplaceViewModel.Instance.SearchResult = "No Results Found, Starting Search from Beginning";
                CaretOffset=0;
            }

        }

        public void JumpTo(int line, int column)
        {
            Focus();
            ScrollTo(line, column);

            if (TextEditorOptions.Instance.EnableAnimations)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)DisplayCaretHighlightAnimation);
        }
        
        void DisplayCaretHighlightAnimation()
        {

          if (TextArea == null)
              return;
        
          var layer = AdornerLayer.GetAdornerLayer(TextArea.TextView);
        
          if (layer == null)
              return;
        
          var adorner = new CaretHighlightAdorner(TextArea);
          layer.Add(adorner);
        //
        //  WorkbenchSingleton.CallLater(TimeSpan.FromSeconds(1), (Action)(() => layer.Remove(adorner)));
        }

        /// <summary>
        /// Select Text
        /// <remarks>Used with Function window</remarks>
        /// </summary>
        /// <param name="text"></param>
        public void SelectText(string text)
        {
            if (text == null) throw new ArgumentNullException("text");


            var d = DummyDoc.Instance.TextBox.Document.GetLineByOffset(Text.IndexOf(text, 0, StringComparison.Ordinal));
            TextArea.Caret.BringCaretToView();
            CaretOffset = d.Offset;
            ScrollToLine(d.LineNumber);
          
          
            var f = _foldingManager.GetFoldingsAt(d.Offset);
            if (f.Count > 0)
            {
                var fs = f[0];
                fs.IsFolded = false;
            }

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
        
        private void RaiseUpdate(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible) return;

            if (UpdateFunctions != null)
                UpdateFunctions(this, new FunctionEventArgs(Text));
           
            FindBookmarkMembers();
            UpdateFolds();
        }

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

            
            // Get XML Folding
            if (Path.GetExtension(FileName) == ".xml")
                _foldingStrategy = new XmlFoldingStrategy();
            else
                if (DummyDoc.Instance.FileLanguage != null)
                    _foldingStrategy = DummyDoc.Instance.FileLanguage.FoldingStrategy;
            
           if (_foldingStrategy != null && foldingEnabled)
            {
                if (_foldingManager == null)
                    _foldingManager = FoldingManager.Install(TextArea);
               //this.BeginChange();
                _foldingStrategy.UpdateFoldings(_foldingManager, Document);
                RegisterFoldTitles();
               //this.EndChange();
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
            if ((DummyDoc.Instance.FileLanguage is LanguageBase) && (Path.GetExtension(FileName) == ".xml")) return;

            foreach (var section in _foldingManager.AllFoldings)
            {
                section.Title = DummyDoc.Instance.FileLanguage.FoldTitle(section, Document);
            }
        }


        private string GetLine(int linenumber)
        {
            var offset = Document.GetLineByNumber(linenumber).Offset;
            var length = Document.GetLineByNumber(linenumber).Length;
            return Document.GetText(offset, length);
        }

        public string FindWord()
        {
           
            string line = GetLine(TextArea.Caret.Line);
            string search = line;
            char[] terminators = { ' ', '=', '(', ')', '[', ']', '<', '>' ,'\r','\n'};


            // Are there any terminators in the line?
       


            int end = line.IndexOfAny(terminators, TextArea.Caret.Column-1);
            if (end > -1)
                search = (line.Substring(0, end));

            int start = search.LastIndexOfAny(terminators) + 1;

            if (start > -1)
                search = search.Substring(start).Trim();
           
            return search;
        }


        private void Mouse_OnHover(object sender, MouseEventArgs e)
        {
            if (_foldingManager == null) return;

            //UpdateFolds();
            GetPositionFromPoint(e.GetPosition(this));

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

            //  if (Document != null)
            //  {
            //      // Is it really necessary to have a tooltip for the folds?
            //      var Fold = GetCurrentFold(e);
            //      if ((Fold != null) && (ToolTip != null))
            //      {
            //
            //          ToolTipTitle = Fold.ToolTip.Title;
            //          ToolTipMessage = Fold.Message;
            //          ToolTipAdditional = Fold.Text;
            //      }
            //  }
        }


        private bool CanToggleFolding()
        {
            return ((_foldingManager != null) && (_foldingManager.AllFoldings.Any()));
        }

        private void CanToggleFolding(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanToggleFolding();
        }

        private void CanToggleAllFolding(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanToggleFolding();
        }

        private void ExecuteToggleFolding(object sender, ExecutedRoutedEventArgs e)
        {
            if (_foldingManager != null)
            {
                // Look for folding on this line: 
                FoldingSection folding =
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


        private void ExecuteToggleAllFolding(object sender, ExecutedRoutedEventArgs e)
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection fm in _foldingManager.AllFoldings)
                    fm.IsFolded = !fm.IsFolded;
            }
        }

        private void CanOpenAllFolds(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((_foldingManager != null) && (_foldingManager.AllFoldings.Any()));
        }

        private void ExecuteOpenAllFolds(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (FoldingSection fm in _foldingManager.AllFoldings)
                fm.IsFolded = false;
        }

        private void CanCloseAllFolds(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_foldingManager != null && _foldingManager.AllFoldings.Any());
        }

        private void ExecuteCloseAllFolds(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (FoldingSection fm in _foldingManager.AllFoldings)
                fm.IsFolded = true;
        }


        private void CanShowDefinitions(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _foldingManager != null;
        }

        private void ExecuteShowDefinitions(object sender, ExecutedRoutedEventArgs e)
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection fm in _foldingManager.AllFoldings)
                    fm.IsFolded = fm.Tag is NewFoldingDefinition;
            }
        }

        private abstract class NewFoldingDefinition : NewFolding
        {
            protected NewFoldingDefinition(int start, int end) : base(start, end)
            {
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region FileWatcher

        
        private void SetWatcher()
        {
            //TODO Reimplement this
            if (Directory.Exists(Path.GetDirectoryName(FileName)) == null) return;

            // Only Watch For Module and Not individual files. This prevents from Reloading twice
        }

       


        public void Reload()
        {
            base.Load(FileName);    
            UpdateFolds();
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
            if (File.Exists(FileName))
            StatusBar.Instance.Name = FileName;

            switch (DummyDoc.Instance.FileLanguage.RobotType)
            {
                case TYPLANGUAGE.Fanuc:
                case TYPLANGUAGE.ABB:
                case TYPLANGUAGE.KUKA:
                case TYPLANGUAGE.KAWASAKI:
                    StatusBar.Instance.Robot = DummyDoc.Instance.FileLanguage.RobotType.ToString();
                    break;
                default:
                    StatusBar.Instance.Robot = String.Empty;
                    break;
            }

            StatusBar.Instance.FileSave = !String.IsNullOrEmpty(FileName) ? System.IO.File.GetLastWriteTime(FileName).ToString(CultureInfo.InvariantCulture) : String.Empty;
            StatusBar.Instance.Line = TextArea.Caret.Line.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Column = TextArea.Caret.Column.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Offset = CaretOffset.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            if (_iconBarMargin!=null)
            _iconBarMargin.Dispose();

        }
    }
}
