﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Core;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Interfaces;
using miRobotEditor.Properties;
using miRobotEditor.Snippets;
using miRobotEditor.ViewModel;
using Global = miRobotEditor.Classes.Global;
using MenuItem = System.Windows.Controls.MenuItem;
using RelayCommand = miRobotEditor.Commands.RelayCommand;
using Utilities = miRobotEditor.Classes.Utilities;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class KUKA : AbstractLanguageClass
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="file">Filename for opening</param>
        public KUKA(string file) : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public KUKA()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        #endregion

        #region Commands

        private RelayCommand _systemFunctionCommand;

        public ICommand SystemFunctionCommand => _systemFunctionCommand ??
                       (_systemFunctionCommand =
                           new RelayCommand(p => FunctionGenerator.GetSystemFunctions(), p => true));

        #endregion

        #region Private Members

        private readonly FileInfo _fi = new FileInfo();

        internal override Typlanguage RobotType => Typlanguage.KUKA;

        #endregion

        private const RegexOptions Ro = (int) RegexOptions.IgnoreCase + RegexOptions.Multiline;
        private static ObservableCollection<Snippet> _snippets;

        internal override string SourceFile => throw new NotImplementedException();


        public static List<string> Ext => new List<string> { ".dat", ".src", ".ini", ".sub", ".zip", ".kfd" };

        /// Sets ComboBox Filter Items for searching
        /// <returns></returns>
        public override List<string> SearchFilters => Ext;

        public string Comment { get; set; }

        public new MenuItem MenuItems
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

        public override Regex EnumRegex => new Regex(@"^(ENUM)\s+([\d\w]+)\s+([\d\w,]+)", Ro);

        public override Regex StructRegex => new Regex(@"DECL STRUC|^STRUC\s([\w\d]+\s*)", Ro);

        //public override Regex MethodRegex {get {return new Regex("GLOBAL DEFFCT |^DEFFCT |GLOBAL DEF |^DEF |^EXT ",ro);}}
        public override Regex MethodRegex => new Regex(@"^[GLOBAL ]*(DEF)+\s+([\w\d]+\s*)\(", Ro);

        public override Regex FieldRegex => new Regex(
                        @"^[DECL ]*[GLOBAL ]*[CONST ]*(INT|REAL|BOOL|CHAR)\s+([\$0-9a-zA-Z_\[\],\$]+)=?([^\r\n;]*);?([^\r\n]*)",
                        Ro);

        protected override string ShiftRegex => @"((E6POS [\w]*={)X\s([\d.-]*)\s*,*Y\s*([-.\d]*)\s*,Z\s*([-\d.]*))";

        internal override string FunctionItems => @"((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\w\s]*)\(([\w\]\s:_\[,]*)\))";

        public override string CommentChar => ";";

        public override Regex SignalRegex => new Regex(@"^(SIGNAL+)\s+([\d\w]+)\s+([^\r\;]*)", Ro);

        public override Regex XYZRegex => new Regex(@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)=?\{?([^}}]*)?\}?", Ro);

        public override string ExtractXYZ(string positionstring)
        {
            var p = new PositionBase(positionstring);
            return p.ExtractFromMatch();
        }

        #endregion

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

        public FileInfo GetFileInfo(string text)
        {
            return _fi.GetFileInfo(text);
        }

        /// <summary>
        ///     Destructor
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
        ///     Determines if file should be loaded from dat only
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool OnlyDatExists(string filename)
        {
            return
                File.Exists(Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + ".src"));
        }

        internal override bool IsFileValid(System.IO.FileInfo file)
        {
            return Ext.Any(e => file.Extension.ToLower() == e);
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

        public static AvlonEditor ReversePath(AvlonEditor editor)
        {
            var points = new Collection<Collection<string>>();
            for (int i = 0; i <= (editor.Document.Lines.Count - 1); i++)
            {
                if (
                    (editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD LIN", StringComparison.OrdinalIgnoreCase) > -1) |
                    (editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD PTP", StringComparison.OrdinalIgnoreCase) > -1))
                {
                    points.Add(GetPositionFromFile(i, editor));
                }
            }
            editor.Text = string.Empty;
            for (int b = points.Count - 1; b >= 0; b--)
            {
                for (int j = 0; j < points[b].Count; j++)
                {
                    Collection<string> l = points[b];
                    editor.AppendText(l[j] + "\r\n");
                }
            }

            return editor;
        }


        public override ObservableCollection<Snippet> GetSnippets()
        {
            if (_snippets != null)
                return _snippets;
            throw new NotImplementedException();
        }


        public override DocumentViewModel GetFile(string filepath)
        {
            ImageSource icon = null;
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

            var model = new DocumentViewModel(filepath) {IconSource = icon};

            return model;
        }

        private static void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
            var sc = new SnippetCollection {ForSnippet};

            return sc;
        }


        public static string GetDatFileName(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf('.')) + ".dat";
        }

        public static List<string> GetModuleFileNames(string filename)
        {
            string rootname = filename.Substring(0, filename.LastIndexOf('.'));
            var result = new List<string>();

            if (File.Exists(rootname + ".src"))
                result.Add(rootname + ".src");

            if (File.Exists(rootname + ".dat"))
                result.Add(rootname + ".dat");

            return result;
        }

        #region "_file Interface Info"

        public static string GetSystemFunctions => FunctionGenerator.GetSystemFunctions();

        [Localizable(false)]
        public static string SystemFileName()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter =
                    Resources.KUKA_SystemFileName_KUKA_VxWorks_File__vxWorks_rt_VxWorks_Debug__vxWorks_rt_vxWorks_debug;
                ofd.InitialDirectory = @"C:\krc\bin\";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return string.Empty;
        }

        #endregion

        private static class FunctionGenerator
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
                var sb = new StringBuilder();

                var vm = new SystemFunctionsViewModel();

                var frm = new Window {Content = vm};


                if (frm.DialogResult.HasValue && frm.DialogResult.Value)
                {
                    var ofd = new Microsoft.Win32.OpenFileDialog();

                    try
                    {
                        ofd.Filter = "KUKA VxWorks _file (vxWorks.rt;VxWorks.Debug;*.*)|vxWorks.rt;vxWorks.debug;*.*";
                        ofd.Title = ("Select file for reading System Functions");
                        ofd.InitialDirectory = "C:\\krc\\bin\\";

                        const string st = "************************************************";
                        bool? result = ofd.ShowDialog();
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
                var sb = new StringBuilder();
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
                var sb = new StringBuilder();
                // Open _file for reading
                using (var r = new StreamReader(functionFile))
                {
                    // Read each line until EOF
                    string line = r.ReadToEnd();

                    var rgx = new Regex(matchString, RegexOptions.IgnoreCase);
                    MatchCollection matchs = rgx.Matches(line);
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

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        public override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");
            string eval = section.TextContent.ToLower().Trim();

            const string sStart = "%{PE}%";

            // Trim String
            string resultstring = section.TextContent.Trim();


            int perct = section.TextContent.Trim().IndexOf(sStart, StringComparison.Ordinal) - sStart.Length;
            int crlf = section.TextContent.Trim().IndexOf("\r\n", StringComparison.Ordinal);

            //Find Offset of Text for spaces here
#pragma warning disable 168
            int start = section.StartOffset + s[0].Length;
#pragma warning restore 168

            resultstring = resultstring.Substring(eval.IndexOf(s[0]) + s[0].Length);


            int end = resultstring.Length - s[0].Length; //eval.IndexOf(s[1]);


            if (perct > -1)
                end = perct < crlf ? perct : end;

            //  return section.TextContent.Substring(s[0].Length,section.TextContent.Length-s[0].Length-s[1].Length);

            return resultstring.Substring(0, end);
        }

        /// <summary>
        ///     The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            /// <summary>
            ///     Create <see cref="NewFolding" />s for the specified document.
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

            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
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

        #endregion
    }
}