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
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
namespace miRobotEditor.ViewModel
{

    public interface IDocument
    {
        void Load(string filepath);
        void SelectText(IVariable variable);
        Visibility Visibility { get; set; }       
        AbstractLanguageClass FileLanguage { get; set; }
        Editor TextBox { get; set; }
        string FilePath { get; set; }
        ImageSource IconSource { get; set; }
        string FileName { get;  }
        string Title { get; set; }
        bool IsDirty { get; set; }
        string ContentId { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
    }

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
            if (filepath != null)
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(filepath));
            Instance = this;
            TextBox.TextChanged += (s, e) => TextChanged(s);
        }


        #region Properties

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility { get { return _visibility; } set { _visibility = value; RaisePropertyChanged("Visibility"); } }

        public static DocumentViewModel Instance { get; set; }
        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get { return _filelanguage; } set { _filelanguage = value; RaisePropertyChanged("FileLanguage"); } }
        private Editor _textBox = new Editor();
        public Editor TextBox { get { return _textBox; } set { _textBox = value; RaisePropertyChanged("TextBox"); } }

        public ICSharpCode.AvalonEdit.Document.TextDocument TextDocument { get; set; }
        public ICSharpCode.AvalonEdit.Document.TextDocument Document { get; set; }
        #endregion


      
        protected void TextChanged(object sender)
        {

            TextBox = sender as Editor;
            FileLanguage.RawText = TextBox.Text ;


            RaisePropertyChanged("Title");
        }



        public void Load(string filepath)
        {
            FilePath = filepath;
            Instance = this;

            TextBox.FileLanguage = FileLanguage;


            var LoadDatFileOnly = Path.GetExtension(FilePath) == ".dat";


            //TODO Set Icon For File
            TextBox.Filename = FilePath;
            //TODO Come back to this;
        //   TextBox.SetHighlighting();
            TextBox.Text = FileLanguage.SourceText;


            // Select Original File            
            RaisePropertyChanged("Title");
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

            // Is Offset of textbox greater than desired value?
            var enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)
                TextBox.SelectText(var);
        }

    }
    public class KukaViewModel : DocumentViewModel,IDocument
    {

        public KukaViewModel(string filepath,AbstractLanguageClass lang): base(filepath,lang)
        {

            Grid.Loaded += Grid_Loaded;

            //     TextBox = new Editor();
            ShowGrid = false;
            FileLanguage = lang;
            Source.FileLanguage = FileLanguage;
            Data.FileLanguage = FileLanguage;
            Source.GotFocus += (s, e) => { TextBox = s as Editor; };
            Data.GotFocus += (s, e) => { TextBox = s as Editor; };
            Source.TextChanged += (s, e) => TextChanged(s);
            Data.TextChanged += (s, e) => TextChanged(s);

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
            get { return _toggleGridCommand ?? (_toggleGridCommand = new RelayCommand((p) => ToggleGrid(), (p) => Grid != null)); }
        }

     


        #endregion

        #region Properties



        private Controls.ExtendedGridSplitter _grid = new Controls.ExtendedGridSplitter();
        public Controls.ExtendedGridSplitter Grid { get { return _grid; } set { _grid = value; RaisePropertyChanged("Grid"); } }

        private Editor _source = new Editor();
        public Editor Source { get { return _source; } set { _source = value; RaisePropertyChanged("Source"); } }
        private Editor _data = new Editor();
        public Editor Data { get { return _data; } set { _data = value; RaisePropertyChanged("Data"); } }
        private int _gridrow = 1;
        private int _datarow = 2;

        public int GridRow { get { return _gridrow; } set { _gridrow = value; RaisePropertyChanged("GridRow"); } }
        public int DataRow { get { return _datarow; } set { _datarow = value; RaisePropertyChanged("DataRow"); } }

        #endregion

        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Text = FileLanguage.DataText;
                        Data.Filename = Path.Combine(Path.GetDirectoryName(FileName), FileLanguage.DataName);
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
        public new  void SelectText(IVariable var)
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


        public new void Load(string filepath)
        {
            FilePath = filepath;
            Instance = this;
            TextBox.FileLanguage = FileLanguage;
            Source.FileLanguage = FileLanguage;
            Grid.IsAnimated = false;

            var LoadDatFileOnly = Path.GetExtension(filepath) == ".dat";
            //TODO Set Icon For File

            IconSource = Utilities.LoadBitmap(Global.ImgSrc);
            Source.Filename = filepath;
            Source.SetHighlighting();
            Source.Text = LoadDatFileOnly ? FileLanguage.DataText : FileLanguage.SourceText;

            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                ShowGrid = true;
                Data.FileLanguage = FileLanguage;
                Data.Filename = Path.Combine(Path.GetDirectoryName(filepath), FileLanguage.DataName);
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
