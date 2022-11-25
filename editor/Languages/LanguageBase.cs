using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Abstract;
using RobotEditor.Classes;
using RobotEditor.Enums;
using RobotEditor.ViewModel;
using FileInfo = System.IO.FileInfo;

namespace RobotEditor.Languages
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

        public override List<string> SearchFilters
        {
            get { return DefaultSearchFilters; }
        }

        private static List<string> DefaultSearchFilters
        {
            get
            {
                return new List<string>
                {
                    "*.*"
                };
            }
        }

        internal override Typlanguage RobotType
        {
            get { return Typlanguage.None; }
        }

        internal override string FunctionItems
        {
            get { return string.Empty; }
        }

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

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

        public override Regex MethodRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex StructRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex FieldRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex EnumRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override void Initialize(string filename)
        {
            Initialize();
        }

        public override string CommentChar
        {
            get { throw new NotImplementedException(); }
        }

        public override Regex SignalRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex XYZRegex
        {
            get { return new Regex(string.Empty); }
        }

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
            var array = Regex.Split(section.Title, "æ");
            var offset = section.StartOffset + array[0].Length;
            var length = section.Length - array[0].Length;
            return doc.GetText(offset, length);
        }

        public override string ExtractXYZ(string positionstring)
        {
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }
    }
}