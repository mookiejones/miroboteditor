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
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Core.Classes;
using miRobotEditor.Core.Commands;
using miRobotEditor.Core.Interfaces;
using miRobotEditor.EditorControl.Interfaces;
using miRobotEditor.EditorControl.Languages;

namespace miRobotEditor.EditorControl
{
    public class DocumentModel : FileViewModel, IDocument
    {

        public DocumentModel(string filepath) : base(filepath)
        {
            InitializeViewModel(filepath);
            ContentId = filepath;
        }

        public DocumentModel(string filepath, AbstractLanguageClass lang):base(filepath)
        {
            FileLanguage = lang;
            InitializeViewModel(filepath);
        }

        void InitializeViewModel(string filepath)
        {
            ContentId = filepath;
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
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(CloseWindow)); }
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

            throw new NotImplementedException("Need to Implement Close");

//            Workspace.Instance.Close(this);
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

        
   
        [ObservableProperty]
        private Visibility visibility = Visibility.Visible;
      
        private static DocumentModel _instance;

        public static DocumentModel Instance
        {
            get { return _instance ?? (_instance = new DocumentModel(null)); }
            set  {_instance  =value;
        }
    }
        [ObservableProperty]
        private AbstractLanguageClass fileLanguage = new LanguageBase();
        [ObservableProperty]
         private Editor textBox = new Editor();

        public Editor Source { get { return TextBox; } set { TextBox = value; } }
        public ImageSource IconSource { get; set; }
         public bool IsSelected { get; set; }

        #endregion


      
        protected void TextChanged(object sender)
        {

            TextBox = sender as Editor;
            if (TextBox != null) fileLanguage.RawText = TextBox.Text ;

            OnPropertyChanged(nameof(Title));

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

            OnPropertyChanged(nameof(Title));
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
