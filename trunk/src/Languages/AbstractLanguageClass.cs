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
using miRobotEditor.Enums;
using miRobotEditor.Forms;
using miRobotEditor.GUI;
using System.Windows.Controls;
namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public abstract class AbstractLanguageClass :ViewModelBase
    {
        #region Properties
        private string _filename;

        public string FileName
        {
            get { return _filename; }
            set { _filename = value;RaisePropertyChanged("Filename"); }
        }
    	
    	private MenuItem _robotmenuitems;
    	public MenuItem RobotMenuItems
    	{
    		get
    		{
    			return _robotmenuitems;
    		}
    		set{_robotmenuitems=value;RaisePropertyChanged("RobotMenuItems");}
    	}

    	private MenuItem GetMenuItems()
    	{
    		var rd= new System.Windows.ResourceDictionary
    		            {Source = new Uri("/miRobotEditor;component/Themes/MenuDictionary.xaml", UriKind.RelativeOrAbsolute)};
    	    var i = rd[RobotType + "Menu"] as MenuItem ??
        		        new MenuItem();

    	    return i;
    	}

        private Editor _sourcedocument;
        public Editor SourceDocument{get { return _sourcedocument; }set{_sourcedocument = value;RaisePropertyChanged("SourceDocument");}}

        private Editor _datadocument;
        public Editor DataDocument{get { return _datadocument; }set{_datadocument = value;RaisePropertyChanged("DataDocument");}}



        private List<IVariable> _positions;
        public List<IVariable> Positions{get { return _positions; }set { _positions = value;RaisePropertyChanged("Positions");}}

       	public static AbstractLanguageClass Instance { get; set; }

        #endregion

        public abstract FileModel GetFile(string filename);

        public ObservableCollection<VariableItem> Functions;
        public ObservableCollection<VariableItem> GetMatches(Regex matchstring, string file, BitmapImage image, string type)
        {

            var result = new ObservableCollection<VariableItem>();
            // Dont Include Empty Values
            if (String.IsNullOrEmpty(matchstring.ToString())) return null;

            var m = matchstring.Match(file.ToLower());
            while (m.Success)
            {
                var item = new VariableItem {Icon = image, Location = file, Type = type};
                result.Add(item);
                m = m.NextMatch();

            }
            return result;
        }

        public IList<ICompletionData> CompletionList (string currentWord,IList<ICompletionData> data )
        {
            try
            {
                foreach (var t1 in DummyDoc.Instance.TextBox.SyntaxHighlighting.MainRuleSet.Rules)
                {
                    var parseString = t1.Regex.ToString();

                    var start = parseString.IndexOf(">", StringComparison.Ordinal) + 1;
                    var end = parseString.LastIndexOf(")", StringComparison.Ordinal);
                    parseString = parseString.Substring(start, end - start);

                    var spl = parseString.Split('|');
                    foreach (var t in spl)
                    {
                        if (String.IsNullOrEmpty(t)) continue;

                        var item = new CodeCompletion(t.Replace("\\b", ""));

                        if (!data.Contains(item) && char.IsLetter(item.Text, 0))
                            data.Add(item);
                    }
                    //TODO Get Info from ObjectBrowser and Add to List
                    foreach (var va in ObjectBrowserViewModel.Instance.AllVariables)
                    {
                        if ((va.Type != "def") && (va.Type != "deffct"))
                        {
                            var item = new CodeCompletion(va.Name);
                            data.Add(item);
                        }

                    }
                }
            }
            catch
            {
            }


            return data;
        }

        public static AbstractLanguageClass GetRobotType( string file)
        {
            if (!String.IsNullOrEmpty(file))
            {
                var extension = Path.GetExtension(file);
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".as":
                        case ".pg":
                            return  new Kawasaki(file);
                        case ".rt":
                        case ".src":
                        case ".dat":
                        case ".sub":
                            return  new KUKA(file);
                        case ".mod":
                        case ".prg":
                            return  new ABB(file);
                        case ".bas":
                            return  new VBA(file);
                        case ".ls":
                            return new Fanuc(file);
                        default:
                            return new LanguageBase(file);
                    }
            }
            return new LanguageBase();
        }

        #region Constructors
        internal string DataName{get; private set;}
        internal string SourceName { get; private set; }
        protected AbstractLanguageClass()
        {
        	Instance=this;
        	RobotMenuItems=GetMenuItems();
        }

        protected AbstractLanguageClass(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            var dirExists = dir != null && Directory.Exists(dir);
            var ext = Path.GetExtension(filename);
            var fn = Path.GetFileNameWithoutExtension(filename);
            //TODO Need to find way to make sps to work.


            if ((this is KUKA) && ((ext == ".src")||(ext==".dat")))
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
					Instance=this;        
					RobotMenuItems=GetMenuItems();					
        }
        #endregion

        #region Properties

        /// <summary>
        /// Text of _files For searching
        /// </summary>
        internal string RawText { get; set; }

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

        public abstract List<string> SearchFilters { get; }

        public List<string> Methods { get; set; }
        public List<string> Fields { get; set; }

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

        #endregion

        #region Abstract Properties

        internal abstract Typlanguage RobotType { get; }
        protected abstract string ShiftRegex { get; }

        /// <summary>
        /// Regular Expression for Functions
        /// </summary>
        internal abstract string FunctionItems { get; }

        internal abstract IList<ICompletionData> CodeCompletion { get; }
        internal abstract AbstractFoldingStrategy FoldingStrategy { get; set; }

        internal abstract Color FocusedColor { get; }
        internal abstract Color UnfocusedColor { get; }

        #endregion

        public abstract string CommentChar {get;}
  
        /// <summary>
        /// Strips Comment Character from string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string CommentReplaceString(string text)
        {
        	var pattern = String.Format("^([ ]*)([{0}]*)([^\r\n]*)",CommentChar);
			var rgx = new Regex(pattern);		 
        	var m = rgx.Match(text);
			if (m.Success)
			{
				return  m.Groups[1] + m.Groups[3].ToString();
			}
			return text;			
        }
        
        public virtual int CommentOffset(string text)
        {
        //TODO Create Result Regex
        	var rgx=new Regex(@"(^[\s]+)");
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

        //TODO Need to figure a way to use multiple extensions
        /// <summary>
        /// Source file extension
        /// </summary>
        internal abstract string SourceFile { get; }

        #region IDisposable Members

        

        #endregion

        #region Folding Section

        public static IEnumerable<LanguageFold> CreateFoldingHelper(ITextSource document, string startFold, string endFold, bool defaultclosed)
        {
            var newFoldings = new List<LanguageFold>();
            var startOffsets = new Stack<int>();
            var doc = (document as TextDocument);

            for (var i = 0; i < ((TextDocument) document).Lines.Count - 1; i++)
            {
                var textDocument = document as TextDocument;
                if (textDocument != null)
                {
                    var seg = textDocument.Lines[i];
                    int offs, end = document.TextLength;
                    char c;
                    for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                    {
                    }

                    if (offs == end)
                        break;

                    var spacecount = offs - seg.Offset;

                    //now offs points to the non-whitespace char on the line
                    if (document.GetCharAt(offs) != ' ')
                    {
                        var text = document.GetText(offs, seg.Length - spacecount);

                        if (text.ToLower().StartsWith(startFold))
                            startOffsets.Push(offs);
                        //                     startOffsets.Push(offs + text.Length);
                        if (text.ToLower().StartsWith(endFold) && startOffsets.Count > 0)
                        {
                            //FIXME this is Wrong!!!!!!
                            // Might Be for EndFolds
                            bool valid;
                            if (endFold.ToLower() == "end")
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
                                var start = startOffsets.Pop();
                                var nf = new LanguageFold(start, offs + text.Length,doc.GetText(start + startFold.Length + 1,offs - start - endFold.Length))
                                             {
                                                 Name = String.Format("{0}æ{1}", startFold, endFold),
                                                 DefaultClosed = defaultclosed
                                             };
                                newFoldings.Add(nf);
                            }
                        }
                    }
                }
            }

            return newFoldings;
        }

        #endregion

        internal abstract string FoldTitle(FoldingSection section, TextDocument doc);

        //     public static Stream GetIntellisense(string name)
        //     {
        //         var filestream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        //         return filestream ?? null;
        //     }

        public abstract Regex MethodRegex{get;}
        
        /// <summary>
        /// Regular Expression for finding Fields
        /// <remarks> Used in Editor.FindBookmarks</remarks>
        /// </summary>
        public abstract Regex FieldRegex{get;}

        public abstract Regex EnumRegex{get;}

        public abstract Regex XYZRegex { get; }

        public abstract Regex StructRegex{get;}
        public abstract Regex SignalRegex { get; }


        public abstract string ExtractXYZ(string positionstring);

