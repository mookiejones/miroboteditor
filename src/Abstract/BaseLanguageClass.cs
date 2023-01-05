using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Abstract
{
    public abstract class BaseLanguageClass : ILanguageClass
    {

        #region · AbstractMembers ·

        public abstract DirectoryInfo RootPath { get; set; }
        public abstract string FileName { get; set; }
        public abstract string RawText { get; set; }
        public Regex MethodRegex { get; set; }
        public Regex StructRegex { get; set; }
        public Regex FieldRegex { get; set; }
        public Regex SignalRegex { get; set; }
        public Regex EnumRegex { get; set; }
        public Regex XYZRegex { get; set; }
        public ReadOnlyObservableCollection<Snippet> Snippets { get; set; }
        public AbstractFoldingStrategy FoldingStrategy { get; set; }
        public string CommentChar { get; set; }
        public IEnumerable<IVariable> Fields { get; set; }
        public abstract MenuItem RobotMenuItems { get; set; }
        public abstract string Name { get; }
        public string SnippetPath { get; }
        public string Intellisense { get; }
        public string SnippetFilePath { get; }
        public string Filename { get; }
        #endregion
        protected string DataName { get; set; }
        protected string SourceName { get; set; }
        public abstract void GetRootDirectory(string path);
        public string FoldTitle(FoldingSection section, TextDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
