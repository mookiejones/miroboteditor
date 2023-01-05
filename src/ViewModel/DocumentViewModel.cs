/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/09/2013
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Languages;
using robot_editor.Interfaces;

namespace miRobotEditor.ViewModel
{
    public class DocumentViewModel : FileViewModel, IDocument
    {
        private RelayCommand _closeCommand;

        public DocumentViewModel(string filepath) : base(filepath)
        {
            InitializeViewModel(filepath);
            ContentId = filepath;
        }

        public DocumentViewModel(string filepath, AbstractLanguageClass lang) : base(filepath)
        {
            FileLanguage = lang;
            InitializeViewModel(filepath);
        }

        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => CloseWindow(), p => true));

        public void Load(string filepath)
        {
            FilePath = filepath;
            Instance = this;

            TextBox.FileLanguage = FileLanguage;


            //TODO Set Icon For File
            TextBox.Filename = filepath;
            TextBox.SetHighlighting();
            if (File.Exists(filepath))
                TextBox.Text = File.ReadAllText(filepath);


            // Select Original File            
            RaisePropertyChanged("Title");
        }

        /// <summary>
        ///     Select Text from variable offset
        /// </summary>
        /// <remarks>
        ///     Selects appropriate editor that text resides in.
        /// </remarks>
        public void SelectText(IVariable var)
        {
            if (var.Name == null) throw new ArgumentNullException("var");

            //TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window

            // Is Offset of textbox greater than desired value?
            bool enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)
                TextBox.SelectText(var);
        }

        private void InitializeViewModel(string filepath)
        {
            Load(filepath);
            TextBox.FileLanguage = FileLanguage;

            TextBox.GotFocus += (s, e) => { TextBox = s as Editor; };
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;


            if (filepath != null)
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(filepath));
            Instance = this;
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
        }

        public void CloseWindow()
        {
            if (IsDirty)
            {
                MessageBoxResult res = MessageBox.Show(string.Format("Save changes for file '{0}'?", FileName),
                    "miRobotEditor", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                    return;
                if (res == MessageBoxResult.Yes)
                {
                    Save(TextBox);
                }
            }
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.Close(this);
        }

        internal void Save(IEditor txtBox)
        {
            if (txtBox.Filename == null)
            {
                txtBox.SaveAs();
            }
            IsDirty = false;
        }

        protected void TextChanged(object sender)
        {
            TextBox = sender as Editor;
            if (TextBox != null) FileLanguage.RawText = TextBox.Text;


            RaisePropertyChanged("Title");
        }

        #region Properties

        private ILanguageClass _filelanguage = new LanguageBase();
        private Editor _textBox = new Editor();
        private Visibility _visibility = Visibility.Visible;

        public static DocumentViewModel Instance { get; set; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }

        public ILanguageClass FileLanguage
        {
            get { return _filelanguage; }
            set
            {
                _filelanguage = value;
                RaisePropertyChanged("FileLanguage");
            }
        }

        public Editor TextBox
        {
            get { return _textBox; }
            set
            {
                _textBox = value;
                RaisePropertyChanged("TextBox");
            }
        }

        #endregion
    }
}