//        {
//            var p = new PositionBase(positionstring);
//            return p.ExtractFromMatch();
//        }

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
                Return.Add(new LanguageFold {StartOffset = Convert.ToInt32(result[i]), Name = "UserDefined"});
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
        public ShiftClass ShiftProgram(DummyDoc doc, FrmShift shift)
        {
            var result = new ShiftClass {Source = ShiftProgram(doc.Source, shift)};

            if (!(string.IsNullOrEmpty(doc.Data.Text)))
                result.Data = ShiftProgram(doc.Data, shift);

            return result;
        }

        private string ShiftProgram(Editor doc, FrmShift shift)
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

            var splashthread = new Thread(SplashScreen.ShowSplashScreen) {IsBackground = true};

            splashthread.Start();

            // Spin for a while waiting for the started thread to become
            // alive:
            while (!splashthread.IsAlive)
            {
            }

            // get divisible value for progress update
            Double prog = 0;

            double increment = (count > 0) ? 100/count : count;

            
            // doc.SuspendLayout();
            foreach (Match m in r.Matches(doc.Text))
            {
                SplashScreen.UpdateProgress((int) prog);
                SplashScreen.UpdateStatusTextWithStatus(
                    string.Format("Shifting Program. Progress:= {0}",
                                  ((int) prog).ToString(CultureInfo.InvariantCulture)), TypeOfMessage.Success);
               
                prog = prog + increment;

// ReSharper disable UnusedVariable
                var xf = Convert.ToDouble(m.Groups[3].Value) + shiftvalX;

                var yf = Convert.ToDouble(m.Groups[4].Value) + shiftvalY;
                var zf = Convert.ToDouble(m.Groups[5].Value) + shiftvalZ;
// ReSharper restore UnusedVariable
                switch (DummyDoc.Instance.FileLanguage.RobotType)
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


    }
}