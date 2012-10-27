using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Classes;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Properties;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using miRobotEditor.Snippets;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class KUKA : AbstractLanguageClass
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">Filename for opening</param>
        public KUKA(System.IO.FileInfo file):base(file)
        {         
            FoldingStrategy = new RegionFoldingStrategy();
        }


        #region Private Members

        readonly FileInfo fi = new FileInfo();

        #endregion


        internal override Enums.TYPLANGUAGE RobotType { get { return Enums.TYPLANGUAGE.KUKA; } }

       public KUKA()
        {
            FoldingStrategy = new RegionFoldingStrategy();
          
        }

        protected override string ShiftRegex { get { return @"((E6POS [\w]*={)X\s([\d.-]*)\s*,*Y\s*([-.\d]*)\s*,Z\s*([-\d.]*))";}}

        public FileInfo GetFileInfo(string text)
        {
            return fi.GetFileInfo(text);
        }

        #region Properties
        public Forms.frmKUKAIOPoints IO
        {
            get;
            set;
        }
        #endregion

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

      
        /// <summary>
        /// Destructor
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose(true);
            }
        }

        #region KUKA_IO Form
        public Forms.frmKUKAIOPoints GetIO(string filename)
        {
            IO = new Forms.frmKUKAIOPoints(filename);
            return IO;
        }
        #endregion


        /*        public new bool ContextPromptBeforeOpen(ILexemLine lexemLine)
                {
                    // get Lexem
                    Lexem = LanguageBase.GetLexem(lexemLine);
                    return IsLexemPrompt();
                }
         */
        internal override Collection<string> FunctionItems
        {
            get { return new Collection<string> { @"((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\w\s]*)\(([\w\]\s:_\[,]*)\))" }; }
        }

        #region "_file Interface Info"

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)")]
        public static string SystemFileName()
        {
            using (var ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Filter = Resources.KUKA_SystemFileName_KUKA_VxWorks_File__vxWorks_rt_VxWorks_Debug__vxWorks_rt_vxWorks_debug;
                ofd.InitialDirectory = @"C:\krc\bin\";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return string.Empty;
        }

        public static string GetSystemFunctions
        {
            get
            {
                return FunctionGenerator.GetSystemFunctions();
            }
        }

        #endregion

        public static List<string> Ext
        {
        	get{
        		return new List<string> { ".dat", ".src", ".ini", ".sub", ".zip", ".kfd" };
        	}
        }
       
        ///
        /// Sets ComboBox Filter Items for searching
        /// <returns></returns>
        public override List<string> SearchFilters
        {
            get
            {
                return Ext;
            }
        }

        public string Comment {get;set;}

        #region Code Completion Section

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                var codeCompletionList = new List<ICompletionData> {new CodeCompletion("Item1")};
                return codeCompletionList;
            }
        }
  
        #endregion

        private static Editor GetPointsFromArray(Editor editor, Collection<string> points)
        {
            if (points == null) throw new ArgumentNullException("points");
            for (var b = 0; b <= (points.Count - 1); b++)
            {
                editor.Text = editor.Text + points[b] + "\r\n";
            }
            return editor;
        }

        private static Collection<string> GetPositionFromFile(int lineNumber, ITextEditorComponent editor)
        {
            var Points = new Collection<string>();
            while (true)
            {
                Points.Add(editor.Document.Lines[lineNumber].ToString());
                /*   if (!editor.Lines[LineNumber].ToUpperInvariant().IndexOf(";ENDFOLD", StringComparison.OrdinalIgnoreCase).Equals(-1))
                   { 
                       return Points;
                   }*/
                lineNumber++;
            }
        }

        public static Editor ReversePath(Editor editor)
        {
            var Points = new Collection<Collection<string>>();
            for ( int i = 0; i <=(editor.Document.Lines.Count - 1);i++)
            {                
                   if ((editor.Document.Lines[i].ToString().ToUpperInvariant().IndexOf(";FOLD LIN", StringComparison.OrdinalIgnoreCase) > -1) | (editor.Document.Lines[i].ToString().ToUpperInvariant().IndexOf(";FOLD PTP", StringComparison.OrdinalIgnoreCase) > -1))
                 {
                     Points.Add(GetPositionFromFile(i, editor));
                 }
            }
            editor.Text = string.Empty;
            for (var b = Points.Count - 1; b >= 0; b--)
            {
                for (var j = 0; j < Points[b].Count; j++)
                {
                    Collection<string> l = Points[b];
                    editor.AppendText(l[j] + "\r\n");
                }
            }

            return editor;
        }

        public static string PositionRegex = @"(E6POS) ([^=]*)=\{([^\}]*)\}";

  // <summary>
  // Adds custom variables located from the current file
  // </summary>
  //     public new System.Collections.ObjectModel.Collection<ConfigLexem> CustomVariables(string text)
  //     {
  //         const string vars = @"(?<!DEF|DEFFCT) (BOOL|REAL|INT|SIGNAL|E6POS|E6AXIS|POS) ([_\w ]*)";
  //         return LanguageBase.AddInternalVariables(text, vars);
  //     }

        internal static class FunctionGenerator
        {
            private static string _functionFile = String.Empty;
            private static string GetSTRUC(string filename)
            {
                return RemoveFromFile(filename, "((?<!_)STRUC [\\w\\s,\\[\\]]*)");
            }

            public static string GetSystemFunctions()
            {

                var sb = new System.Text.StringBuilder();
                using (var frm = new Forms.frmSystemFunctions())
                {

                    frm.ShowDialog();

                    if (frm.DialogResult.HasValue &&frm.DialogResult.Value)
                    {

                       

                       var ofd = new OpenFileDialog();

                        try
                       {
                           ofd.Filter = "KUKA VxWorks _file (vxWorks.rt;VxWorks.Debug;*.*)|vxWorks.rt;vxWorks.debug;*.*";
                           ofd.Title = ("Select file for reading System Functions");
                           ofd.InitialDirectory = "C:\\krc\\bin\\";

                           var result =ofd.ShowDialog();
                           if (result == true)
                           {

                               if (!File.Exists(ofd.FileName)) return null;

                               File.Copy(ofd.FileName, "c:\\Temp.rt", true);
                               _functionFile = "c:\\Temp.rt";
                               if (frm.Structures)
                               {
                                   sb.AppendLine("************************************************");
                                   sb.AppendLine("*** Structures  ******************");
                                   sb.AppendLine("************************************************");
                                   sb.Append(GetSTRUC(_functionFile));
                               }
                               if (frm.Programs)
                               {
                                   sb.AppendLine("************************************************");
                                   sb.AppendLine("*** Programs  ******************");
                                   sb.AppendLine("************************************************");
                                   sb.Append(GetRegex(_functionFile, @"(EXTFCTP|EXTDEF)([\d\w]*)([\[\]\w\d\( :,]*\))"));
                               }
                               if (frm.Functions)
                               {

                                   sb.AppendLine("************************************************");
                                   sb.AppendLine("*** Functions  ******************");
                                   sb.AppendLine("************************************************");
                                   sb.Append(GetRegex(_functionFile, @"(EXTFCTP|EXTDEF)([\d\w]*)([\[\]\w\d\( :,]*\))"));
                               }
                               if (frm.Variables)
                               {
                                   //sb.AppendLine("***********************************************");
                                   //sb.AppendLine("***   _variables  ******************");
                                   //sb.AppendLine("***********************************************");
                                   //sb.Append(getRegex(FunctionFile, @"(?<!EXTFCTP) (INT|BOOL|REAL|SIGNAL) ([\s\$\w]*)"));
                               }

                           }
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine(ex.ToString());
                       }
                    }
                }

                return sb.ToString();

                //(?<!EXTFCTP\s\()(BOOL|INT) ([\w\$,]*)
            }
            private static string RemoveFromFile(string functionfile, string matchString)
            {
                string line;
                var sb = new System.Text.StringBuilder();
                using (var r = new StreamReader(_functionFile))
                {
                    line = r.ReadToEnd();
                    var rgx = new Regex(matchString, RegexOptions.IgnoreCase);
                    MatchCollection matchs = rgx.Matches(line);
                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                            sb.AppendLine(match.Value);
                    }
                }
                var regex = new Regex(matchString);
                string result = regex.Replace(line, String.Empty);

                using (var outfile = new StreamWriter(functionfile))
                {
                    outfile.Write(result);
                }
                return sb.ToString();
            }
            private static string GetRegex(string functionFile, string matchString)
            {
                if (String.IsNullOrEmpty(functionFile)) return null;
                var sb = new System.Text.StringBuilder();
                // Open _file for reading
                using (var r = new StreamReader(functionFile))
                {
                    // Read each line until EOF
                    string line = r.ReadToEnd();

                    var rgx = new Regex(matchString, RegexOptions.IgnoreCase);
                    var matchs = rgx.Matches(line);
                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                            sb.AppendLine(match.Value);
                    }
                }

                return sb.ToString();
            }
        }
        public sealed class FileInfo
        {
            private const string COMMENT = "&COMMENT";
            private const string VERSION = "&REL";
            private const string PARAM = "&PARAM";
            private const string ACCESS = "&ACCESS";
            private const string USER = "&USER";           
            private string access = String.Empty;

            internal string user = String.Empty;
            public string Comment { get; private set; }           
            public string Version { get; private set; }
            public bool Visible { get; private set; }          
            private string _text = String.Empty;

            public FileInfo(string text)
            {
                _text = text;
            }
            public FileInfo()
            {
            }
            public FileInfo GetFileInfo()
            {
                access = getinfo(ACCESS);
                Comment = getinfo(COMMENT);
                Version = getinfo(VERSION);
                getinfo(PARAM);
                user = getinfo(USER);
                Visible = access.IndexOf("V", StringComparison.Ordinal) > -1;
                return this;
            }

            public FileInfo GetFileInfo(string text)
            {
                _text = text;
                return GetFileInfo();
            }
            private string getinfo(string Lookfor)
            {
                int length = Lookfor.Length;
                int found = _text.IndexOf(Lookfor, StringComparison.Ordinal);
                if (found > -1)
                    return _text.Substring(found + length, _text.IndexOf("\n", found, StringComparison.Ordinal) - found - length);

                return String.Empty;
            }
        }


        #region Folding Section


        internal override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        /// <summary>
        /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        public class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public IEnumerable<LanguageFold> CreateNewFoldings(ITextSource document)
            {
                var newFoldings = new List<LanguageFold>();

                newFoldings.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "def", "end ", false));
                newFoldings.AddRange(CreateFoldingHelper(document, "global def", "end", true));
          
                newFoldings.AddRange(CreateFoldingHelper(document, "global deffct ", "endfct", true));

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }

        }
        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var s = Regex.Split(section.Title, "æ");

            const string sStart = "%{PE}%";

            var perct = section.TextContent.IndexOf(sStart, StringComparison.Ordinal) - sStart.Length;
            var crlf = section.TextContent.IndexOf("\r\n", StringComparison.Ordinal);

            var start = section.StartOffset + s[0].Length;
            var end = section.Length - (s[0].Length + s[1].Length);
            if (perct > -1)
                end = perct < crlf ? perct : end;

            return doc.GetText(start, end);
        }
        #endregion


        public static MenuItem MenuItems
        {
            get
            {
                //Add to main menu
                var MainItem = new MenuItem {Header = "KUKA"};

                //Add to a sub item
                var newMenuItem2 = new MenuItem {Header = "Test 456"};
                MainItem.Items.Add(newMenuItem2);

                return MainItem;
            }
        }


        internal override System.Windows.Media.Color FocusedColor
        {
            get { return System.Windows.Media.Colors.Orange ; }
        }

        internal override System.Windows.Media.Color UnfocusedColor
        {
            get { return System.Windows.Media.Colors.Gray; }
        }
        
        const RegexOptions ro = (int)RegexOptions.IgnoreCase+RegexOptions.Multiline;

        public override FileModel GetFile(System.IO.FileInfo file)
        {
            switch (file.Extension)
            {
                case ".src":
                    GetInfo();
                       return new FileModel {File = file, Icon = Utilities.LoadBitmap(Global.imgSRC)};
                case ".dat":
                        GetInfo();
                        return new FileModel {File = file, Icon = Utilities.LoadBitmap(Global.imgDAT)};
                case ".sub":
                case ".sps":
                case ".kfd":
                    GetInfo();
                    return new FileModel {File = file, Icon = Utilities.LoadBitmap(Global.imgSPS)};
            }
            return null;
        }
        private void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
            var snippet = new Snippet
                              {
                                  
                              }
            var loopCounter = new SnippetReplaceableTextElement { Text = "i" };
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
        private Snippet forSnippet
        {
            get
            {
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
                return snippet;
            }
        }

        #region Regex Expressions

        public override Regex EnumRegex {get {return new Regex("^ENUM ",ro);}}
        
        public override Regex StructRegex {get {return new Regex("DECL STRUC|^STRUC",ro);}}
        
        //public override Regex MethodRegex {get {return new Regex("GLOBAL DEFFCT |^DEFFCT |GLOBAL DEF |^DEF |^EXT ",ro);}}
        public override Regex MethodRegex { get { return new Regex(@"^[GLOBAL ]*(DEF)+\s+([\w\d]+\s*)\(", ro); } }
       
        public override Regex FieldRegex { get { return new Regex(@"^[GLOBAL ]*[DECL ]*(INT|REAL|BOOL)\s+([\$0-9a-zA-Z_\[\],\$]+)=?([^\r\n;]*);?([^\r\n]*)", ro); } }
    	
        public override string CommentChar {get{return ";";}}
        public override Regex SignalRegex { get { return new Regex("Signal",ro); } }

        public override Regex XYZRegex { get { return new Regex(@"^[DECL ]*(E6POS|E6AXIS|FRAME) ([\w\d_\$]+)", ro); } }
        #endregion
    }
    }

