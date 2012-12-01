using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class ABB : AbstractLanguageClass
    {
        
        public ABB()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public ABB(string file) : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public static List<string> EXT
        {
            get { return new List<string> {".mod", ".prg"}; }
        }


        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters
        {
            get { return EXT; }
        }


        internal override Typlanguage RobotType
        {
            get { return Typlanguage.ABB; }
        }

        internal override string SourceFile
        {
            get { return String.Empty; }
        }


        internal override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex
        {
            get { return @"((RobTarget\s*[\w]*\s*:=\s*\[\[)([\d.-]*),([\d.-]*),([-.\d]*))"; }
        }

        internal override Collection<string> FunctionItems
        {
            get { return new Collection<string> {@"((?<!END)()()PROC\s([\d\w]*)[\(\)\w\d_. ]*)"}; }
        }

        internal override Color FocusedColor
        {
            get { return Color.FromRgb(255, 192, 84); }
        }

        internal override Color UnfocusedColor
        {
            get { return Color.FromRgb(255, 192, 84); }
        }


        public override Regex MethodRegex
        {
            get { return new Regex("( proc )", RegexOptions.IgnoreCase); }
        }

        public override Regex StructRegex
        {
            get { return new Regex(String.Empty); }
        }

        public override Regex FieldRegex
        {
            get { return new Regex("( num )", RegexOptions.IgnoreCase); }
        }

        public override Regex EnumRegex
        {
            get { return new Regex(String.Empty); }
        }

        public override Regex XYZRegex
        {
            get { return new Regex("(robtarget|jointtarget)"); }
        }

        public override string CommentChar
        {
            get { return "!"; }
        }


        public override Regex SignalRegex
        {
            get { return new Regex("SignalDI|SignalDO|SignalGI|SignalGO"); }
        }

        #region Folding Section

        /// <summary>
        /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        private class RegionFoldingStrategy : AbstractFoldingStrategy
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
            private IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var newFoldings = new List<NewFolding>();

                newFoldings.AddRange(CreateFoldingHelper(document, "proc", "endproc", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "module", "endmodule", false));
                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }
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

        #region Indentation

        #endregion

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            int end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }

        public override FileModel GetFile(string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case ".prg":
                case ".mod":
                    return new FileModel {FileName = filename};
            }
            return null;
        }
    }
}