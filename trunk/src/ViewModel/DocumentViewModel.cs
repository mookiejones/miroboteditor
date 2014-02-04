﻿/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/09/2013
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;
using miRobotEditor.Languages;
using System.IO;
using System.Windows;
using miRobotEditor.Resources.StringResources;

namespace miRobotEditor.ViewModel
{
    public class DocumentViewModel : FileViewModel,IDocument
    {

        public DocumentViewModel(string filepath) : base(filepath)
        {
            InitializeViewModel(filepath);
            ContentId = filepath;
        }

        public DocumentViewModel(string filepath, AbstractLanguageClass lang):base(filepath)
        {
            FileLanguage = lang;
            InitializeViewModel(filepath);
        }

        void InitializeViewModel(string filepath)
        {
            Load(filepath);
            TextBox.FileLanguage = FileLanguage;
           
            TextBox.GotFocus += (s, e) => { TextBox = s as EditorClass; };
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;

            if (filepath != null)
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(filepath));
            Instance = this;
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
        }


        #region CloseCommand

        private RelayCommand _closeCommand;
        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(ExecuteCloseCommand));
            }
        }

        private void ExecuteCloseCommand()
        {
            CloseWindow();
        }
        #endregion
    
       
        public void CloseWindow()
        {

            if (IsDirty)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", FileName), "miRobotEditor", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                    return;
                if (res == MessageBoxResult.Yes)
                {
                    Save(TextBox);
                }
            }

            WorkspaceViewModel.Instance.Close(this);
        }

        internal void Save(EditorClass txtBox)
        {

            var fn = txtBox.Filename;
            if (txtBox.Filename == null)
            {
                txtBox.Save();

                if (fn != txtBox.Filename)
                    RaiseFileNameChanged();

            }
            IsDirty = false;
        }

        internal void RaiseFileNameChanged()
        {
            if (FilenameChanged != null)
                FilenameChanged(this, new EventArgs());
        }

        #region Properties

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility { get { return _visibility; } set { _visibility = value; RaisePropertyChanged(@"Visibility"); } }

        public static DocumentViewModel Instance { get; set; }
        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; RaisePropertyChanged(@"FileLanguage"); } }
        private EditorClass _textBox = new EditorClass();
        public EditorClass TextBox { get { return _textBox; } set { _textBox = value; RaisePropertyChanged(@"TextBox"); } }
        #endregion


      
        protected void TextChanged(object sender)
        {
            Console.WriteLine(Findahome.FileNameIs, FileName);

            TextBox = sender as EditorClass;
            if (TextBox != null) FileLanguage.RawText = TextBox.Text ;


            RaisePropertyChanged("Title");
        }



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
        /// Select Text from variable offset
        /// </summary>
        /// <remarks>
        /// Selects appropriate editor that text resides in.
        /// </remarks>
        public void SelectText(IVariable var)
        {
            if (var.Name == null) throw new ArgumentNullException("var");

            //TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window

            // Is Offset of textbox greater than desired value?
            var enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)
                TextBox.SelectText(var);
        }


        public event FileNameChangedEventHandler FilenameChanged;
    }
}