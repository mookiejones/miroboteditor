using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class Kawasaki : AbstractLanguageClass
    {
        public Kawasaki(string file)
        {
            Filename = file;
            FoldingStrategy = new RegionFoldingStrategy();
        }
        public Kawasaki()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters
        {
            get
            {
                return EXT;
            }
        }

        public static List<string> EXT
        {
        	get{return new List<string> { ".as", ".prg" };}
        }

        internal override Enums.Typlanguage RobotType { get { return Enums.Typlanguage.KAWASAKI; } }

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
            get {throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }



        internal override List<string> FunctionItems
        {
            get { return new List<string> { "(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)" }; }
        }
    
        internal override sealed AbstractFoldingStrategy FoldingStrategy{get;set;}

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
        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = Regex.Split(section.Title, "æ");
            int start = section.StartOffset ;
            int end = section.Length - (s[0].Length + s[1].Length);

            return doc.GetText(start, end);
        }

        internal override System.Windows.Media.Color FocusedColor
        {
            get
            {
                return System.Windows.Media.Colors.LightGray;
            }          
        }

        internal override System.Windows.Media.Color UnfocusedColor
        {
            get
            {
                return System.Windows.Media.Colors.Gray;
            }         
        }

           	
        // public override string SignalRegex{get{return "DEFSIG_";}}
        public override Regex MethodRegex {get{return new Regex( String.Empty);}}
		public override Regex StructRegex {get{return new Regex( String.Empty);}}   	
		public override Regex FieldRegex  {get{return new Regex( String.Empty);}}   	
		public override Regex EnumRegex   {get{return new Regex( String.Empty);}}
        public override Regex XYZRegex { get { return new Regex(String.Empty); } }
    	
		public override string CommentChar {get{return ";";}}


        public override Regex SignalRegex { get { return new Regex(String.Empty); } }


        public override FileModel GetFile(string filename)
        {
            return new FileModel { FileName = filename };
        }
    }
}
