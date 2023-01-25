using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Abstract;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class VBA : AbstractLanguageClass
    {
        /// <summary>
        ///     Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        private static ObservableCollection<Snippet> _snippets;

        public VBA(string file) : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public override List<string> SearchFilters => new List<string> { "*.*", "*.dat", "*.src", "*.ini", "*.sub", "*.zip", "*.kfd" };

        internal override Typlanguage RobotType => Typlanguage.VBA;

        internal override string SourceFile => String.Empty;


        //Was XmlFoldingStrategy
        public override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex => @"((RobTarget\s*[\w]*\s*:=\s*\[\[)([\d.-]*),([\d.-]*),([-.\d]*))";

        internal override string FunctionItems => @"((?<!END)()()PROC\s([\d\w]*)[\(\)\w\d_. ]*)";

        public override Regex MethodRegex => new Regex("( sub )", RegexOptions.IgnoreCase);

        public override Regex StructRegex => new Regex("( struc )", RegexOptions.IgnoreCase);

        public override Regex FieldRegex => new Regex("( boolean )", RegexOptions.IgnoreCase);

        public override Regex EnumRegex => new Regex("( enum )", RegexOptions.IgnoreCase);

        public override string CommentChar => "'";

        public override Regex SignalRegex => new Regex(String.Empty);

        public override Regex XYZRegex => new Regex(String.Empty);

        #region Folding Section

        public override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            int end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }

        /// <summary>
        ///     The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        public class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            /// <summary>
            ///     Create <see cref="NewFolding" />s for the specified document.
            /// </summary>
            public virtual IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var newFoldings = new List<NewFolding>();

                newFoldings.AddRange(CreateFoldingHelper(document, "public function", "end function", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "private function", "end function", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "public sub", "end sub", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "private sub", "end sub", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "property", "end property", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "If", "End If", false));
                newFoldings.AddRange(CreateFoldingHelper(document, "Select Case", "End Select", false));
                //                /Open Fold Strings = "Do While" "If" "ElseIf" "Function" "Sub" "With" "For" "Select Case" "Case Else" "Case" "Else"
                //Close Fold Strings = "ElseIf" "End If" "End Function" "End Sub" "End With" "Loop" "Next" "Wend" "End Select" "Case Else" "Case" "Else"

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }

            /// <summary>
            ///     Create <see cref="NewFolding" />s for the specified document.
            /// </summary>
            public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
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

        public override ObservableCollection<Snippet> GetSnippets()
        {
            if (_snippets != null)
                return _snippets;
            throw new NotImplementedException();
        }

        internal override bool IsFileValid(System.IO.FileInfo file)
        {
            return false;
        }

        public override string ExtractXYZ(string positionstring)
        {
            return String.Empty;
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            return new DocumentViewModel(filepath);
        }
    }
}