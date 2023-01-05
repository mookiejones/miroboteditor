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
using miRobotEditor.ViewModel;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public sealed class LanguageBase : AbstractLanguageClass
    {
        public LanguageBase()
        {
        }

        public LanguageBase(string file) : base(file)
        {
        }


        internal override Typlanguage RobotType => Typlanguage.None;

        internal override string FunctionItems => string.Empty;

        public override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex => throw new NotImplementedException();

        internal override string SourceFile => throw new NotImplementedException();

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

        #region Regular Expressions

        private static ObservableCollection<Snippet> _snippets;

        public override Regex MethodRegex => new Regex(String.Empty);

        public override Regex StructRegex => new Regex(String.Empty);

        public override Regex FieldRegex => new Regex(String.Empty);

        public override Regex EnumRegex => new Regex(String.Empty);

        public override string CommentChar => throw new NotImplementedException();

        public override Regex SignalRegex => new Regex(String.Empty);

        public override Regex XYZRegex => new Regex(String.Empty);

        public override ObservableCollection<Snippet> GetSnippets()
        {
            if (_snippets != null)
                return _snippets;
            throw new NotImplementedException();
        }

        public override string ExtractXYZ(string positionstring)
        {
            var p = new PositionBase(positionstring);
            return p.ExtractFromMatch();
        }

        #endregion

        internal override bool IsFileValid(System.IO.FileInfo file)
        {
            return false;
        }

        public override DocumentViewModel GetFile(string filename)
        {
            return new DocumentViewModel(filename);
        }

        public override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            string[] s = Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            // var end = section.Length - (s[0].Length + s[1].Length);
            int end = section.Length - s[0].Length; //eval.IndexOf(s[1]);

            return doc.GetText(start, end);
        }

        #region Properties

        /// <summary>
        ///     Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters => DefaultSearchFilters;

        /// <summary>
        ///     Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        private static List<string> DefaultSearchFilters => new List<string> { "*.*" };

        #endregion
    }
}