using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using miRobotEditor.Languages;
using miRobotEditor.Classes;
using System.ComponentModel;
using miRobotEditor.ViewModel;
using miRobotEditor.Commands;
namespace miRobotEditor.GUI
{
    /// <summary>
    /// Document Editor
    /// </summary>
    [Localizable(false)]
    public partial class DummyDoc : UserControl, INotifyPropertyChanged
    {


    	
        public string Title
        {        	
            get 
            {
            	var _t = TextBox.IsModified?" *":String.Empty;
            	return String.Format("{0}{1}",String.IsNullOrEmpty(TextBox.Filename)?FilePath:Path.GetFileName(TextBox.Filename),_t);
            }
        }
        
        
        
        private string _filepath = string.Empty;
        public string FilePath{get{return _filepath;} set{_filepath = value;}}
    	
    	public DummyDoc()
        {
            Instance = this;
            TextBox = new Editor();
            InitializeComponent();            
            ShowGrid = false;
            FileLanguage =  AbstractLanguageClass.GetRobotType(Filename);
            Source.FileLanguage = FileLanguage;
            Data.FileLanguage=FileLanguage;
            TextBox.TextChanged += (s,e) => TextChanged(s);
            DataContext = this;
        
        }
    	




        public object Host { get; set; }

        private AbstractLanguageClass _filelanguage = new LanguageBase();
        public AbstractLanguageClass FileLanguage { get{return _filelanguage;} set{_filelanguage=value;OnPropertyChanged("FileLanguage");} }
        
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
        	if (TextBox.Variables==null)
        		SwitchTextBox();
        	
        	
            // Is Offset of textbox greater than desired value?
            var enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)            
	            TextBox.SelectText(var);        	
            else
            {
            	TextBox = data;
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


        public static DummyDoc Instance { get; set; }

        #region Public Events

      //  public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        #region Properties

        public string Filename { get; set; }
               
        public Editor Source
        {
            get {return source; }
            set
            {
                source = value;
                OnPropertyChanged("Source");
            }
        }

        public Editor Data
        {
            get {return data; }
            set
            {
                data=value;
                OnPropertyChanged("Data");
            }
        }
        
        private Editor _textbox = new Editor();
        public Editor TextBox { get {return _textbox;} set{_textbox=value;OnPropertyChanged("TextBox");} }

        #endregion

        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Text = FileLanguage.DataText;
                        Data.Filename = Path.Combine(Path.GetDirectoryName(Filename) , FileLanguage.DataName);
                        Data.SetHighlighting();
                        Data.Visibility = Visibility.Visible;
                        grid.Visibility = Visibility.Visible;
                        Grid.SetRow(grid, 1);
                        Grid.SetRow(data, 2);
                        break;
                    case false:
                        if (Data == null)
                            Data = new Editor();
                        Data.Visibility = Visibility.Collapsed;
                        grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        #region Commands
        private RelayCommand _gotFocusCommand;

        public ICommand GotFocusCommand
        {
            get { return _gotFocusCommand ?? (_gotFocusCommand = new RelayCommand((p) => FocusChanged(p), (p) => TextBox.IsFocused)); }
        }

        private RelayCommand _textChangedCommand;

        public ICommand TextChangedCommand
        {
            get { return _textChangedCommand ?? (_textChangedCommand = new RelayCommand((p) => TextChanged(p), (p) => TextBox!=null)); }
        }

        private RelayCommand _reloadCommand;


        //TODO Should Check to see if the text is changed before reloading.
        public ICommand ReloadCommand
        {
            get { return _reloadCommand ?? (_reloadCommand = new RelayCommand((p) => Load(Filename), (p) => File.Exists(Filename))); }
        }

        #endregion
        void FocusChanged(object sender)
        {
            TextBox = sender as Editor;
            OnPropertyChanged("Title");
        }
     
       


        

        public void Load(string filename)
        {
        	Filename = filename;
//            Filename = filename;
            Instance = this;
            FileLanguage = AbstractLanguageClass.GetRobotType(Filename);
            TextBox.FileLanguage=FileLanguage;
            Source.FileLanguage=FileLanguage;
            grid.IsAnimated = false;
            
            var LoadDatFileOnly = Path.GetExtension(Filename) == ".dat";
            //TODO Set Icon For File
	            Source.Filename = Filename;
	            Source.SetHighlighting();	            
	            Source.Text = LoadDatFileOnly?FileLanguage.DataText:FileLanguage.SourceText;


           
            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                ShowGrid = true;
//                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
				Data.FileLanguage=FileLanguage;
                Data.Filename = Path.Combine(Path.GetDirectoryName(Filename), FileLanguage.DataName); 
                Data.Text = FileLanguage.DataText;
                Data.SetHighlighting();
            }
            else
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = false));
            }

            // Select Original File            
            TextBox = source.Filename == filename ? source : data;
            grid.IsAnimated = true;
            OnPropertyChanged("Title");
        }


        //TODO ReWrite this
        private void ReloadFile(object sender, EventArgs e)
        {
            // Dispatcher is used because of the FileChanged Events
            if (!String.IsNullOrEmpty(Source.Filename))
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Source.Reload()));
            // Dispatcher is used because of the FileChanged Events
            if (!String.IsNullOrEmpty(data.Filename))
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Data.Reload()));
        }

        private void Raiseupdated(object sender, FunctionEventArgs e)
        {
   //       if (TextUpdated != null)
   //       {
   //           if (sender is Editor)
   //               TextBox = sender as Editor;
   //   //TODO Remember why im calling this at this point. It resets the "View As" Setting
   //   //        GetRobotType();
   //           TextUpdated(sender, e);
   //           OnPropertyChanged("Title");
   //       }
        }

        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           Instance = this;

          //
          // if (!(sender is Editor)) return;
          // TextBox = sender as Editor;
           // FileLanguage=AbstractLanguageClass.GetRobotType(Filename);
        }

        public void Find()
        {
            TextBox.ShowFindDialog();
        }

        private void TextChanged(object sender)
        {
            TextBox = sender as Editor;              
            FileLanguage.RawText = Source.Text + Data.Text;
            OnPropertyChanged("Title");
            Raiseupdated(TextBox, new FunctionEventArgs(TextBox.Text));
        }

        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            grid.IsAnimated = false;
            grid.Collapse();
            grid.IsCollapsed = true;
            grid.IsAnimated = true;
        }


        #region OnPropertyChanged
        protected void OnPropertyChanged(string propertyName)
        {
            try
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (InvalidOperationException ex)
            {
                ViewModel.MessageViewModel.AddError(ex);
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        private void FocusChanged(object sender, RoutedEventArgs e)
        {
            TextBox = sender as Editor;
            OnPropertyChanged("Title");
            OnPropertyChanged("TextBox");
        }

        private void dTextChangedCommand(object sender, EventArgs e)
        {
            TextChanged(sender);
           
        }

        
        
    }
    public class DummyDocViewModel:ViewModelBase
    {

        #region Public Events

    //    public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        public string Filename { get; set; }

        #region Properties
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
            Raiseupdated(TextBox, new FunctionEventArgs(TextBox.Text));
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
                ShowGrid=false;
            }


            // Select Original File            
            TextBox = _source.Filename == filename ? _source : _data;
            Grid.IsAnimated = true;
           RaisePropertyChanged("Title");
        }

        private int _gridrow = 1;
        private int _datarow = 1;
        public int GridRow { get { return _gridrow; } set { _gridrow = value; RaisePropertyChanged("GridRow"); } }
        public int DataRow { get { return _datarow; } set { _datarow = value; RaisePropertyChanged("DataRow"); } }
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
        private void Raiseupdated(object sender, FunctionEventArgs e)
        {
   //        if (TextUpdated != null)
   //        {
   //            if (sender is Editor)
   //                TextBox = sender as Editor;
   //            //TODO Remember why im calling this at this point. It resets the "View As" Setting
   //            //        GetRobotType();
   //            TextUpdated(sender, e);
   //            RaisePropertyChanged("Title");
   //        }
        }

    }
}
