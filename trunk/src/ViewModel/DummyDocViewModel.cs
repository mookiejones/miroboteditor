/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/09/2013
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using miRobotEditor.Languages;
using miRobotEditor.GUI;
using System.IO;
using System.Text;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.Collections.ObjectModel;
using System.Windows;
namespace miRobotEditor.ViewModel
{

    public class DummyDocViewModel : ViewModelBase
    {

        #region Public Events

        //    public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        public string Filename { get; set; }

        #region Properties

        public string Title
        {
            get
            {
                var _t = TextBox.IsModified ? " *" : String.Empty;
                return String.Format("{0}{1}", String.IsNullOrEmpty(TextBox.Filename) ? FilePath : Path.GetFileName(TextBox.Filename), _t);
            }
        }


        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility { get { return _visibility; } set { _visibility = value; RaisePropertyChanged("Visibility"); } }



        public object Host { get; set; }


        private string _filepath = string.Empty;
        public string FilePath { get { return _filepath; } set { _filepath = value; } }

        private Controls.ExtendedGridSplitter _grid = new Controls.ExtendedGridSplitter();
        public Controls.ExtendedGridSplitter Grid { get { return _grid; } set { _grid = value; RaisePropertyChanged("Grid"); } }
        public static DummyDocViewModel Instance { get; set; }
        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; RaisePropertyChanged("FileLanguage"); } }
        private Editor _textbox = new Editor();
        public Editor TextBox { get { return _textbox; } set { _textbox = value; RaisePropertyChanged("TextBox"); } }
        private Editor _source = new Editor();
        public Editor Source { get { return _source; } set { _source = value; RaisePropertyChanged("Source"); } }
        private Editor _data = new Editor();
        public Editor Data { get { return _data; } set { _data = value; RaisePropertyChanged("Data"); } }
        private int _gridrow = 1;
        private int _datarow = 1;
        public int GridRow { get { return _gridrow; } set { _gridrow = value; RaisePropertyChanged("GridRow"); } }
        public int DataRow { get { return _datarow; } set { _datarow = value; RaisePropertyChanged("DataRow"); } }

        #endregion

        #region Commands
        private RelayCommand _gotFocusCommand;

        public ICommand GotFocusCommand
        {
            get { return _gotFocusCommand ?? (_gotFocusCommand = new RelayCommand((p) => FocusChanged(p), (p) => TextBox.IsFocused)); }
        }

        private RelayCommand _textChangedCommand;

        public ICommand TextChangedCommand
        {
            get { return _textChangedCommand ?? (_textChangedCommand = new RelayCommand((p) => TextChanged(p), (p) => TextBox != null)); }
        }

        void FocusChanged(object sender)
        {
            TextBox = sender as Editor;
            RaisePropertyChanged("Title");
        }

        private RelayCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get { return _reloadCommand ?? (_reloadCommand = new RelayCommand((p) => Load(Filename), (p) => File.Exists(Filename))); }
        }


        private void TextChanged(object sender)
        {

            TextBox = sender as Editor;
            FileLanguage.RawText = Source.Text + Data.Text;


            RaisePropertyChanged("Title");
        }


        #endregion

        public void Load(string filename)
        {
            Filename = filename;
            //            Filename = filename;
            Instance = this;
            FileLanguage = AbstractLanguageClass.GetRobotType(Filename);
            TextBox.FileLanguage = FileLanguage;
            Source.FileLanguage = FileLanguage;
            Grid.IsAnimated = false;

            var LoadDatFileOnly = Path.GetExtension(Filename) == ".dat";
            //TODO Set Icon For File
            Source.Filename = Filename;
            Source.SetHighlighting();
            Source.Text = LoadDatFileOnly ? FileLanguage.DataText : FileLanguage.SourceText;



            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                ShowGrid = true;
                //                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
                Data.FileLanguage = FileLanguage;
                Data.Filename = Path.Combine(Path.GetDirectoryName(Filename), FileLanguage.DataName);
                Data.Text = FileLanguage.DataText;
                Data.SetHighlighting();
            }
            {
                ShowGrid = false;
            }


            // Select Original File            
            TextBox = _source.Filename == filename ? _source : _data;
            Grid.IsAnimated = true;
            RaisePropertyChanged("Title");
        }
      
        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Text = FileLanguage.DataText;
                        Data.Filename = Path.Combine(Path.GetDirectoryName(Filename), FileLanguage.DataName);
                        Data.SetHighlighting();
                        Data.Visibility = Visibility.Visible;
                        Grid.Visibility = Visibility.Visible;
                        GridRow = 1;
                        DataRow = 2;
                        break;
                    case false:
                        if (Data == null)
                            Data = new Editor();
                        Data.Visibility = Visibility.Collapsed;
                        Grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        /// <summary>
        /// Select Text from variable offset
        /// </summary>
        /// <remarks>
        /// Selects appropriate editor that text resides in.
        /// </remarks>
        /// <param name="variable"></param>
        public void SelectText(IVariable var)
        {
            if (var.Name == null) throw new ArgumentNullException("text");

            //TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window

            // Does Textbox have Variables
            if (TextBox.Variables == null)
                SwitchTextBox();


            // Is Offset of textbox greater than desired value?
            var enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)
                TextBox.SelectText(var);
            else
            {
                TextBox = Data;
                enoughlines = TextBox.Text.Length >= var.Offset;
                if (enoughlines)
                    TextBox.SelectText(var);
            }
        }


        void SwitchTextBox()
        {
            switch (TextBox.EditorType)
            {
                case EDITORTYPE.SOURCE:
                    TextBox = Data;
                    break;
                case EDITORTYPE.DATA:
                    TextBox = Source;
                    break;
            }

        }




    }

}
