using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using miRobotEditor.GUI;
using System.Windows.Controls;
using miRobotEditor.ViewModel;
using miRobotEditor.Forms;
using System.Windows;
namespace miRobotEditor.Languages
{
    public enum Typlanguage { None = 0, KUKA = 1, KAWASAKI = 2, ABB = 3, VBA = 4, Fanuc = 5 }

    [Localizable(false)]
    public abstract class AbstractLanguageClass : ViewModelBase
    {

        #region Constructors

        public AbstractLanguageClass()
        {
            Instance = this;
            //RobotMenuItems=GetMenuItems();
        }

        public AbstractLanguageClass(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            var dirExists = dir != null && Directory.Exists(dir);
            var ext = Path.GetExtension(filename);
            var fn = Path.GetFileNameWithoutExtension(filename);

            if (this is KUKA && ext == ".sub")
            {
                SourceName = Path.GetFileName(filename);
            }
            else
                if ((this is KUKA) && ((ext == ".src") || (ext == ".dat")))
                {
                    SourceName = fn + ".src";
                    DataName = fn + ".dat";
                }
                else
                {
                    SourceName = Path.GetFileName(filename);
                    DataName = string.Empty;
                }

            if (dirExists && File.Exists(Path.Combine(dir, SourceName)))
                using (var reader = new StreamReader(Path.Combine(dir, SourceName)))
                    SourceText += reader.ReadToEnd();



            if (dirExists && File.Exists(Path.Combine(dir, DataName)))
                using (var reader = new StreamReader(Path.Combine(dir, DataName)))
                    DataText += reader.ReadToEnd();

            RawText = SourceText + DataText;
            Instance = this;
            RobotMenuItems = GetMenuItems();
        }
        #endregion


        #region Properties
        private DirectoryInfo _rootpath = null;
        public DirectoryInfo RootPath { get { return _rootpath; } set { _rootpath = value; RaisePropertyChanged("RootPath"); } }

