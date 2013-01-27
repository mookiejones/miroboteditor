using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
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
        public KUKA(string file):base(file)
        {         
            FoldingStrategy = new RegionFoldingStrategy();
        }
		
        

        #region Private Members

        readonly Language_Specific.FileInfo _fi = new Language_Specific.FileInfo();
        internal override Enums.Typlanguage RobotType { get { return Enums.Typlanguage.KUKA; } }
        
        #endregion

        public KUKA()
        {
            FoldingStrategy = new RegionFoldingStrategy();
          
        }


        public Language_Specific.FileInfo GetFileInfo(string text)
        {
            return _fi.GetFileInfo(text);
        }

      
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

     


        /*        public new bool ContextPromptBeforeOpen(ILexemLine lexemLine)
                {
                    // get Lexem
                    Lexem = LanguageBase.GetLexem(lexemLine);
                    return IsLexemPrompt();
                }
         */
       

        #region "_file Interface Info"

        [Localizable(false)]
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

/*
        private static Editor GetPointsFromArray(Editor editor, Collection<string> points)
        {
            if (points == null) throw new ArgumentNullException("points");
            for (var b = 0; b <= (points.Count - 1); b++)
            {
                editor.Text = editor.Text + points[b] + "\r\n";
            }
            return editor;
        }
*/

        //TODO Find out where this is used
        private static Collection<string> GetPositionFromFile(int line, ITextEditorComponent editor)
        {
            var points = new Collection<string>();
            while (true)
            {
                points.Add(editor.Document.Lines[line].ToString());
                /*   if (!editor.Lines[LineNumber].ToUpperInvariant().IndexOf(";ENDFOLD", StringComparison.OrdinalIgnoreCase).Equals(-1))
                   { 
                       return Points;
                   }*/
                line++;
            }
/*
            return points;
*/
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        public static Editor ReversePath(Editor editor)
        {
            var points = new Collection<Collection<string>>();
            for ( var i = 0; i <=(editor.Document.Lines.Count - 1);i++)
            {                
                   if ((editor.Document.Lines[i].ToString().ToUpperInvariant().IndexOf(";FOLD LIN", StringComparison.OrdinalIgnoreCase) > -1) | (editor.Document.Lines[i].ToString().ToUpperInvariant().IndexOf(";FOLD PTP", StringComparison.OrdinalIgnoreCase) > -1))
                 {
                     points.Add(GetPositionFromFile(i, editor));
                 }
            }
            editor.Text = string.Empty;
            for (var b = points.Count - 1; b >= 0; b--)
            {
                for (var j = 0; j < points[b].Count; j++)
                {
                    var l = points[b];
                    editor.AppendText(l[j] + "\r\n");
                }
            }

            return editor;
        }

     

        internal static class FunctionGenerator
        {
            private static string _functionFile = String.Empty;
            private static string GetSTRUC(string filename)
            {
                return RemoveFromFile(filename, "((?<!_)STRUC [\\w\\s,\\[\\]]*)");
            }

// ReSharper disable MemberHidesStaticFromOuterClass
            public static string GetSystemFunctions()
// ReSharper restore MemberHidesStaticFromOuterClass
            {

                var sb = new System.Text.StringBuilder();
                using (var frm = new Forms.FrmSystemFunctions())
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
                           MessageViewModel.Instance.AddError(ex);
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
                    var matchs = rgx.Matches(line);
                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                            sb.AppendLine(match.Value);
                    }
                }
                var regex = new Regex(matchString);
                var result = regex.Replace(line, String.Empty);

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
                    var line = r.ReadToEnd();

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
                var mainItem = new MenuItem {Header = "KUKA"};

                //Add to a sub item
                var newMenuItem2 = new MenuItem {Header = "Test 456"};
                mainItem.Items.Add(newMenuItem2);

                return mainItem;
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
        
        private const RegexOptions Ro = (int)RegexOptions.IgnoreCase+RegexOptions.Multiline;

        public override FileModel GetFile(string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case ".src":
                    GetInfo();
                    return new FileModel { FileName = filename, Icon = Utilities.LoadBitmap(Global.ImgSrc) };
                case ".dat":
                        GetInfo();
                        return new FileModel { FileName = filename, Icon = Utilities.LoadBitmap(Global.ImgDat) };
                case ".sub":
                case ".sps":
                case ".kfd":
                    GetInfo();
                    return new FileModel { FileName = filename, Icon = Utilities.LoadBitmap(Global.ImgSps) };
            }
            return null;
        }
        private void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
        	var sc = new SnippetCollection {forSnippet};

            return sc;

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
             
                return snippet;
            }
        }

        #region Regex Expressions

        public override Regex EnumRegex {get {return new Regex("^ENUM ",Ro);}}
        
        public override Regex StructRegex {get {return new Regex("DECL STRUC|^STRUC",Ro);}}
        
        //public override Regex MethodRegex {get {return new Regex("GLOBAL DEFFCT |^DEFFCT |GLOBAL DEF |^DEF |^EXT ",ro);}}
        public override Regex MethodRegex { get { return new Regex(@"^[GLOBAL ]*(DEF)+\s+([\w\d]+\s*)\(", Ro); } }

        public override Regex FieldRegex { get { return new Regex(@"^[DECL ]*[GLOBAL ]*[CONST ]*(INT|REAL|BOOL|CHAR)\s+([\$0-9a-zA-Z_\[\],\$]+)=?([^\r\n;]*);?([^\r\n]*)", Ro); } }
        protected override string ShiftRegex { get { return @"((E6POS [\w]*={)X\s([\d.-]*)\s*,*Y\s*([-.\d]*)\s*,Z\s*([-\d.]*))"; } }
        internal override string FunctionItems
        {
            get { return @"((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\w\s]*)\(([\w\]\s:_\[,]*)\))" ; }
        }
        public override string CommentChar {get{return ";";}}
        public override Regex SignalRegex { get { return new Regex("Signal",Ro); } }
        public override string ExtractXYZ(string positionstring)
        {
            var p = new PositionBase(positionstring);
            return p.ExtractFromMatch();
        }

        public override Regex XYZRegex { get { return new Regex(@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)=?\{?([^}}]*)?\}?", Ro); } }
        #endregion
        
        public static string GetDatFileName(string filename)
        {
        	return filename.Substring(0,filename.LastIndexOf('.')) + ".dat";
        }
        public static List<string> GetModuleFileNames(string filename)
        {
        	var rootname = filename.Substring(0,filename.LastIndexOf('.'));
        	var result = new List<string>();
        	
        	if (File.Exists(rootname + ".src"))
        		result.Add(rootname + ".src");
        	
        	if (File.Exists(rootname + ".dat"))
        		result.Add(rootname + ".dat");
        	
        	return result;
        	
        }
    }
    }

