using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using miRobotEditor.GUI;
using miRobotEditor.Languages;

namespace miRobotEditor.ViewModel
{
    public class KukaViewModel : DocumentViewModel,IDocument
    {

        public KukaViewModel(string filepath,AbstractLanguageClass lang): base(filepath,lang)
        {

            Grid.Loaded += Grid_Loaded;

            ShowGrid = false;
            FileLanguage = lang;
            Source.FileLanguage = FileLanguage;
            Data.FileLanguage = FileLanguage;
            Source.GotFocus += (s, e) => { TextBox = s as Editor; };
            Data.GotFocus += (s, e) => { TextBox = s as Editor; };
            Source.TextChanged += (s, e) => TextChanged(s);
            Data.TextChanged += (s, e) => TextChanged(s);
            Source.IsModified = false;
            Data.IsModified = false;

        }

        void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
            Grid.IsAnimated = false;
            Grid.Collapse();
            Grid.IsCollapsed = true;
            Grid.IsAnimated = true;
        }
       
        #region Public Events

        //    public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        #region Commands
        private RelayCommand _toggleGridCommand;

        public ICommand ToggleGridCommand
        {
            get { return _toggleGridCommand ?? (_toggleGridCommand = new RelayCommand(p => ToggleGrid(), p => Grid != null)); }
        }
        private RelayCommand _closeCommand;
        public new ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => CloseWindow(), p => true)); }
        }

        #endregion



        #region Properties



        private Controls.ExtendedGridSplitter _grid = new Controls.ExtendedGridSplitter();
        public Controls.ExtendedGridSplitter Grid { get { return _grid; } set { _grid = value; RaisePropertyChanged(); } }

        private Editor _source = new Editor();
        public Editor Source { get { return _source; } set { _source = value; RaisePropertyChanged(); } }
        private Editor _data = new Editor();
        public Editor Data { get { return _data; } set { _data = value; RaisePropertyChanged(); } }
        private int _gridrow = 1;
        private int _datarow = 2;

        public int GridRow { get { return _gridrow; } set { _gridrow = value; RaisePropertyChanged(); } }
        public int DataRow { get { return _datarow; } set { _datarow = value; RaisePropertyChanged(); } }

        #endregion

       
        public new void CloseWindow()
        {
            CheckClose(Data);
            CheckClose(Source);
            Workspace.Instance.Close(this);
        }

        /// <summary>
        /// Checks both boxes to determine if they should be saved or not
        /// </summary>
        /// <param name="txtBox"></param>
        void CheckClose(Editor txtBox)
        {
            if (txtBox != null)
                if (txtBox.IsModified)
                {
                    var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", txtBox.Filename), "miRobotEditor", MessageBoxButton.YesNoCancel);
                    if (res == MessageBoxResult.Cancel)
                        return;
                    if (res == MessageBoxResult.Yes)
                    {
                        Save(txtBox);
                    }
                }
        }
        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Text = FileLanguage.DataText;
// ReSharper disable AssignNullToNotNullAttribute
                        Data.Filename = Path.Combine(Path.GetDirectoryName(FileName), FileLanguage.DataName);
// ReSharper restore AssignNullToNotNullAttribute
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
        public new  void SelectText(IVariable var)
        {
            if (var.Name == null) throw new ArgumentNullException("var");


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
     

        public new void Load(string filepath)
        {
            FilePath = filepath;
            Instance = this;
            TextBox.FileLanguage = FileLanguage;
            Source.FileLanguage = FileLanguage;
            Grid.IsAnimated = false;

            var loadDatFileOnly = Path.GetExtension(filepath) == ".dat" && (!KUKA.OnlyDatExists(filepath));
            //TODO Set Icon For File

            IconSource = Utilities.LoadBitmap(Global.ImgSrc);
            Source.Filename = filepath;
            Source.SetHighlighting();
            Source.Text = loadDatFileOnly ? FileLanguage.DataText : FileLanguage.SourceText;

            if ((FileLanguage is KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                ShowGrid = true;
                Data.FileLanguage = FileLanguage;
// ReSharper disable AssignNullToNotNullAttribute
                Data.Filename = Path.Combine(Path.GetDirectoryName(filepath), FileLanguage.DataName);
// ReSharper restore AssignNullToNotNullAttribute
                Data.Text = FileLanguage.DataText;
                Data.SetHighlighting();
            }
           

            // Select Original File            
            TextBox = Source.Filename == filepath ? Source : Data;
            Grid.IsAnimated = true;
            RaisePropertyChanged("Title");
        }

        void ToggleGrid()
        {

            if (Grid.IsCollapsed)

                Grid.Expand();
            else
                Grid.Collapse();

        }
    }
}