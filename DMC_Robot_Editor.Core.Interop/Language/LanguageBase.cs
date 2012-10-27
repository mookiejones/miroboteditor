using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.IO;
using System.Text.RegularExpressions;
using miRobotEditor.Classes;
using miRobotEditor.Core.Language;

namespace miRobotEditor.Languages
{
    public class LanguageBase : AbstractLanguageClass, IDisposable
    {
    	public LanguageBase(){}
    	public LanguageBase(FileInfo file):base(file)
    	{
    		_file = file;
    	}


        
        public override FileModel GetFile(FileInfo file)
        {
            return new FileModel() { File = file };
        }
    	#region Properties
        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public override string[] SearchFilters
        {
            get
            {
               return LanguageBase.DefaultSearchFilters;
            }
        }

        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>
        public static string[] DefaultSearchFilters
        {
            get
            {
                return new string[] { "*.*" };
            }
        }

        #endregion

        internal override string FoldTitle(FoldingSection section, ICSharpCode.AvalonEdit.Document.TextDocument doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            string[] s = Regex.Split(section.Title, "æ");

            int start = section.StartOffset + s[0].Length;
            int end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }

        public System.Windows.Forms.ToolStripItem[] AddMenuItems() { return null; }

        internal override TYPLANGUAGE RobotType { get { return TYPLANGUAGE.None; } }

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
                var codeCompletionList = new List<ICompletionData>();
                codeCompletionList.Add(new Classes.CodeCompletion("Item1"));
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
