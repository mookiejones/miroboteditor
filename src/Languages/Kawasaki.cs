using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class Kawasaki : AbstractLanguageClass
    {
        private const RegexOptions Ro = (int) RegexOptions.IgnoreCase + RegexOptions.Multiline;
        private static ObservableCollection<Snippet> _snippets;

        public Kawasaki(string file) : base(file)
        {
            Filename = file;
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public Kawasaki()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        /// <summary>
        ///     Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters
        {
            get { return EXT; }
        }

        public static List<string> EXT
        {
            get { return new List<string> {".as", ".prg"}; }
        }

        internal override Typlanguage RobotType
        {
            get { return Typlanguage.KAWASAKI; }
        }

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                var codeCompletionList = new List<ICompletionData> {new CodeCompletion("Item1")};
                return codeCompletionList;
            }
        }

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }


        internal override string FunctionItems
        {
            get { return @"(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)"; }
        }

        internal override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        // public override string SignalRegex{get{return "DEFSIG_";}}
        public override Regex MethodRegex
        {
            get { return new Regex("(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)", Ro); }
        }

        public override Regex StructRegex
        {
            get { return new Regex("(ISKAWASAKI)(ISKAWASAKI)(ISKAWASAKI)", Ro); }
        }

        public override Regex FieldRegex
        {
            get { return new Regex("(ISKAWASAKI)(ISKAWASAKI)(ISKAWASAKI)", Ro); }
        }

        public override Regex EnumRegex
        {
            get { return new Regex("^ENUM ", Ro); }
        }

        public override Regex XYZRegex
        {
            get { return new Regex(@"^(LINEAR|JOINT) ([^#])*#\[([^\]]*)", Ro); }
        }

        public override string CommentChar
        {
            get { return ";"; }
        }


        public override Regex SignalRegex
        {
            get { return new Regex(String.Empty); }
        }

        internal override bool IsFileValid(System.IO.FileInfo file)
        {
            return EXT.Any(e => file.Extension.ToLower() == e);
        }

        public override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");
            int start = section.StartOffset;
            int end = section.Length - (s[0].Length + s[1].Length);

            return doc.GetText(start, end);
        }

        public override string ExtractXYZ(string positionstring)
        {
#pragma warning disable 168
            var p = new PositionBase(positionstring);
#pragma warning restore 168

            return positionstring.Substring(positionstring.IndexOf("#[") + 2);
        }


        public override DocumentViewModel GetFile(string filepath)
        {
            return new DocumentViewModel(filepath);
        }

        public override ObservableCollection<Snippet> GetSnippets()
        {
            if (_snippets != null)
                return _snippets;
            throw new NotImplementedException();
        }

        private class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            public void UpdateFoldings(FoldingManager manager, TextDocument document)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Create <see cref="NewFolding" />s for the specified document.
            /// </summary>
            public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }


            /// <summary>
            ///     Create <see cref="NewFolding" />s for the specified document.
            /// </summary>
            private IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var newFoldings = new List<NewFolding>();
                newFoldings.AddRange(CreateFoldingHelper(document, ".program", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".robotdata1", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".ope_info1", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".sysdata", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".auxdata", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".awdata", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".inter_panel_d", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".inter_panel_color_d", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".sig_comment", ".end", false));
                newFoldings.AddRange(CreateFoldingHelper(document, ".trans", ".end", true));
                newFoldings.AddRange(CreateFoldingHelper(document, ".real", ".end", true));
                newFoldings.AddRange(CreateFoldingHelper(document, ".strings", ".end", true));

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }
        }
    }
}