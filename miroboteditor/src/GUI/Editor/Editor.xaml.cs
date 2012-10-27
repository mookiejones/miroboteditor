using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.Editing;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using miRobotEditor.Robot;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Snippets;

namespace miRobotEditor.Controls
{
    public delegate void UpdateFunctionEventHandler(object sender, FunctionEventArgs e);

    
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        public event EventHandler ReloadFile;

        #region Static Members

        public static readonly RoutedCommand ToggleComment = new RoutedCommand();
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
         
        
        private IconBarManager iconBarManager;
        private IconBarMargin iconBarMargin;
       

        /// <summary>
        /// Records last key For Multiple Key presses
        /// </summary>
        private KeyEventArgs lastKeyUpArgs = null;

        /// <summary>
        //  For Zooming When Scrolling Text
        /// </summary>
        private const int LOGIC_LIST_FONT_SIZE_MAX = 50;

        /// <summary>
        ///  For Zooming When Scrolling Text
        /// </summary>
        private const int LOGIC_LIST_FONT_SIZE_MIN = 10;

        #endregion

        #region Public Events

        public event UpdateFunctionEventHandler UpdateFunctions;

        #endregion

        #region Properties

        public FileInfo File { get; private set; }


        private string Filename
        {
            get { return (File != null) ? File.FullName : String.Empty; }
        }



        #endregion

        #region Constructor

        public Editor()
        {
            InitializeComponent();

            RegisterSyntaxHighlighting();

            iconBarMargin = new IconBarMargin(iconBarManager = new IconBarManager());
            this.TextArea.LeftMargins.Insert(0, iconBarMargin);

             this.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(this.TextArea));

            AddBindings();

