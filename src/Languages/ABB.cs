﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    public sealed class ABB : AbstractLanguageClass
    {
        private const RegexOptions Ro = (int) RegexOptions.IgnoreCase + RegexOptions.Multiline;
        private static ObservableCollection<Snippet> _snippets;

        public ABB(string file) : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        // ReSharper disable once InconsistentNaming
        public static List<string> EXT => new List<string> { ".mod", ".prg" };


        /// <summary>
        ///     Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters => EXT;


        internal override Typlanguage RobotType => Typlanguage.ABB;

        internal override string SourceFile => String.Empty;


        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex => @"((RobTarget\s*[\w]*\s*:=\s*\[\[)([\d.-]*),([\d.-]*),([-.\d]*))";

        internal override string FunctionItems => @"((?<!END)()()PROC\s([\d\w]*)[\(\)\w\d_. ]*)";

        public override Regex MethodRegex => new Regex(@"( proc )\s*([\d\w]*)\(([^\)]*)", RegexOptions.IgnoreCase);

        public override Regex StructRegex => new Regex(String.Empty);

        public override Regex FieldRegex => new Regex(
                        @"^([^\r\n]*)(tooldata|wobjdata|num|mecunit|string|datapos|intnum|bool|signaldo|dignaldi|signalgo|signalgi)\s+([\$0-9a-zA-Z_\[\],\$]+)(:=)?([^\r\n]*)",
                        RegexOptions.IgnoreCase);

        public override Regex EnumRegex => new Regex(String.Empty);

        public override Regex XYZRegex => new Regex(@"^[PERS ]*(robtarget|jointtarget) ([\w\d_]*)", Ro);

        //@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)(=\{[^}}]*\})?"


        public override string CommentChar => "!";


        public override Regex SignalRegex => new Regex("SignalDI|SignalDO|SignalGI|SignalGO");

        internal override bool IsFileValid(System.IO.FileInfo file)
        {
            return EXT.Any(e => file.Extension.ToLower() == e);
        }

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

        public override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            int end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            switch (Path.GetExtension(filepath))
            {
                case ".prg":
                case ".mod":
                    return new DocumentViewModel(filepath);
            }
            return null;
        }

        #region Folding Section

        /// <summary>
        ///     The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        private class RegionFoldingStrategy : AbstractFoldingStrategy
        {
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
            public virtual IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
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
    }
}