using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using miRobotEditor.Controls;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Languages;
using miRobotEditor.Interfaces;

namespace miRobotEditor.ViewModel
{
    public class KukaViewModel : DocumentViewModel, IDocument
    {
        public KukaViewModel(string filepath, AbstractLanguageClass lang) : base(filepath, lang)
        {
            Grid.Loaded += Grid_Loaded;

            ShowGrid = false;
            FileLanguage = lang;
            Source.FileLanguage = FileLanguage;
            Data.FileLanguage = FileLanguage;
            Source.GotFocus += (s, e) => { TextBox = s as AvalonEditor; };
            Data.GotFocus += (s, e) => { TextBox = s as AvalonEditor; };
            Source.TextChanged += (s, e) => TextChanged(s);
            Data.TextChanged += (s, e) => TextChanged(s);
            Source.IsModified = false;
            Data.IsModified = false;
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
                            Data = new AvalonEditor();
                        Data.Visibility = Visibility.Collapsed;
                        Grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        /// <summary>
        ///     Select Text from variable offset
        /// </summary>
        /// <remarks>
        ///     Selects appropriate editor that text resides in.
        /// </remarks>
        public new void SelectText(IVariable var)
        {
            if (var.Name == null) throw new ArgumentNullException("var");

            //TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window

            // Does Textbox have Variables
            if (TextBox.Variables == null)
                SwitchTextBox();


            // Is Offset of textbox greater than desired value?
            bool enoughlines = TextBox.Text.Length >= var.Offset;
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

        public new void Load(string filepath)
        {
            FilePath = filepath;
            Instance = this;
            TextBox.FileLanguage = FileLanguage;
            Source.FileLanguage = FileLanguage;
            Grid.IsAnimated = false;

            bool loadDatFileOnly = Path.GetExtension(filepath) == ".dat";
            //TODO Set Icon For File

            IconSource = Utilities.LoadBitmap(Global.ImgSrc);
            Source.Filename = filepath;
            Source.SetHighlighting();
            Source.Text = loadDatFileOnly ? FileLanguage.DataText : FileLanguage.SourceText;

            if ((FileLanguage is KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) &&
                (Source.Text != FileLanguage.DataText))
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.IsAnimated = false;
            Grid.Collapse();
            Grid.IsCollapsed = true;
            Grid.IsAnimated = true;
        }

        public new void CloseWindow()
        {
            CheckClose(Data);
            CheckClose(Source);
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.Close(this);
        }

        /// <summary>
        ///     Checks both boxes to determine if they should be saved or not
        /// </summary>
        /// <param name="txtBox"></param>
        private void CheckClose(AvalonEditor txtBox)
        {
            if (txtBox != null)
                if (txtBox.IsModified)
                {
                    MessageBoxResult res =
                        MessageBox.Show(string.Format("Save changes for file '{0}'?", txtBox.Filename), "miRobotEditor",
                            MessageBoxButton.YesNoCancel);
                    if (res == MessageBoxResult.Cancel)
                        return;
                    if (res == MessageBoxResult.Yes)
                    {
                        Save(txtBox);
                    }
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


        private void ToggleGrid()
        {
            if (Grid.IsCollapsed)

                Grid.Expand();
            else
                Grid.Collapse();
        }

        #region Public Events

        //    public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        #region Commands

        private RelayCommand _closeCommand;
        private RelayCommand _toggleGridCommand;

        public ICommand ToggleGridCommand => _toggleGridCommand ??
                       (_toggleGridCommand = new RelayCommand(p => ToggleGrid(), p => Grid != null));

        public new ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => CloseWindow(), p => true));

        #endregion

        #region Properties

        private IEditor _data = new AvalonEditor();

        private ExtendedGridSplitter _grid = new ExtendedGridSplitter();
        private int _gridrow = 1;

        private AvalonEditor _source = new AvalonEditor();

        public ExtendedGridSplitter Grid
        {
            get { return _grid; }
            set
            {
                _grid = value;
                RaisePropertyChanged("Grid");
            }
        }

        public AvalonEditor Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged("Source");
            }
        }

        public IEditor Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged("Data");
            }
        }

        public int GridRow
        {
            get { return _gridrow; }
            set
            {
                _gridrow = value;
                RaisePropertyChanged("GridRow");
            }
        }


        
        #region DataRow
        /// <summary>
        /// The <see cref="DataRow" /> property's name.
        /// </summary>
        public const string DataRowPropertyName = "DataRow";

        private int _dataRow = 2;

        /// <summary>
        /// Sets and gets the DataRow property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int DataRow
        {
            get
            {
                return _dataRow;
            }

            set
            {
                if (_dataRow == value)
                {
                    return;
                }

                RaisePropertyChanging(DataRowPropertyName);
                _dataRow = value;
                RaisePropertyChanged(DataRowPropertyName);
            }
        }
        #endregion
      

        #endregion
    }
}