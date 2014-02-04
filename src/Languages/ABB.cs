using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using miRobotEditor.GUI.Editor;
using miRobotEditor.ViewModel;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.CodeCompletion;
using miRobotEditor.Classes;
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


        public override string IsLineMotion(string lineValue, ICollection<IVariable> variables)
        {
            return string.Empty;

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

        internal override bool IsFileValid(FileInfo file)
        {
            return EXT.Any(e => file.Extension.ToLower() == e);
        }

        internal override string FunctionItems
        {
            get { return  @"((?<!END)()()PROC\s([\d\w]*)[\(\)\w\d_. ]*)" ; }
        }

        public override Regex MethodRegex
        {
            get { return new Regex(@"( proc )\s*([\d\w]*)\(([^\)]*)", RegexOptions.IgnoreCase); }
        }

        public override Regex StructRegex
        {
            get { return new Regex(String.Empty); }
        }

        public override Regex FieldRegex
        {
            get { return new Regex(@"^([^\r\n]*)(tooldata|wobjdata|num|mecunit|string|datapos|intnum|bool|signaldo|dignaldi|signalgo|signalgi)\s+([\$0-9a-zA-Z_\[\],\$]+)(:=)?([^\r\n]*)", RegexOptions.IgnoreCase); }
        }

        public override Regex EnumRegex
        {
            get { return new Regex(String.Empty); }
        }
        public override Regex XYZRegex
        {
            get { return new Regex(@"^[PERS ]*(robtarget|jointtarget) ([\w\d_]*)",Ro); }
        }
        //@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)(=\{[^}}]*\})?"
        public override string CommentChar
        {
            get { return "!"; }
        }


        public override Regex SignalRegex
        {
            get { return new Regex("SignalDI|SignalDO|SignalGI|SignalGO"); }
        }

        public override string ExtractXYZ(string positionstring)
        {
            var p = new PositionBase(positionstring);
            return p.ExtractFromMatch();
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
            private static IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
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
            var s = Regex.Split(section.Title, "æ");

            var start = section.StartOffset + s[0].Length;
            var end = section.Length - (s[0].Length + s[1].Length);


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
    }
}