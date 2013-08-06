using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using Microsoft.Win32;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.GUI;
using miRobotEditor.Properties;
using miRobotEditor.Snippets;
using miRobotEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Global = miRobotEditor.Classes.Global;
using RelayCommand = miRobotEditor.Commands.RelayCommand;
using Utilities = miRobotEditor.Classes.Utilities; 

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class KUKA : AbstractLanguageClass
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">Filename for opening</param>
        public KUKA(string file)
            : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public KUKA()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        #endregion Constructor

        #region Commands

        private RelayCommand _systemFunctionCommand;

        public ICommand SystemFunctionCommand
        {
            get { return _systemFunctionCommand ?? (_systemFunctionCommand = new RelayCommand(p => FunctionGenerator.GetSystemFunctions(), p => true)); }
        }

        #endregion Commands

        #region Private Members

        private readonly Language_Specific.FileInfo _fi = new Language_Specific.FileInfo();

        internal override Typlanguage RobotType { get { return Typlanguage.KUKA; } }

        #endregion Private Members

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

        /// <summary>
        /// Determines if file should be loaded from dat only
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool OnlyDatExists(string filename)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".src"));
        }

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

        #endregion "_file Interface Info"

        public static List<string> Ext
        {
            get
            {
                return new List<string> { ".dat", ".src", ".ini", ".sub", ".zip", ".kfd" };
            }
        }

        internal override bool IsFileValid(FileInfo file)
        {
            return Ext.Any(e => file.Extension.ToLower() == e);
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

        public string Comment { get; set; }

        #region Code Completion Section

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                var codeCompletionList = new List<ICompletionData> { new CodeCompletion("Item1") };
                return codeCompletionList;
            }
        }

        #endregion Code Completion Section

        public override string IsLineMotion(string lineValue, IReadOnlyCollection<IVariable> variables)
        {
            if (lineValue.Trim().StartsWith(";FOLD ", StringComparison.OrdinalIgnoreCase))
                lineValue = lineValue.Replace(";FOLD", String.Empty);

            if (lineValue.Trim().StartsWith("lin", StringComparison.OrdinalIgnoreCase) | lineValue.Trim().StartsWith("ptp", StringComparison.OrdinalIgnoreCase))
            {
                var split = lineValue.Trim().Split(' ');
                Console.WriteLine();
                //TODO Need to account for explicit commands

                var pos = split[1];

                var positions = from v in variables
                                where v.Type.ToUpper() == "E6POS" && (v.Name.ToLower() == pos.ToLower() || v.Name.ToLower() == "x" + pos.ToLower())
                                select v;

                foreach (var p in positions)
                {
                    return p.Value;
                }

                // Parse Value
            }
            return string.Empty;
        }

        //Used for Reverse Path
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
            for (var i = 0; i <= (editor.Document.Lines.Count - 1); i++)
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

            private static string GetStruc(string filename)
            {
                return RemoveFromFile(filename, "((?<!_)STRUC [\\w\\s,\\[\\]]*)");
            }

            // ReSharper disable MemberHidesStaticFromOuterClass
            public static string GetSystemFunctions()
            // ReSharper restore MemberHidesStaticFromOuterClass
            {
                var sb = new System.Text.StringBuilder();

                var vm = new SystemFunctionsViewModel();

                var frm = new System.Windows.Window { Content = vm };

                if (frm.DialogResult.HasValue && frm.DialogResult.Value)
                {
                    var ofd = new OpenFileDialog();

                    try
                    {
                        ofd.Filter = "KUKA VxWorks _file (vxWorks.rt;VxWorks.Debug;*.*)|vxWorks.rt;vxWorks.debug;*.*";
                        ofd.Title = ("Select file for reading System Functions");
                        ofd.InitialDirectory = "C:\\krc\\bin\\";

                        const string st = "************************************************";
                        var result = ofd.ShowDialog();
                        if (result == true)
                        {
                            if (!File.Exists(ofd.FileName)) return null;

                            File.Copy(ofd.FileName, "c:\\Temp.rt", true);
                            _functionFile = "c:\\Temp.rt";
                            if (vm.Structures)
                            {
                                sb.AppendFormat("{0}\r\n*** Structures  ******************\r\n{0}\r\n", st);
                                sb.Append(GetStruc(_functionFile));
                            }
                            if (vm.Programs)
                            {
                                sb.AppendFormat("{0}\r\n*** Programs  ******************\r\n{0}\r\n", st);
                                sb.Append(GetRegex(_functionFile, @"(EXTFCTP|EXTDEF)([\d\w]*)([\[\]\w\d\( :,]*\))"));
                            }
                            if (vm.Functions)
                            {
                                sb.AppendFormat("{0}\r\n*** Functions  ******************\r\n{0}\r\n", st);
                                sb.Append(GetRegex(_functionFile, @"(EXTFCTP|EXTDEF)([\d\w]*)([\[\]\w\d\( :,]*\))"));
                            }
                            if (vm.Variables)
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
                        MessageViewModel.AddError("GetSystemFiles", ex);
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
                var newFoldings = new List<LanguageFold>();

                newFoldings.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "def", "end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, "global def", "end", true));

                newFoldings.AddRange(CreateFoldingHelper(document, "global deffct", "endfct", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "deftp", "endtp", true));

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var s = Regex.Split(section.Title, "æ");
            var eval = section.TextContent.ToLower().Trim();

            const string sStart = "%{PE}%";

            // Trim String
            var resultstring = section.TextContent.Trim();

            var perct = section.TextContent.Trim().IndexOf(sStart, StringComparison.Ordinal) - sStart.Length;
            var crlf = section.TextContent.Trim().IndexOf("\r\n", StringComparison.Ordinal);

            //Find Offset of Text for spaces here
#pragma warning disable 168
            var start = section.StartOffset + s[0].Length;
#pragma warning restore 168

            resultstring = resultstring.Substring(eval.IndexOf(s[0]) + s[0].Length);

            var end = resultstring.Length - s[0].Length;//eval.IndexOf(s[1]);

            if (perct > -1)
                end = perct < crlf ? perct : end;

            //  return section.TextContent.Substring(s[0].Length,section.TextContent.Length-s[0].Length-s[1].Length);

            return resultstring.Substring(0, end);
        }

        #endregion Folding Section

        public new MenuItem MenuItems
        {
            get
            {
                //Add to main menu
                var mainItem = new MenuItem { Header = "KUKA" };

                //Add to a sub item
                var newMenuItem2 = new MenuItem { Header = "Test 456" };
                mainItem.Items.Add(newMenuItem2);

                return mainItem;
            }
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            System.Windows.Media.ImageSource icon = null;
            switch (Path.GetExtension(filepath.ToLower()))
            {
                case ".src":
                    GetInfo();
                    icon = Utilities.GetIcon(Global.ImgSrc);
                    break;

                case ".dat":
                    GetInfo();
                    icon = Utilities.GetIcon(Global.ImgDat);
                    break;

                case ".sub":
                case ".sps":
                case ".kfd":
                    GetInfo();
                    icon = Utilities.GetIcon(Global.ImgSps);
                    break;
            }

            var model = new DocumentViewModel(filepath) { IconSource = icon };

            return model;
        }

        private static void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
            var sc = new SnippetCollection { ForSnippet };

            return sc;
        }

        private static Snippet ForSnippet
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

        public override Regex EnumRegex { get { return new Regex(@"^(ENUM)\s+([\d\w]+)\s+([\d\w,]+)", Ro); } }

        public override Regex StructRegex { get { return new Regex(@"DECL STRUC|^STRUC\s([\w\d]+\s*)", Ro); } }

        //public override Regex MethodRegex {get {return new Regex("GLOBAL DEFFCT |^DEFFCT |GLOBAL DEF |^DEF |^EXT ",ro);}}
        public override Regex MethodRegex { get { return new Regex(@"^[GLOBAL ]*(DEF)+\s+([\w\d]+\s*)\(", Ro); } }

        public override Regex FieldRegex { get { return new Regex(@"^[DECL ]*[GLOBAL ]*[CONST ]*(INT|REAL|BOOL|CHAR)\s+([\$0-9a-zA-Z_\[\],\$]+)=?([^\r\n;]*);?([^\r\n]*)", Ro); } }

        protected override string ShiftRegex { get { return @"((E6POS [\w]*={)X\s([\d.-]*)\s*,*Y\s*([-.\d]*)\s*,Z\s*([-\d.]*))"; } }

        internal override string FunctionItems
        {
            get { return @"((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\w\s]*)\(([\w\]\s:_\[,]*)\))"; }
        }

        public override string CommentChar { get { return ";"; } }

        public override Regex SignalRegex { get { return new Regex(@"^(SIGNAL+)\s+([\d\w]+)\s+([^\r\;]*)", Ro); } }

        public override string ExtractXYZ(string positionstring)
        {
            var p = new PositionBase(positionstring);
            return p.ExtractFromMatch();
        }

        public override Regex XYZRegex { get { return new Regex(@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)=?\{?([^}}]*)?\}?", Ro); } }

        #endregion Regex Expressions

        public static string GetDatFileName(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf('.')) + ".dat";
        }

        public static List<string> GetModuleFileNames(string filename)
        {
            var rootname = filename.Substring(0, filename.LastIndexOf('.'));
            var result = new List<string>();

            if (File.Exists(rootname + ".src"))
                result.Add(rootname + ".src");

            if (File.Exists(rootname + ".dat"))
                result.Add(rootname + ".dat");

            return result;
        }

        public class Positions : ViewModelBase, IVariable
        {
            private bool _isSelected;

            public bool IsSelected
            {
                get
                {
                    return _isSelected;
                }
                set
                {
                    _isSelected = value; RaisePropertyChanged("IsSelected");
                }
            }

            private System.Windows.Media.Imaging.BitmapImage _icon;

            public System.Windows.Media.Imaging.BitmapImage Icon
            {
                get { return _icon; }
                set { _icon = value; RaisePropertyChanged("Icon"); }
            }

            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; RaisePropertyChanged("Name"); }
            }

            private String _type;

            public string Type
            {
                get { return _type; }
                set { _type = value; RaisePropertyChanged("Type"); }
            }

            private string _path;

            public string Path
            {
                get { return _path; }
                set { _path = value; RaisePropertyChanged("Path"); }
            }

            private string _value;

            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value; RaisePropertyChanged("Value");
                }
            }

            private string _comment;

            public string Comment
            {
                get { return _comment; }
                set { _comment = value; RaisePropertyChanged("Value"); }
            }

            private string _declaration;

            public string Declaration
            {
                get { return _declaration; }
                set { _declaration = value; RaisePropertyChanged("Declaration"); }
            }

            private string _description;

            public string Description
            {
                get { return _description; }
                set { _description = value; RaisePropertyChanged("Description"); }
            }

            private int _offset;

            public int Offset
            {
                get { return _offset; }
                set { _offset = value; RaisePropertyChanged("Offset"); }
            }
        }
    }
}