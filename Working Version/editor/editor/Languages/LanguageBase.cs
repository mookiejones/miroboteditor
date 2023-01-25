using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Controls.TextEditor.Completion;
using miRobotEditor.Controls.TextEditor.Folding;
using miRobotEditor.Controls.TextEditor.Language;
using miRobotEditor.Enums;
using miRobotEditor.ViewModel;
using FileInfo = System.IO.FileInfo;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public sealed class LanguageBase : AbstractLanguageClass
    {
        public LanguageBase()
        {
        }

        public LanguageBase(string file)
            : base(file)
        {
        }

        public override List<string> SearchFilters => DefaultSearchFilters;

        private static List<string> DefaultSearchFilters => new()
        {
                    "*.*"
                };

        internal override Typlanguage RobotType => Typlanguage.None;

        internal override string FunctionItems => string.Empty;

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex => throw new NotImplementedException();

        internal override string SourceFile => throw new NotImplementedException();

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        public override Regex MethodRegex => new(string.Empty);

        public override Regex StructRegex => new(string.Empty);

        public override Regex FieldRegex => new(string.Empty);

        public override Regex EnumRegex => new(string.Empty);

        public override void Initialize(string filename)
        {
            Initialize();
        }

        public override string CommentChar => throw new NotImplementedException();

        public override Regex SignalRegex => new(string.Empty);

        public override Regex XYZRegex => new(string.Empty);

        protected override bool IsFileValid(FileInfo file)
        {
            return false;
        }

        public override DocumentViewModel GetFile(string filename)
        {
            return new DocumentViewModel(filename);
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            string[] array = Regex.Split(section.Title, "æ");
            int offset = section.StartOffset + array[0].Length;
            int length = section.Length - array[0].Length;
            return doc.GetText(offset, length);
        }


    }
}