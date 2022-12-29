using GalaSoft.MvvmLight;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RobotEditor.Core.Languages
{
    internal class DatCleaner : ViewModelBase
    {
    }

    [Localizable(false)]
    public sealed class Fanuc : AbstractLanguageClass
    {
        public Fanuc(string file)
            : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public Fanuc()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public override List<string> SearchFilters
        {
            get { return EXT; }
        }

        public static List<string> EXT
        {
            get
            {
                return new List<string>
                {
                    ".ls"
                };
            }
        }

        internal override Typlanguage RobotType
        {
            get { return Typlanguage.Fanuc; }
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

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

        internal override string FunctionItems
        {
            get { return "(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)"; }
        }

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

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

        [Localizable(false)]
        public override string CommentChar
        {
            get { return "!"; }
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
            return EXT.Any((string e) => file.Extension.ToLower() == e);
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var array = Regex.Split(section.Title, "æ");
            var offset = section.StartOffset + array[0].Length;
            var length = section.Length - (array[0].Length + array[1].Length);
            return doc.GetText(offset, length);
        }

        public override string CommentReplaceString(string text)
        {
            Regex regex = null;
            if (Regex.IsMatch(text, "^[\\s]*\\d*:\\s*!"))
            {
                regex = new Regex("^([\\s\\d]*:\\s*)!([^\\r\\n]*)");
            }
            if (Regex.IsMatch(text, "^[\\s]*!"))
            {
                regex = new Regex("^([\\s]*)!([^\\r\\n]*)");
            }
            string result;
            if (regex != null)
            {
                var match = regex.Match(text);
                if (match.Success)
                {
                    result = match.Groups[1] + match.Groups[2].ToString();
                    return result;
                }
            }
            result = text;
            return result;
        }

        public override int CommentOffset(string text)
        {
            var regex = new Regex("(^[\\s\\d:]+)");
            var match = regex.Match(text);
            var result = match.Success ? match.Groups[1].Length : 0;
            return result;
        }

        public override bool IsLineCommented(string text)
        {
            var regex = new Regex("^[\\s]*\\d*:\\s*!");
            var regex2 = new Regex("^[\\s]*!");
            return regex.IsMatch(text) || regex2.IsMatch(text);
        }

        public override string ExtractXYZ(string positionstring)
        {
            Debugger.Break();
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            return new DocumentViewModel(filepath);
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
                list.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                list.Sort((NewFolding a, NewFolding b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }

    public abstract class DocumentBase : FileViewModel, IEditorDocument
    {
        public DocumentBase()
        {
        }

        public DocumentBase(string filePath) : base(filePath)
        {
            ContentId = filePath;
            InitializeControl();
        }

        public DocumentBase(string filePath, AbstractLanguageClass language) : base(filePath)
        {
            ContentId = filePath;
            FileLanguage = language;
            InitializeControl();
        }


        public abstract void Close();

        public abstract void Load(string filepath);

        public void SelectText(IVariable variable)
        {
            throw new NotImplementedException();
        }

        private void InitializeControl()
        {
            TextBox.FileLanguage = FileLanguage;
            Load(ContentId);

            TextBox.GotFocus += delegate (object s, RoutedEventArgs e) { TextBox = (s as Editor); };
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
            if (ContentId != null)
            {
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(ContentId));
            }
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
        }

        protected void TextChanged(object sender)
        {
            TextBox = (sender as Editor);
            if (TextBox != null)
            {
                FileLanguage.RawText = TextBox.Text;
            }
            RaisePropertyChanged("Title");
        }

        internal void Save(Editor txtBox)
        {
            if (txtBox.Filename == null)
            {
                txtBox.SaveAs();
            }
            else
            {
                txtBox.Save(FileName);
            }
            IsDirty = false;
        }

        #region FileLanguage

        private AbstractLanguageClass _filelanguage = new LanguageBase();

        public AbstractLanguageClass FileLanguage
        {
            get { return _filelanguage; }
            set
            {
                _filelanguage = value;
                RaisePropertyChanged("FileLanguage");
            }
        }

        #endregion

        #region Visibility

        private Visibility _visibility = Visibility.Visible;

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }

        #endregion        public AbstractLanguageClass FileLanguage { get; set; }

        #region TextBox

        /// <summary>
        ///     The <see cref="TextBox" /> property's name.
        /// </summary>
        private const string TextBoxPropertyName = "TextBox";

        private Editor _textBox = new Editor();

        /// <summary>
        ///     Sets and gets the TextBox property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Editor TextBox
        {
            get { return _textBox; }

            set
            {
                if (_textBox == value)
                {
                    return;
                }


                _textBox = value;
                RaisePropertyChanged(TextBoxPropertyName);
            }
        }

        #endregion
    }
}