            TextArea.TextEntered += TextArea_TextEntered;          
            TextArea.TextEntering += TextArea_TextEntering;
            this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            DataContext = this;
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            var s = sender as ICSharpCode.AvalonEdit.Editing.Caret;
            StatusBar.Instance.Line = s.Line.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Column =  s.Column.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Offset = CaretOffset.ToString(CultureInfo.InvariantCulture);
        }

     

        #endregion

        #region Overrides

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // call base handler
            base.OnKeyUp(e);
            if (lastKeyUpArgs == null)
            {
                lastKeyUpArgs = e;
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
            lastKeyUpArgs = e;


        }

        #endregion

        /// <summary>
        /// Loads all of syntax Highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
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
            Register("KUKA", KUKA.Ext);
            Register("KAWASAKI", Kawasaki.EXT);
            Register("Fanuc", Fanuc.EXT);
            Register("ABB", ABB.EXT);         
        }

        private void AddBookMark(int lineNumber, string type, string imgpath)
        {
            BitmapImage bitmap = Utilities.LoadBitmap(imgpath);
            BookmarkImage bmi = new BookmarkImage(bitmap);
            iconBarManager.Bookmarks.Add(new ClassMemberBookmark(lineNumber, bmi));
        }

        private void FindMatches(Regex matchstring, string imgPath)
        {
            // Dont Include Empty Values
            if (String.IsNullOrEmpty(matchstring.ToString())) return;

            Match m = matchstring.Match(Text.ToLower());
            while (m.Success)
            {
                var d = Document.GetLineByOffset(m.Index);
                AddBookMark(d.LineNumber, m.Groups[0].ToString(), imgPath);
                m = m.NextMatch();

            }
        }

        /// <summary>
        /// Find info for bookmark
        /// <remarks>Need to make sure Correct Priority is set. Whatever is set first will overwrite anything after</remarks>
        /// </summary>
        private void FindBookmarkMembers()
        {
            // Return if Robot doesnt exist yet
            if (DummyDoc.ActiveEditor.Robot == null) return;
            iconBarManager.Bookmarks.Clear();

            FindMatches(DummyDoc.ActiveEditor.Robot.MethodRegex, Global.imgMethod);

            FindMatches(DummyDoc.ActiveEditor.Robot.StructRegex, Global.imgStruct);

            FindMatches(DummyDoc.ActiveEditor.Robot.FieldRegex, Global.imgField);

            FindMatches(DummyDoc.ActiveEditor.Robot.SignalRegex, Global.imgSignal);

            FindMatches(DummyDoc.ActiveEditor.Robot.EnumRegex, Global.imgEnum);

            FindMatches(DummyDoc.ActiveEditor.Robot.XYZRegex, Global.imgXYZ);


            if (DummyDoc.ActiveEditor.Robot is KUKA)
                FindMatches(new Regex("DECL [a-zA-Z0-9_$]+", (RegexOptions) 3), Global.imgValue);

            //TODO Create Icon for XYZ      		
            //TODO Create Icon For SignalRegex
            //      		FindMatches(Robot.XYZRegex,
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Bindings

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
            commandBindings.Add(new CommandBinding(ToggleComment, ExecuteToggleComment, CanToggleComment));
            commandBindings.Add(new CommandBinding(IncreaseIndent, ExecuteIncreaseIndent, CanIncreaseIndent));
            commandBindings.Add(new CommandBinding(DecreaseIndent, ExecuteDecreaseIndent, CanDecreaseIndent));
            var inputBindings = TextArea.InputBindings;
            inputBindings.Add(new KeyBinding(ApplicationCommands.Find, Key.F, ModifierKeys.Control));
            inputBindings.Add(new KeyBinding(ApplicationCommands.Replace, Key.R, ModifierKeys.Control));
            inputBindings.Add(new KeyBinding(ToggleComment, Key.C, (int) ModifierKeys.Shift + ModifierKeys.Control));
        }

        private void CanDecreaseIndent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.TextArea.IndentationStrategy != null;
        }

        private void CanIncreaseIndent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.TextArea.IndentationStrategy != null;
        }

        private void IndentLine(int num)
        {
          

        }
        private void ExecuteDecreaseIndent(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var start = Document.GetLineByOffset(SelectionStart);
                var end = Document.GetLineByOffset(SelectionStart + SelectionLength);
                var Positions = 0;
                using (Document.RunUpdate())
                {
                    for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                    {
                        var currentline = GetLine(line.LineNumber);
                        var rgx = new Regex(@"(^[\s]+)");

                        Match m = rgx.Match(currentline);
                        if (m.Success)
                            Positions = m.Groups[1].Length;

                        Positions = Positions > 1 ? Positions - 1 : Positions;

                        if (Positions > 1)
                            Document.Replace(line.Offset, currentline.Length, currentline.Substring(1));
                    }
                }
            }
            catch
            {
            }

        }

        private void ExecuteIncreaseIndent(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var start = Document.GetLineByOffset(SelectionStart);
                var end = Document.GetLineByOffset(SelectionStart + SelectionLength);
                var Positions = 0;
                using (Document.RunUpdate())
                {
                    for (var line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                    {
                        var currentline = GetLine(line.LineNumber);
                        var rgx = new Regex(@"(^[\s]+)");

                        Match m = rgx.Match(currentline);
                        if (m.Success)
                            Positions = m.Groups[1].Length;
                            Document.Insert(line.Offset + Positions , " ");
                    }
                }
            }
            catch
            {
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
            if (DummyDoc.ActiveEditor.Robot == null) return;

            // Get Comment to insert
            DocumentLine start = Document.GetLineByOffset(SelectionStart);
            DocumentLine end = Document.GetLineByOffset(SelectionStart + SelectionLength);

            using (Document.RunUpdate())
            {
                for (DocumentLine line = start; line.LineNumber < end.LineNumber + 1; line = line.NextLine)
                {
                    var currentline = GetLine(line.LineNumber);
                    DummyDoc.ActiveEditor.Robot.IsLineCommented(currentline);

                    // Had to put in comment offset for Fanuc 
                    if (!DummyDoc.ActiveEditor.Robot.IsLineCommented(currentline))
                        Document.Insert(DummyDoc.ActiveEditor.Robot.CommentOffset(currentline) + line.Offset, DummyDoc.ActiveEditor.Robot.CommentChar);
                    else
                    {
                        string replacestring = DummyDoc.ActiveEditor.Robot.CommentReplaceString(currentline);
                        Document.Replace(line.Offset, currentline.Length, replacestring);
                    }
                }
            }
        }

        private void CanToggleComment(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DummyDoc.ActiveEditor.Robot != null)
                e.CanExecute = !String.IsNullOrEmpty(DummyDoc.ActiveEditor.Robot.CommentChar);
        }

        private void CanReplace(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (File != null)
            {
                e.CanExecute = (File.Exists && IsModified);
            }
            else
            {
                e.CanExecute = false;
            }
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
            var f = new GotoDialog();
            var v = f.ShowDialog();

            if (v == true)
                Console.WriteLine(f.SelectedLine);

        }

        private void ExecuteSaveAs(object target, ExecutedRoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.SaveFileDialog {Title = "Save As", Filter = "All _files(*.*)|*.*"};

            if (File != null)
            {
                ofd.FileName = this.File.FullName;
                ofd.Filter += String.Format("|Current Type (*{0})|*{0}", File.Extension);
                ofd.FilterIndex = 2;
                ofd.DefaultExt = File.Extension;
            }

            ofd.ShowDialog();

            if (!String.IsNullOrEmpty(ofd.FileName))
            {
                var filename = ofd.FileName;
                File = new FileInfo(filename);
                Save(filename);
                RecentFileList.Instance.InsertFile(filename);
                OutputMessages.Messages.Add(new OutputWindowMessage {Title= "_file Saved",Description=filename,Icon = null});
            }
        }

        private void ExecuteSave(object target, ExecutedRoutedEventArgs e)
        {
            _watcher.EnableRaisingEvents = false;
                Save(Filename);

            StatusBar.Instance.FileSave = System.IO.File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture);
            _watcher.EnableRaisingEvents = true;
              // Suspend the calling thread until the file has been deleted. 
            }       
        #endregion
    
        protected override void OnOptionChanged(PropertyChangedEventArgs e)
        {
            var options = this.Options as TextEditorOptions;
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
            File = new FileInfo(filename);
            base.Load(filename);
            SetWatcher();           
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(filename));
        }

        public void UpdateVisualText()
        {
            Text_Changed(null, null);
        }

        private void Text_Changed(object sender, EventArgs e)
        {           
            if (IsModified)
            RaiseUpdate(null,new DependencyPropertyChangedEventArgs());
        }

        #region Code Completion

        private CompletionWindow completionWindow;

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (DummyDoc.ActiveEditor.Robot == null | DummyDoc.ActiveEditor.Robot is RobotBase) return;
            var currentword = FindWord();
            if (String.IsNullOrEmpty(currentword)) return;
            completionWindow = new CompletionWindow(TextArea);

            DummyDoc.ActiveEditor.Robot.CompletionList(currentword, completionWindow.CompletionList.CompletionData);
            completionWindow.Closed += delegate { completionWindow = null; };
            completionWindow.CloseWhenCaretAtBeginning = true;
            completionWindow.CompletionList.SelectItem(currentword);
            if (completionWindow.CompletionList.SelectedItem != null)
            completionWindow.Show();
            
        }


        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        #endregion

        #region Search Replace Section

        public void ReplaceAll()
        {
            var r = FindAndReplaceForm.Instance.RegexPattern;
            Match m = r.Match(Text);
            while (m.Success)
            {
                Document.GetLineByOffset(m.Index);
                r.Replace(FindAndReplaceForm.Instance.LookFor, FindAndReplaceForm.Instance.ReplaceWith, m.Index);
                m = m.NextMatch();
            }
            }
        


       
        public void ReplaceText()
        {
            FindText();
            
            SelectedText = SelectedText.Replace(SelectedText, FindAndReplaceForm.Instance.ReplaceWith);
        }

        public void FindText()
        {

            int nIndex = Text.IndexOf(FindAndReplaceForm.Instance.LookFor, CaretOffset, StringComparison.Ordinal);
            if (nIndex > -1)
            {
                DocumentLine d = Document.GetLineByOffset(nIndex);
                ScrollTo(d.LineNumber, 0);
                SelectionStart = nIndex;
                SelectionLength = FindAndReplaceForm.Instance.LookFor.Length;
            }
            else
            {
                FindAndReplaceForm.Instance.SearchResult = "No Results Found, Starting Search from Beginning";
                CaretOffset=0;
            }

        }

        /// <summary>
        /// Select Text
        /// <remarks>Used with Function window</remarks>
        /// </summary>
        /// <param name="text"></param>
        public void SelectText(string text)
        {
            if (text == null) throw new ArgumentNullException("text");

            var d = DummyDoc.ActiveEditor.TextBox.Document.GetLineByOffset(Text.IndexOf(text, 0, StringComparison.Ordinal));
            ScrollToLine(d.LineNumber);

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

        #region goto Dialog

        public void ShowGoToDialog()
        {
            var go = new GotoDialog(this);
            bool? result = go.ShowDialog();

            if (result == true)
                ScrollToLine(go.SelectedLine);
        }

        #endregion

        private void RaiseUpdate(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                if (UpdateFunctions != null)
                    UpdateFunctions(this, new FunctionEventArgs(this.Text));
                if (DummyDoc.ActiveEditor.Robot is RobotBase | DummyDoc.ActiveEditor.Robot == null)
                    return;
                FindBookmarkMembers();
                UpdateFolds();             
            }
            
     
        }

        private void EditorPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                Console.WriteLine(e.Delta.ToString());
                if (e.Delta <= 0 || !(FontSize < LOGIC_LIST_FONT_SIZE_MAX))
                {
                    FontSize += 1;
                }
                else if ((e.Delta > 0) && (FontSize > LOGIC_LIST_FONT_SIZE_MIN))
                {
                    FontSize -= 1;
                }
                e.Handled = true;
            }
        }




        #region Folding Section

        private FoldingManager _foldingManager;
        private AbstractFoldingStrategy _foldingStrategy;

        private void UpdateFolds()
        {
            var foldingEnabled = (Options as TextEditorOptions).EnableFolding;

            if (File == null) return;
            if (SyntaxHighlighting == null)
                _foldingStrategy = null;

            // Get XML Folding
            if (File.Extension == ".xml")
                _foldingStrategy = new XmlFoldingStrategy();
            else
                if (DummyDoc.ActiveEditor.Robot != null)
                    _foldingStrategy = DummyDoc.ActiveEditor.Robot.FoldingStrategy;
            
           if (_foldingStrategy != null&& foldingEnabled)
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
            if (File.Extension == ".xml") return;

            foreach (var section in _foldingManager.AllFoldings)
            {
                section.Title = DummyDoc.ActiveEditor.Robot.FoldTitle(section, Document);
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
            char[] Terminators = { ' ', '=', '(', ')', '[', ']', '<', '>' ,'\r','\n'};


            // Are there any terminators in the line?
            var any = line.IndexOfAny(Terminators);



            int end = line.IndexOfAny(Terminators, TextArea.Caret.Column-1);
            if (end > -1)
                search = (line.Substring(0, end));

            int start = search.LastIndexOfAny(Terminators) + 1;

            if (start > -1)
                search = search.Substring(start).Trim();

            Console.WriteLine(search);
           
            return search;
        }

        private string FindWord(string Line, int column)
        {
            if (Line.Length < column) return string.Empty;


            string search = Line;
            char[] Terminators = {' ', '=', '(', ')', '[', ']', '<', '>'};


            // Are there any terminators in the line?
            var any = Line.IndexOfAny(Terminators);



            int end = Line.IndexOfAny(Terminators, column);

            if (end > -1)
                search = (Line.Substring(0, end));

            int start = search.LastIndexOfAny(Terminators) + 1;

            if (start > -1)
                search = search.Substring(start).Trim();

            Console.WriteLine(search);

            return search;
        }


        private string GetCurrentWord(ICSharpCode.AvalonEdit.TextViewPosition? pos)
        {
            var result = string.Empty;
            if (pos != null)
            {
                var currentLine = GetLine(pos.Value.Line).Trim();

                if (!String.IsNullOrEmpty(currentLine))
                    result = FindWord(currentLine, pos.Value.Column);
            }
            return result;
        }

        private void Mouse_Hover(object sender, MouseEventArgs e)
        {
            if (_foldingManager == null) return;

            //UpdateFolds();
            ICSharpCode.AvalonEdit.TextViewPosition? pos = GetPositionFromPoint(e.GetPosition(this));

            if (pos == null) return;
            var currentword = GetCurrentWord(pos);
            //TODO _variables
            //    toolTip.PlacementTarget = this;
            //    // required for property inheritance 
            //    toolTip.Content = wordhover; 
            //    pos.ToString();
            //    toolTip.IsOpen = true;
            //    e.Handled = true;



            // Is Current Line a Variable?
            // ToolTip t = Robot.variables.VariableToolTip(GetLine(pos.Value.Line));
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

        private RobotFold GetCurrentFold(MouseEventArgs e)
        {
            ICSharpCode.AvalonEdit.TextViewPosition? pos = GetPositionFromPoint(e.GetPosition(this));
            if (pos != null)
            {
                var Fold = _foldingManager.GetFoldingsAt(Document.GetOffset(pos.Value.Line, pos.Value.Column));
                if (Fold.Count > 0)
                {
                    return Fold[0].Tag as RobotFold;
                }
            }
            return null;
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
            e.CanExecute = _foldingManager != null;
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
            public NewFoldingDefinition(int start, int end) : base(start, end)
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

        private FileSystemWatcher _watcher = null;

        private void SetWatcher()
        {
            if (File.DirectoryName == null) return;
                        _watcher = new FileSystemWatcher(File.DirectoryName, File.Name);
                        _watcher.EnableRaisingEvents = true;
                        _watcher.NotifyFilter =NotifyFilters.LastWrite;
                        _watcher.IncludeSubdirectories = true;
                        _watcher.Changed += _watcher_Changed;
                        _watcher.Error += _watcher_Error;
                        _watcher.Renamed += _watcher_Renamed;

                        if (_watcher != null) _watcher.EnableRaisingEvents = true;
        }


        /// <summary>
        /// Used to prevent the bug of FileSystemWatcher sending multiple triggers
        /// </summary>
        DateTime lastRead = DateTime.MinValue;

        /// <summary>
        /// Trigger On File Being Saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {

         

            DateTime lastWriteTime = System.IO.File.GetLastWriteTime(e.FullPath);

            if (lastWriteTime != lastRead)
            {
                string message =
                    String.Format("{0}\r\n has been changed.\r\n\r\nWould you like to reload the file?", e.FullPath);
                var result = MessageBox.Show(message, "File has Changed, Reload File?", MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    RaiseReload();
                }
                lastRead = lastWriteTime;
            }

            // Write To Status Bar
            StatusBar.Instance.FileSave = lastWriteTime.ToString(CultureInfo.InvariantCulture);
        }

        public void Reload()
        {
                base.Load(File.FullName);           
        }

        void RaiseReload()
        {
            if (ReloadFile != null)
                ReloadFile(this, null);
        }
        void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            OutputMessages.Add("FileWatcher","File has Been Renamed but Nothing has been Written to handle it",null);
        }

        void _watcher_Error(object sender, ErrorEventArgs e)
        {
            OutputMessages.Add("FileWatcher","File Error Detected",null);
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
            snippet.Insert(this.TextArea);


        }

        private void TextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (File!=null)
            StatusBar.Instance.Name = File.FullName;

            switch (DummyDoc.ActiveEditor.Robot.RobotType)
            {
                case TYPROBOT.Fanuc:
                case TYPROBOT.ABB:
                case TYPROBOT.KUKA:
                case TYPROBOT.KAWASAKI:
                    StatusBar.Instance.Robot = DummyDoc.ActiveEditor.Robot.RobotType.ToString();
                    break;
                default:
                    StatusBar.Instance.Robot = String.Empty;
                    break;
            }
            StatusBar.Instance.FileSave = !String.IsNullOrEmpty(Filename) ? System.IO.File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture) : String.Empty;
            StatusBar.Instance.Line = TextArea.Caret.Line.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Column = TextArea.Caret.Column.ToString(CultureInfo.InvariantCulture);
            StatusBar.Instance.Offset = CaretOffset.ToString(CultureInfo.InvariantCulture);
        }
    }
}
