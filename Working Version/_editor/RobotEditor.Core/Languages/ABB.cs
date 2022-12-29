using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace RobotEditor.Core.Languages
{
    [Localizable(false)]
    public sealed class ABB : AbstractLanguageClass
    {
        // ReSharper disable once UnusedMember.Local
        private const RegexOptions Ro = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        public ABB(string file)
            : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        // ReSharper disable once InconsistentNaming
        public static List<string> EXT
        {
            get
            {
                return new List<string>
                {
                    ".mod",
                    ".prg"
                };
            }
        }

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
            get { return string.Empty; }
        }

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex
        {
            get { return "((RobTarget\\s*[\\w]*\\s*:=\\s*\\[\\[)([\\d.-]*),([\\d.-]*),([-.\\d]*))"; }
        }

        internal override string FunctionItems
        {
            get { return "((?<!END)()()PROC\\s([\\d\\w]*)[\\(\\)\\w\\d_. ]*)"; }
        }

        public override Regex MethodRegex
        {
            get { return new Regex("( proc )\\s*([\\d\\w]*)\\(([^\\)]*)", RegexOptions.IgnoreCase); }
        }

        public override Regex StructRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex FieldRegex
        {
            get
            {
                return
                    new Regex(
                        "^([^\\r\\n]*)(tooldata|wobjdata|num|mecunit|string|datapos|intnum|bool|signaldo|dignaldi|signalgo|signalgi)\\s+([\\$0-9a-zA-Z_\\[\\],\\$]+)(:=)?([^\\r\\n]*)",
                        RegexOptions.IgnoreCase);
            }
        }

        public override Regex EnumRegex
        {
            get { return new Regex(string.Empty); }
        }

        public override Regex XYZRegex
        {
            get
            {
                return new Regex("^[PERS ]*(robtarget|jointtarget) ([\\w\\d_]*)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        public override void Initialize(string filename)
        {
            Initialize();
        }

        public override string CommentChar
        {
            get { return "!"; }
        }

        public override Regex SignalRegex
        {
            get { return new Regex("SignalDI|SignalDO|SignalGI|SignalGO"); }
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

        protected override bool IsFileValid(FileInfo file)
        {
            return EXT.Any(e => file.Extension.ToLower(CultureInfo.InvariantCulture) == e);
        }

        public override string ExtractXYZ(string positionstring)
        {
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var array = Regex.Split(section.Title, "æ");
            var offset = section.StartOffset + array[0].Length;
            var length = section.Length - (array[0].Length + array[1].Length);
            return doc.GetText(offset, length);
        }

        public override DocumentViewModel GetFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension != null)
            {
                if (extension == ".prg" || extension == ".mod")
                {
                    var result = new DocumentViewModel(filePath);
                    return result;
                }
            }
            return null;
        }

        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var list = new List<NewFolding>();
                list.AddRange(CreateFoldingHelper(document, "proc", "endproc", true));
                list.AddRange(CreateFoldingHelper(document, "module", "endmodule", false));
                list.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }
}
