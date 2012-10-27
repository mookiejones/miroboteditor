using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;
using miRobotEditor.Core.Language;
using miRobotEditor.Languages;

namespace miRobotEditor.Languages
{
    public class Fanuc : AbstractLanguageClass
    {
    	public Fanuc(FileInfo file):base(file)
        {
            _file = file;
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public Fanuc()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override string[] SearchFilters
        {
            get
            {
                return EXT;
            }
        }
        
        public static string[] EXT{get{return new string[] {".ls"};}}
        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] s = System.Text.RegularExpressions.Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            int end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }
        internal override TYPLANGUAGE RobotType { get { return TYPLANGUAGE.Fanuc; } }

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                List<ICompletionData> codeCompletionList = new List<ICompletionData>();
                codeCompletionList.Add(new Classes.CodeCompletion("Item1"));
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
       
     

        internal override Collection<string> FunctionItems
        {
            get { return new Collection<string>() { "(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)" }; }
        }
    
        internal override sealed AbstractFoldingStrategy FoldingStrategy{get;set;}

        public class RegionFoldingStrategy : AbstractFoldingStrategy
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
            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                List<NewFolding> newFoldings = new List<NewFolding>();

                newFoldings.AddRange(LanguageBase.CreateFoldingHelper(document, ";fold", ";endfold", true));
  //              newFoldings.AddRange(LanguageBase.CreateFoldingHelper(document, "def", "end", false));

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }
        }


        internal override System.Windows.Media.Color FocusedColor
        {
            get
            {
                return System.Windows.Media.Colors.Yellow;
            }          
        }

        internal override System.Windows.Media.Color UnfocusedColor
        {
            get
            {
                return System.Windows.Media.Colors.Gray;
            }         
        }
    	
        
        
         /// <summary>
        /// Strips Comment Character from string.
        /// <remarks> Used with the Comment/Uncomment Command</remarks>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override string CommentReplaceString(string text)
        {
        	// Create Result Regex
        	Regex rgx=null;
        	// Is it a line Comment
        	string linereg = @"^[\s]*\d*:\s*!";
        	string blankReg=@"^[\s]*!";
        	if (Regex.IsMatch(text,linereg))
        		rgx = new Regex(@"^([\s\d]*:\s*)!([^\r\n]*)");
        		
        	if (Regex.IsMatch(text,blankReg))
        		rgx = new Regex(@"^([\s]*)!([^\r\n]*)");
        		
        	if (rgx!=null)
        	{
        		Match m = rgx.Match(text);
        			if (m.Success)
        				return m.Groups[1].ToString()+ m.Groups[2].ToString();
        	}			
				return text;			
        }
    	
        public override int CommentOffset(string  text)
        {
        	// Create Result Regex
        	Regex rgx=new Regex(@"(^[\s\d:]+)");
        		
        	if (rgx!=null)
        	{
        		Match m = rgx.Match(text);
        			if (m.Success)
        				return m.Groups[1].Length;
        				//return m.Groups[1].ToString()+ m.Groups[2].ToString();
        	}			
				return 0;		
        }

        /// <summary>
        /// Trims Line and Then Returns if first Character is a comment Character
        /// </summary>
        /// <returns></returns>
        public override bool IsLineCommented(string text)
        {       
        		var linereg =  new Regex(@"^[\s]*\d*:\s*!");
        		var blankReg=new Regex(@"^[\s]*!");
			return ((linereg.IsMatch(text))||(blankReg.IsMatch(text)));
        }
        
        public override Regex MethodRegex {get{return new Regex( String.Empty);}}
    	
        public override Regex StructRegex {get{return new Regex( String.Empty);}}
    	
        public override Regex FieldRegex {get{return new Regex( String.Empty);}}
    	
        public override Regex EnumRegex {get{return new Regex( String.Empty);}}
    	
		public override string CommentChar {get { return "!";}}

        public override Regex SignalRegex { get { return new Regex(String.Empty); } }

        public override Regex XYZRegex { get { return new Regex(String.Empty); } }
        public override Classes.FileModel GetFile(FileInfo file)
        {
            return new FileModel() { File = file };
        }
    }
}
