using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.IO;
using System.Text.RegularExpressions;
using miRobotEditor.Classes;

namespace miRobotEditor.Languages
{
    [Localizable(false)]
    public class LanguageBase : AbstractLanguageClass
    {
    	public LanguageBase(){}
    	public LanguageBase(string file):base(file)
    	{    		
    	}



        public override FileModel GetFile(string filename)
        {
            return new FileModel { FileName = filename };
        }
    	#region Properties
        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override List<string> SearchFilters
        {
            get
            {
               return DefaultSearchFilters;
            }
        }

        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public static List<string> DefaultSearchFilters
        {
            get
            {
                return new List<string> { "*.*" };
            }
        }

        #endregion

        internal override string FoldTitle(FoldingSection section, ICSharpCode.AvalonEdit.Document.TextDocument doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            var s = Regex.Split(section.Title, "æ");

            var start = section.StartOffset + s[0].Length;
            var end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }

        public System.Windows.Forms.ToolStripItem[] AddMenuItems() { return null; }

        internal override Enums.TYPLANGUAGE RobotType { get { return Enums.TYPLANGUAGE.None; } }

        internal override Collection<string> FunctionItems
        {
            get { return new Collection<string>(); }
        }

        internal override AbstractFoldingStrategy FoldingStrategy{ get; set; }

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

       
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


            internal override System.Windows.Media.Color FocusedColor
            {
                get { return System.Windows.Media.Colors.AntiqueWhite; }
            }

            internal override System.Windows.Media.Color UnfocusedColor
            {
                get { return System.Windows.Media.Colors.WhiteSmoke; }
            }

            
            #region Regular Expressions
            public override Regex MethodRegex {get {return new Regex(String.Empty);}}
    	
		public override Regex StructRegex {get {return new Regex(String.Empty);}}
    	
		public override Regex FieldRegex{get {return new Regex(String.Empty);}}
    	
		public override Regex EnumRegex {get {return new Regex(String.Empty);}}
    	
		public override string CommentChar {
			get {
				throw new NotImplementedException();
			}
		}

        public override Regex SignalRegex { get { return new Regex(String.Empty); } }

        public override Regex XYZRegex { get { return new Regex(String.Empty); } }
            #endregion
    }
}