        private string _filename = String.Empty;
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; RaisePropertyChanged("Filename"); }
        }

        private MenuItem _robotmenuitems;
        public MenuItem RobotMenuItems
        {
            get
            {
                return _robotmenuitems;
            }
            set { _robotmenuitems = value; RaisePropertyChanged("RobotMenuItems"); }
        }

        internal string DataName { get; private set; }
        internal string SourceName { get; private set; }

        List<string> _rootFiles = new List<string>();

        public static int Progress { get; private set; }

        private ObservableCollection<IVariable> _objectBrowserVariables = new ObservableCollection<IVariable>();
        ReadOnlyObservableCollection<IVariable> _readOnlyBrowserVariables = null;
        public ReadOnlyObservableCollection<IVariable> ObjectBrowserVariable { get { return _readOnlyBrowserVariables ?? new ReadOnlyObservableCollection<IVariable>(_objectBrowserVariables); } }

        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();
        ReadOnlyObservableCollection<MenuItem> __readonlyMenuItems = null;
        public ReadOnlyObservableCollection<MenuItem> MenuItems { get { return __readonlyMenuItems ?? new ReadOnlyObservableCollection<MenuItem>(_menuItems); } }

        List<FileInfo> _files = new List<FileInfo>();
        ReadOnlyCollection<FileInfo> _readOnlyFiles = null;
        public ReadOnlyCollection<FileInfo> Files { get { return _readOnlyFiles ?? new ReadOnlyCollection<FileInfo>(_files); } }

        /// <summary>
        /// Text of _files For searching
        /// </summary>
        internal string RawText { get; set; }
        public static AbstractLanguageClass Instance { get; set; }
        internal string SourceText { get; set; }
        internal string DataText { get; set; }

        public string SnippetPath
        {
            get { return ".\\Editor\\Config _files\\Snippet.xml"; }
        }

        protected string Intellisense
        {
            get { return String.Concat(RobotType.ToString(), "Intellisense.xml"); }
        }

        protected string SnippetFilePath
        {
            get { return String.Concat(RobotType.ToString(), "Snippets.xml"); }
        }

        internal string Filename { get; set; }

        protected string ConfigFilePath
        {
            get { return String.Concat(RobotType.ToString(), "Config.xml"); }
        }

        internal string SyntaxHighlightFilePath
        {
            get { return String.Concat(RobotType.ToString(), "Highlight.xshd"); }
        }

        internal string StyleFilePath
        {
            get { return String.Concat(RobotType.ToString(), "Style.xshd"); }
        }

        public static int FileCount { get; private set; }

        List<IVariable> _allVariables = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlyAllVariables = null;
        public ReadOnlyCollection<IVariable> AllVariables { get { return _readOnlyAllVariables ?? new ReadOnlyCollection<IVariable>(_allVariables); } }

        List<IVariable> _functions = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlyFunctions = null;
        public ReadOnlyCollection<IVariable> Functions { get { return _readOnlyFunctions ?? new ReadOnlyCollection<IVariable>(_functions); } }

         List<IVariable> _fields = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlyFields = null;
        public ReadOnlyCollection<IVariable> Fields { get { return _readOnlyFields ?? new ReadOnlyCollection<IVariable>(_fields); } }

         List<IVariable> _positions = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlypositions = null;
        public ReadOnlyCollection<IVariable> Positions { get { return _readOnlypositions ?? new ReadOnlyCollection<IVariable>(_positions); } }


         List<IVariable> _enums = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlyenums = null;
        public ReadOnlyCollection<IVariable> Enums { get { return _readOnlyenums ?? new ReadOnlyCollection<IVariable>(_enums); } }


         List<IVariable> _structures = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlystructures = null;
        public ReadOnlyCollection<IVariable> Structures { get { return _readOnlystructures ?? new ReadOnlyCollection<IVariable>(_structures); } }

         List<IVariable> _signals = new List<IVariable>();
        ReadOnlyCollection<IVariable> _readOnlysignals = null;
        public ReadOnlyCollection<IVariable> Signals { get { return _readOnlysignals ?? new ReadOnlyCollection<IVariable>(_signals); } }

        #endregion


        #region Abstract
        #region Abstract Members

        public abstract string CommentChar { get; }
        #endregion

        #region Abstract Properties

        public abstract List<string> SearchFilters { get; }

        internal abstract Typlanguage RobotType { get; }
        protected abstract string ShiftRegex { get; }

        public abstract Regex MethodRegex { get; }

        /// <summary>
        /// Regular Expression for finding Fields
        /// <remarks> Used in Editor.FindBookmarks</remarks>
        /// </summary>
        public abstract Regex FieldRegex { get; }

        public abstract Regex EnumRegex { get; }

        public abstract Regex XYZRegex { get; }

        public abstract Regex StructRegex { get; }
        public abstract Regex SignalRegex { get; }



        /// <summary>
        /// Regular Expression for Functions
        /// </summary>
        internal abstract string FunctionItems { get; }

        internal abstract IList<ICompletionData> CodeCompletion { get; }
        internal abstract AbstractFoldingStrategy FoldingStrategy { get; set; }

        #endregion


        #region Abstract Methods
        public abstract DocumentViewModel GetFile(string filename);

        public abstract string ExtractXYZ(string positionstring);
        //TODO Need to figure a way to use multiple extensions
        /// <summary>
        /// Source file extension
        /// </summary>
        internal abstract string SourceFile { get; }

        internal abstract string FoldTitle(FoldingSection section, TextDocument doc);
        #endregion
        #endregion

        private MenuItem GetMenuItems()
        {
            var rd = new ResourceDictionary { Source = new Uri("/miRobotEditor;component/Themes/MenuDictionary.xaml", UriKind.RelativeOrAbsolute) };
            var i = rd[RobotType + "Menu"] as MenuItem ?? new MenuItem();
            return i;
        }

        public static DocumentViewModel GetViewModel(string filepath)
        {
            if (!String.IsNullOrEmpty(filepath))
            {
                var extension = Path.GetExtension(filepath);
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".as":
                        case ".pg":
                            return new DocumentViewModel(filepath, new Kawasaki(filepath));
                        case ".rt":
                        case ".src":
                        case ".dat":
                        case ".sub":
                        case ".kfd":
                            return new KukaViewModel(filepath, new KUKA(filepath));
                        case ".mod":
                        case ".prg":
                            return new DocumentViewModel(filepath, new ABB(filepath));
                        case ".bas":
                            return new DocumentViewModel(filepath, new VBA(filepath));
                        case ".ls":
                            return new DocumentViewModel(filepath, new Fanuc(filepath));
                        default:
                            return new DocumentViewModel(filepath, new LanguageBase(filepath));
                    }
            }
            return new DocumentViewModel(null, new LanguageBase(filepath));
        }

        public static AbstractLanguageClass GetRobotType(string file)
        {
            if (!String.IsNullOrEmpty(file))
            {
                var extension = Path.GetExtension(file);
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".as":
                        case ".pg":
                            return new Kawasaki(file);
                        case ".rt":
                        case ".src":
                        case ".dat":
                        case ".sub":
                        case ".kfd":
                            return new KUKA(file);
                        case ".mod":
                        case ".prg":
                            return new ABB(file);
                        case ".bas":
                            return new VBA(file);
                        case ".ls":
                            return new Fanuc(file);
                        default:
                            return new LanguageBase(file);
                    }
            }
            return new LanguageBase();
        }


        /// <summary>
        /// Strips Comment Character from string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string CommentReplaceString(string text)
        {
            var pattern = String.Format("^([ ]*)([{0}]*)([^\r\n]*)", CommentChar);
            var rgx = new Regex(pattern);
            var m = rgx.Match(text);
            if (m.Success)
            {
                return m.Groups[1] + m.Groups[3].ToString();
            }
            return text;
        }

        public virtual int CommentOffset(string text)
        {
            //TODO Create Result Regex
            var rgx = new Regex(@"(^[\s]+)");
            {
                var m = rgx.Match(text);
                if (m.Success)
                    return m.Groups[1].Length;
                //return m.Groups[1].ToString()+ m.Groups[2].ToString();
            }


            return 0;
        }

        /// <summary>
        /// Trims Line and Then Returns if first Character is a comment Character
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLineCommented(string text)
        {
            return text.Trim().IndexOf(CommentChar, StringComparison.Ordinal).Equals(0);
        }



        #region IDisposable Members



        #endregion

        #region Folding Section


        static bool IsValidFold(string text, string s, string e)
        {
            text = text.Trim();
            var bSP = text.StartsWith(s);
            var bEP = text.StartsWith(e);

            if (!(bSP | bEP)) return false;

            string cAfterString = string.Empty;
            var lookfor = bSP ? s : e;

            //TODO Come Back and fix this
            if (text.Substring(text.IndexOf(lookfor) + lookfor.Length).Length == 0) return true;

            cAfterString = text.Substring(text.IndexOf(lookfor) + lookfor.Length, 1);


            var cc = Convert.ToChar(cAfterString);
            var isLetter = Char.IsLetterOrDigit(cc);

            return (!isLetter);
        }
        public static IEnumerable<LanguageFold> CreateFoldingHelper(ITextSource document, string startFold, string endFold, bool defaultclosed)
        {
            var newFoldings = new List<LanguageFold>();
            var startOffsets = new Stack<int>();
            var doc = (document as TextDocument);
            endFold = endFold.ToLower();
            int err = 0;

            //TODO Instead of Parsing through lines, I may want to search the textrope

            foreach (var dd in doc.Lines)
            {
                var line = doc.GetLineByNumber(dd.LineNumber);
                var text = doc.GetText(line.Offset, line.Length).ToLower();
                var eval = text.TrimStart();

                try
                {

                    if (!IsValidFold(text, startFold, endFold))
                        continue;

                    if (eval.StartsWith(startFold))
                    {
                        startOffsets.Push(line.Offset);
                        continue;
                    }


                    if (eval.StartsWith(endFold) && startOffsets.Count > 0)
                    {
                        // Might Be for EndFolds
                        bool valid;
                        if (endFold == "end")
                        {
                            if (text.Length == endFold.Length)
                                valid = true;
                            else
                            {
                                var ee = text.ToCharArray(endFold.Length, 1);
                                valid = !char.IsLetterOrDigit(ee[0]);
                            }
                        }
                        else
                            valid = true; // Not an End Statement
                        if (valid)
                        {
                            //Add a new folder to the list
                            var s = startOffsets.Pop();

                            var e = line.Offset + text.Length;

                            var str = doc.GetText(s + startFold.Length + 1, line.Offset - s - endFold.Length);


                            var nf = new LanguageFold(s, e, str, startFold, endFold, defaultclosed);
                            newFoldings.Add(nf);
                        }
                    }
                    else
                        err++;

                }
                catch (Exception ex)
                {

                    MessageViewModel.AddError("AbstractLanguageClass.CreateFoldingHelper", ex);
                    continue;
                }

            }

            return newFoldings;
        }

        #endregion

        // ReSharper disable UnusedMember.Local
        private static GroupCollection GetMatchCollection(string text, string matchstring)
        // ReSharper restore UnusedMember.Local
        {

            var r = new Regex(matchstring, RegexOptions.IgnoreCase);
            var m = r.Match(text);
            return m.Success ? m.Groups : null;
        }

        private static Collection<string> GetMatches(string text, string matchstring)
        {
            var result = new Collection<string>();

            var r = new Regex(matchstring, RegexOptions.IgnoreCase);
            var m = r.Match(text);
            while (m.Success)
            {
                result.Add(m.Groups[2].ToString());
                m = m.NextMatch();
            }
            return result;
        }

        protected static Collection<LanguageFold> AddInternalVariables(string text, string regex)
        {
            var Return = new Collection<LanguageFold>();
            var result = GetMatches(text, regex);

            for (var i = 0; i < result.Count - 1; i++)
            {
                Return.Add(new LanguageFold { StartOffset = Convert.ToInt32(result[i]), Name = "UserDefined" });
            }
            return Return;
        }

        protected void Dispose()
        {
            Dispose();
        }

        internal void PositionVariables(string source, string data)
        {
            // Get Positions
        }

        #region Shift Section

        /// <summary>
        /// Shift Program Fuction
        /// <remarks> Uses RegexString variable to get positions and shift</remarks>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public ShiftClass ShiftProgram(IDocument doc, ShiftViewModel shift)
        {
            if (doc is KukaViewModel)
            {
                var d = doc as KukaViewModel;
                var result = new ShiftClass { Source = ShiftProgram(d.Source, shift) };

                if (!(string.IsNullOrEmpty(d.Data.Text)))
                    result.Data = ShiftProgram(d.Data, shift);

                return result;
            }
            else
            {
                var result = new ShiftClass { Source = ShiftProgram(doc.TextBox, shift) };
                return result;
            }
        }

        private string ShiftProgram(Editor doc, ShiftViewModel shift)
        {
            var splash = new FrmSplashScreen();
            //TODO: Need to put all of this into a thread.
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shiftvalX = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.X);
            var shiftvalY = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Y);
            var shiftvalZ = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Z);


            var r = new Regex(ShiftRegex, RegexOptions.IgnoreCase);

            var matches = r.Matches(doc.Text);
            var count = matches.Count;
            splash.Maximum = count;

            var splashthread = new Thread(miRobotEditor.Forms.SplashScreen.ShowSplashScreen) { IsBackground = true };

            splashthread.Start();

            // Spin for a while waiting for the started thread to become
            // alive:
            while (!splashthread.IsAlive)
            {
            }

            // get divisible value for progress update
            Double prog = 0;

            double increment = (count > 0) ? 100 / count : count;


            // doc.SuspendLayout();
            foreach (Match m in r.Matches(doc.Text))
            {
                miRobotEditor.Forms.SplashScreen.UpdateProgress((int)prog);
                miRobotEditor.Forms.SplashScreen.UpdateStatusTextWithStatus(string.Format(Properties.Resources.ShiftingProgram, ((int)prog).ToString(CultureInfo.InvariantCulture)), TypeOfMessage.Success);

                prog = prog + increment;

                // ReSharper disable UnusedVariable
                var xf = Convert.ToDouble(m.Groups[3].Value) + shiftvalX;

                var yf = Convert.ToDouble(m.Groups[4].Value) + shiftvalY;
                var zf = Convert.ToDouble(m.Groups[5].Value) + shiftvalZ;
                // ReSharper restore UnusedVariable
                switch (RobotType)
                {
                    case Typlanguage.KUKA:
                        doc.ReplaceAll();
                        break;
                    case Typlanguage.ABB:
                        doc.ReplaceAll();
                        break;
                }
            }
            //           doc.ResumeLayout();

            splash.UpdateStatusTextWithStatus("Shift Operation Complete", TypeOfMessage.Success);
            Thread.Sleep(500);

            splashthread.Abort();
            splashthread.Join();
            splash.CloseSplashScreen();

            stopwatch.Stop();
            Console.WriteLine("{0}ms to parse shift", stopwatch.ElapsedMilliseconds);

            return doc.Text;
        }

        #endregion




        // Try to Find Variables

        #region Automatic ObjectBrowser
        string _rootName = string.Empty;
        string TargetDirectory = "KRC";
        bool _rootFound = false;
        public void GetRootDirectory(string dir)
        {
            //Search Backwards from current point to root directory
            var dd = new DirectoryInfo(dir);

            // Cannot Parse Directory
            if (dd.Name == dd.Root.Name) _rootFound=true;

            try
            {
                while ((!_rootFound) && (dd.Parent.Name != TargetDirectory))
                {
                    GetRootDirectory(dd.Parent.FullName);
                }


                if (_rootFound) return;

                _rootName = dd.Parent.Parent.Name != "C" ? dd.Parent.Parent.FullName : dd.Parent.Parent.Parent.FullName;

                var r = new DirectoryInfo(_rootName);

                var f = r.GetDirectories();


                if (f.Length >= 1)
                {
                    if ((f[0].Name == "C") && (f[1].Name == "KRC"))
                        _rootName = r.FullName;

                    _rootFound = true;

                    GetRootFiles(_rootName);
                    FileCount = Files.Count;
                    
                    GetVariables();
                    this._allVariables.AddRange(this.Functions);
                    this._allVariables.AddRange(this.Fields);
                    this._allVariables.AddRange(this.Positions);

                }






            }
            catch (Exception ex)
            {
            }

            // Need to get further to the root so that i can interrogate system files as well.
        }



        private void GetRootFiles(string dir)
        {
            foreach (var d in Directory.GetDirectories(dir))
            {
                foreach (var f in Directory.GetFiles(d))
                {

                    try
                    {
                        var file = new FileInfo(f);
                        _files.Add(file);
                    }
                    catch { }

                }

                this.GetRootFiles(d);
            }
        }


        BackgroundWorker bw;

        void GetVariables()
        {
            bw = new BackgroundWorker();
            bw.DoWork += backgroundVariableWorker_DoWork;
          
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        
            bw.RunWorkerAsync();

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged("Functions");
            RaisePropertyChanged("Fields");
            RaisePropertyChanged("Files");
            RaisePropertyChanged("Positions");

        }

        void backgroundVariableWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            Progress = e.ProgressPercentage;
        }



        void backgroundVariableWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _functions = new List<IVariable>();
            _fields = new List<IVariable>();
            _positions = new List<IVariable>();

            foreach (var f in Files)
            {

                _functions.AddRange(FindMatches(MethodRegex, Global.ImgMethod, f.FullName));
                _structures.AddRange(FindMatches(StructRegex, Global.ImgStruct, f.FullName));
                _fields.AddRange(FindMatches(FieldRegex, Global.ImgField, f.FullName));
                _signals.AddRange(FindMatches(SignalRegex, Global.ImgSignal, f.FullName));
                _enums.AddRange(FindMatches(EnumRegex, Global.ImgEnum, f.FullName));
                _positions.AddRange(FindMatches(XYZRegex, Global.ImgXyz, f.FullName));
            }

        }


        //TODO Signal Path for KUKARegex currently displays linear motion
        private List<IVariable> FindMatches(Regex matchstring, string imgPath, string filepath)
        {

            List<IVariable> _result = new List<IVariable>();
            try
            {
                var Text = File.ReadAllText(filepath);

                // Dont Include Empty Values
                if (String.IsNullOrEmpty(matchstring.ToString())) return _result;

                var m = matchstring.Match(Text.ToLower());

                while (m.Success)
                {
                    _result.Add(new Variable
                    {
                        Declaration = m.Groups[0].ToString(),
                        Offset = m.Index,
                        Type = m.Groups[1].ToString(),
                        Name = m.Groups[2].ToString(),
                        Value = m.Groups[3].ToString(),
                        Path = filepath,
                        Icon = Utilities.LoadBitmap(imgPath)
                    });
                    m = m.NextMatch();
                }
            }
            catch (Exception ex)
            {
            }

            return _result;
        }
        #endregion

    }



}
    
