using System;
using System.IO;
using System.Windows;
using miRobotEditor.Abstract;
using miRobotEditor.Controls.TextEditor;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Classes
{
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
           
            TextBox.GotFocus += delegate(object s, RoutedEventArgs e) { TextBox = (s as Editor); };
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