using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;
using miRobotEditor.Languages;
using miRobotEditor.Resources.StringResources;
using System;
using System.IO;
using System.Windows;
using strings = miRobotEditor.Properties.Resources;

namespace miRobotEditor.ViewModel
{
    public class KukaViewModel : DocumentViewModel, IDocument
    {
        public KukaViewModel(string filepath, AbstractLanguageClass lang)
            : base(filepath, lang)
        {
            if (!IsInDesignMode)
            {
                Grid.Loaded += Grid_Loaded;

                ShowGrid = false;
                FileLanguage = lang;
                Source.FileLanguage = FileLanguage;
                Data.FileLanguage = FileLanguage;
                Source.GotFocus += (s, e) => { TextBox = s as EditorClass; };
                Data.GotFocus += (s, e) => { TextBox = s as EditorClass; };
                Source.TextChanged += (s, e) => TextChanged(s);
                Data.TextChanged += (s, e) => TextChanged(s);
                Source.IsModified = false;
                Data.IsModified = false;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.IsAnimated = false;
            Grid.Collapse();
            Grid.IsCollapsed = true;
            Grid.IsAnimated = true;
        }

        #region Public Events

        //    public event UpdateFunctionEventHandler TextUpdated;

        #endregion Public Events

        #region Commands
        private RelayCommand _toggleGridCommand;

        /// <summary>
        /// Gets the ToggleGridCommand.
        /// </summary>
        public RelayCommand ToggleGridCommand
        {
            get
            {
                return _toggleGridCommand ?? (_toggleGridCommand = new RelayCommand(
                    ExecuteToggleGridCommand,
                    CanExecuteToggleGridCommand));
            }
        }

        private void ExecuteToggleGridCommand()
        {
            ToggleGrid();
        }

        private bool CanExecuteToggleGridCommand()
        {
            return Grid!=null;
        }



        #region CloseCommand

        private RelayCommand _closeCommand;
        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public new  RelayCommand CloseCommand
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

       
        #endregion Commands

        #region Properties

        private Controls.ExtendedGridSplitter _grid = new Controls.ExtendedGridSplitter();

        public Controls.ExtendedGridSplitter Grid { get { return _grid; } set { _grid = value; RaisePropertyChanged("Grid"); } }

        private EditorClass _source = new EditorClass();

        public EditorClass Source { get { return _source; } set { _source = value; RaisePropertyChanged("Source"); } }

        private EditorClass _data = new EditorClass();

        public EditorClass Data { get { return _data; } set { _data = value; RaisePropertyChanged("Data"); } }

        private int _gridrow = 1;
        private int _datarow = 2;

        public int GridRow { get { return _gridrow; } set { _gridrow = value; RaisePropertyChanged("GridRow"); } }

        public int DataRow { get { return _datarow; } set { _datarow = value; RaisePropertyChanged("DataRow"); } }

        #endregion Properties

        public new void CloseWindow()
        {
            CheckClose(Data);
            CheckClose(Source);
            WorkspaceViewModel.Instance.Close(this);
        }

        /// <summary>
        /// Checks both boxes to determine if they should be saved or not
        /// </summary>
        /// <param name="txtBox"></param>
        private void CheckClose(EditorClass txtBox)
        {
            if (txtBox != null)
                if (txtBox.IsModified)
                {
                    var res = MessageBox.Show(string.Format(Findahome.SaveChangesQuestion, txtBox.Filename), Findahome.AppName, MessageBoxButton.YesNoCancel);
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
                            Data = new EditorClass();
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
        public new void SelectText(IVariable var)
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

        private void SwitchTextBox()
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

        private void ToggleGrid()
        {
            // Is Grid in start mode?
            //
            if (_grid.IsCollapsed)
                Grid.Expand();
            else
                Grid.Collapse();
        }
    }
}