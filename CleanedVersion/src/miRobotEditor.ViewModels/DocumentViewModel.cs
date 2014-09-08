﻿/*
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
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Core;
using miRobotEditor.Core.Commands;
using miRobotEditor.Core.Interfaces;
using miRobotEditor.EditorControl;
using miRobotEditor.EditorControl.Languages;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Languages;

namespace miRobotEditor.ViewModels
{
    public sealed class DocumentViewModel : FileViewModel,IDocument
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
           
            TextBox.GotFocus += (s, e) => { TextBox = s as Editor; };
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;



            if (filepath != null)
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(filepath));
            Instance = this;
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
        }

        private RelayCommand _closeCommand;
        public bool IsActive { get; set; }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => CloseWindow(), p => true)); }
        }
       
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

            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            throw new NotImplementedException();
//            main.Close(this);

        }

        internal void Save(Editor txtBox)
        {

            if (txtBox.Filename == null)
            {
                txtBox.SaveAs();
            }
            IsDirty = false;
        }

        #region Properties

        private Visibility _visibility = Visibility.Visible;
     

        public Visibility Visibility { get { return _visibility; } set { _visibility = value; RaisePropertyChanged("Visibility"); } }

        public static DocumentViewModel Instance { get; set; }
        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; RaisePropertyChanged("FileLanguage"); } }
        private Editor _textBox = new Editor();
        public Editor TextBox { get { return _textBox; } set { _textBox = value; RaisePropertyChanged("TextBox"); } }
        public ImageSource IconSource { get; set; }
        public string Title { get; set; }
        public string ContentId { get; set; }
        public bool IsSelected { get; set; }

        #endregion

        private void TextChanged(object sender)
        {

            TextBox = sender as Editor;
            if (TextBox != null) FileLanguage.RawText = TextBox.Text ;



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

    }
}
