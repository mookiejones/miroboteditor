using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Abstract;
using miRobotEditor.Classes;
using miRobotEditor.Controls.TextEditor;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Messages;
using miRobotEditor.ViewModel;
using FileInfo = miRobotEditor.Classes.FileInfo;
using MenuItem = System.Windows.Controls.MenuItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace miRobotEditor.Languages
{
    public class KUKA : AbstractLanguageClass
    {
        private readonly FileInfo _fi = new FileInfo();
        private RelayCommand _systemFunctionCommand;

        public KUKA(string file)
            : base(file)
        {
            //TODO Trying out KUKAs folding Strategy
            FoldingStrategy = new RegionFoldingStrategy();
      //      FoldingStrategy = new KrlFoldingStrategy();
        }

        public override void Initialize(string filename)
        {
            base.Initialize();
        }

        public ICommand SystemFunctionCommand
        {
            get
            {
                return _systemFunctionCommand ??
                       (_systemFunctionCommand = new RelayCommand(() => FunctionGenerator.GetSystemFunctions()));
            }
        }

        internal override Typlanguage RobotType
        {
            get { return Typlanguage.KUKA; }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

        public static string GetSystemFunctions
        {
            get { return FunctionGenerator.GetSystemFunctions(); }
        }

        public static List<string> Ext
        {
            get
            {
                return new List<string>
                {
                    ".dat",
                    ".src",
                    ".ini",
                    ".sub",
                    ".zip",
                    ".kfd"
                };
            }
        }

        public override List<string> SearchFilters
        {
            get { return Ext; }
        }

        public string Comment { get; set; }

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                return new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };
            }
        }

        internal override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        public new MenuItem MenuItems
        {
            get
            {
                var menuItem = new MenuItem
                {
                    Header = "KUKA"
                };
                var newItem = new MenuItem
                {
                    Header = "Test 456"
                };
                menuItem.Items.Add(newItem);
                return menuItem;
            }
        }

        private static Snippet ForSnippet
        {
            get
            {
                return new Snippet
                {
                    Elements =
                    {
                        new SnippetTextElement
                        {
                            Text = "for "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "item"
                        },
                        new SnippetTextElement
                        {
                            Text = " in range("
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "from"
                        },
                        new SnippetTextElement
                        {
                            Text = ", "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "to"
                        },
                        new SnippetTextElement
                        {
                            Text = ", "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "step"
                        },
                        new SnippetTextElement
                        {
                            Text = "):backN\t"
                        },
                        new SnippetSelectionElement()
                    }
                };
            }
        }

        public override Regex EnumRegex
        {
            get
            {
                return new Regex("^(ENUM)\\s+([\\d\\w]+)\\s+([\\d\\w,]+)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        public override Regex StructRegex
        {
            get
            {
                return new Regex("DECL STRUC|^STRUC\\s([\\w\\d]+\\s*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        public override Regex FieldRegex
        {
            get
            {
                return
                    new Regex(
                        "^[DECL ]*[GLOBAL ]*[CONST ]*(INT|REAL|BOOL|CHAR)\\s+([\\$0-9a-zA-Z_\\[\\],\\$]+)=?([^\\r\\n;]*);?([^\\r\\n]*)",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        protected override string ShiftRegex
        {
            get { return "((E6POS [\\w]*={)X\\s([\\d.-]*)\\s*,*Y\\s*([-.\\d]*)\\s*,Z\\s*([-\\d.]*))"; }
        }

        public override Regex MethodRegex
        {
            get
            {
                return new Regex("^[GLOBAL ]*(DEF)+\\s+([\\w_\\d]+\\s*)\\(",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        internal override string FunctionItems
        {
            get { return "((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\\w_\\s]*)\\(([\\w\\]\\s:_\\[,]*)\\))"; }
        }

        public override string CommentChar
        {
            get { return ";"; }
        }

        public override Regex SignalRegex
        {
            get
            {
                return new Regex("^(SIGNAL+)\\s+([\\d\\w]+)\\s+([^\\r\\;]*)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        public override Regex XYZRegex
        {
            get
            {
                return new Regex("^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\\w\\d_\\$]+)=?\\{?([^}}]*)?\\}?",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        public FileInfo GetFileInfo(string text)
        {
            return _fi.GetFileInfo(text);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose(true);
            }
        }

        public static bool OnlyDatExists(string filename)
        {
            return
                File.Exists(Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + ".src"));
        }

        [Localizable(false)]
        public static string SystemFileName()
        {
            string result;
            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "All File (*.*)|*.*";
                openFileDialog.InitialDirectory = "C:\\krc\\bin\\";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    result = openFileDialog.FileName;
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        protected override bool IsFileValid(System.IO.FileInfo file)
        {
            return FileIsValid(file);
        }


        internal bool FileIsValid(System.IO.FileInfo file)
        {
            foreach (var ext in Ext)
            {
                if (file.Extension.ToLower() == ext)
                    return true;
            }
            return false;
        }
        private static Collection<string> GetPositionFromFile(int line, ITextEditorComponent editor)
        {
            var collection = new Collection<string>();
            while (true)
            {
                collection.Add(editor.Document.Lines[line].ToString());
                line++;
            }

            return collection;
        }

        public static Editor ReversePath(Editor editor)
        {
            var collection = new Collection<Collection<string>>();
            for (var i = 0; i <= editor.Document.Lines.Count - 1; i++)
            {
                if (
                    editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD LIN", StringComparison.OrdinalIgnoreCase) > -1 |
                    editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD PTP", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    collection.Add(GetPositionFromFile(i, editor));
                }
            }
            editor.Text = string.Empty;
            for (var j = collection.Count - 1; j >= 0; j--)
            {
                for (var k = 0; k < collection[j].Count; k++)
                {
                    var collection2 = collection[j];
                    editor.AppendText(collection2[k] + "\r\n");
                }
            }
            return editor;
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var array = Regex.Split(section.Title, "�");
            var text = section.TextContent.ToLower().Trim();
            var text2 = section.TextContent.Trim();
            var num = section.TextContent.Trim().IndexOf("%{PE}%", StringComparison.Ordinal) - "%{PE}%".Length;
            var num2 = section.TextContent.Trim().IndexOf("\r\n", StringComparison.Ordinal);
            var num3 = section.StartOffset + array[0].Length;
            text2 = text2.Substring(text.IndexOf(array[0], StringComparison.Ordinal) + array[0].Length);
            var num4 = text2.Length - array[0].Length;
            if (num > -1)
            {
                num4 = ((num < num2) ? num : num4);
            }
            return text2.Substring(0, num4);
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            ImageSource iconSource = null;
            var extension = Path.GetExtension(filepath.ToLower());
            if (extension != null)
            {
                if (!(extension == ".src"))
                {
                    if (!(extension == ".dat"))
                    {
                        if (extension == ".sub" || extension == ".sps" || extension == ".kfd")
                        {
                            GetInfo();
                            iconSource = Utilities.GetIcon("..\\..\\Resources\\spsfile.png");
                        }
                    }
                    else
                    {
                        GetInfo();
                        iconSource = Utilities.GetIcon("..\\..\\Resources\\datfile.png");
                    }
                }
                else
                {
                    GetInfo();
                    iconSource = Utilities.GetIcon("..\\..\\Resources\\srcfile.png");
                }
            }
            return new DocumentViewModel(filepath)
            {
                IconSource = iconSource
            };
        }

        private static void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
            return new SnippetCollection
            {
                ForSnippet
            };
        }

        public override string ExtractXYZ(string positionstring)
        {
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }

        public static string GetDatFileName(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf('.')) + ".dat";
        }

        public static List<string> GetModuleFileNames(string filename)
        {
            var str = filename.Substring(0, filename.LastIndexOf('.'));
            var list = new List<string>();
            if (File.Exists(str + ".src"))
            {
                list.Add(str + ".src");
            }
            if (File.Exists(str + ".dat"))
            {
                list.Add(str + ".dat");
            }
            return list;
        }

        private static class FunctionGenerator
        {
            private static string _functionFile = string.Empty;

            private static string GetStruc(string filename)
            {
                return RemoveFromFile(filename, "((?<!_)STRUC [\\w\\s,\\[\\]]*)");
            }

// ReSharper disable once MemberHidesStaticFromOuterClass
            public static string GetSystemFunctions()
            {
                var stringBuilder = new StringBuilder();
                var systemFunctionsViewModel = new SystemFunctionsViewModel();
                var window = new Window
                {
                    Content = systemFunctionsViewModel
                };
                string result;
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    var openFileDialog = new OpenFileDialog();
                    try
                    {
                        openFileDialog.Filter =
                            "KUKA VxWorks _file (vxWorks.rt;VxWorks.Debug;*.*)|vxWorks.rt;vxWorks.debug;*.*";
                        openFileDialog.Title = "Select file for reading System Functions";
                        openFileDialog.InitialDirectory = "C:\\krc\\bin\\";
                        if (openFileDialog.ShowDialog() == true)
                        {
                            if (!File.Exists(openFileDialog.FileName))
                            {
                                return null;
                            }
                            File.Copy(openFileDialog.FileName, "c:\\Temp.rt", true);
                            _functionFile = "c:\\Temp.rt";
                            if (systemFunctionsViewModel.Structures)
                            {
                                stringBuilder.AppendFormat("{0}\r\n*** Structures  ******************\r\n{0}\r\n",
                                    "************************************************");
                                stringBuilder.Append(GetStruc(_functionFile));
                            }
                            if (systemFunctionsViewModel.Programs)
                            {
                                stringBuilder.AppendFormat("{0}\r\n*** Programs  ******************\r\n{0}\r\n",
                                    "************************************************");
                                stringBuilder.Append(GetRegex(_functionFile,
                                    "(EXTFCTP|EXTDEF)([\\d\\w]*)([\\[\\]\\w\\d\\( :,]*\\))"));
                            }
                            if (systemFunctionsViewModel.Functions)
                            {
                                stringBuilder.AppendFormat("{0}\r\n*** Functions  ******************\r\n{0}\r\n",
                                    "************************************************");
                                stringBuilder.Append(GetRegex(_functionFile,
                                    "(EXTFCTP|EXTDEF)([\\d\\w]*)([\\[\\]\\w\\d\\( :,]*\\))"));
                            }
                            if (systemFunctionsViewModel.Variables)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = new ErrorMessage("GetSystemFiles", ex, MessageType.Error);
                        Messenger.Default.Send<IMessage>(msg);
                    }
                }
                result = stringBuilder.ToString();
                return result;
            }

            private static string RemoveFromFile(string functionfile, string matchString)
            {
                var stringBuilder = new StringBuilder();
                string input;
                using (var streamReader = new StreamReader(_functionFile))
                {
                    input = streamReader.ReadToEnd();
                    var regex = new Regex(matchString, RegexOptions.IgnoreCase);
                    var matchCollection = regex.Matches(input);
                    if (matchCollection.Count > 0)
                    {
                        foreach (Match match in matchCollection)
                        {
                            stringBuilder.AppendLine(match.Value);
                        }
                    }
                }
                var regex2 = new Regex(matchString);
                var value = regex2.Replace(input, string.Empty);
                using (var streamWriter = new StreamWriter(functionfile))
                {
                    streamWriter.Write(value);
                }
                return stringBuilder.ToString();
            }

            private static string GetRegex(string functionFile, string matchString)
            {
                string result;
                if (string.IsNullOrEmpty(functionFile))
                {
                    result = null;
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    using (var streamReader = new StreamReader(functionFile))
                    {
                        var input = streamReader.ReadToEnd();
                        var regex = new Regex(matchString, RegexOptions.IgnoreCase);
                        var matchCollection = regex.Matches(input);
                        if (matchCollection.Count > 0)
                        {
                            foreach (Match match in matchCollection)
                            {
                                stringBuilder.AppendLine(match.Value);
                            }
                        }
                    }
                    result = stringBuilder.ToString();
                }
                return result;
            }
        }

        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                var list = new List<LanguageFold>();
                list.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                list.AddRange(CreateFoldingHelper(document, "def", "end", true));
                list.AddRange(CreateFoldingHelper(document, "global def", "end", true));
                list.AddRange(CreateFoldingHelper(document, "global deffct", "endfct", true));
                list.AddRange(CreateFoldingHelper(document, "deftp", "endtp", true));
                list.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }
}