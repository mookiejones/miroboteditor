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
namespace miRobotEditor.GUI
{
    /// <summary>
    /// Document Editor
    /// </summary>
    [Localizable(false)]
    public partial class DummyDoc : UserControl, INotifyPropertyChanged
    {
        public object Host { get; set; }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public AbstractLanguageClass FileLanguage { get; set; }

        private static DummyDoc _instance;     	
        public static DummyDoc Instance
        {
            get { return _instance ?? (_instance = new DummyDoc()); }
            set { _instance = value; }
        }

        #region Public Events

        public event UpdateFunctionEventHandler TextUpdated;

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

        public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register("TextBox", typeof (Editor),typeof (DummyDoc));

        public Editor TextBox
        {
            get { return (Editor) GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value);
                OnPropertyChanged("TextBox");
            }
        }

       

        #endregion


        public DummyDoc()
        {
            Instance = this;

            InitializeComponent();            
            ShowGrid = false;
            FileLanguage = new LanguageBase();
            DataContext = this;
        }


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
     
        public void CanReload(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = File.Exists(Filename);
        }

        public  void ExecuteReload(object sender, ExecutedRoutedEventArgs e)
        {
            Load(Filename);           
        }

        public void Load(string filename)
        {
            Filename = filename;
            Instance = this;
            FileLanguage = AbstractLanguageClass.GetRobotType(Filename);
            grid.IsAnimated = false;
            //TODO Set Icon For File

            //TODO Get Files Based in language 
            Source.Filename = filename;
            Source.Text = FileLanguage.SourceText;           
            Source.SetHighlighting();

           
            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                ShowGrid = true;
//                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
                Data.Filename = Path.GetDirectoryName(Filename) + "\\" + FileLanguage.DataName;
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
           
        }

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
            if (TextUpdated != null)
            {
                if (sender is Editor)
                    TextBox = sender as Editor;
        //TODO Remember why im calling this at this point. It resets the "View As" Setting
        //        GetRobotType();
                TextUpdated(sender, e);
            }
        }

        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           // if (MainWindow.Instance.IsClosing)
           //     return;
            if (IsVisible)
            Instance = this;
            if (!(sender is Editor)) return;
            TextBox = sender as Editor;
            FileLanguage=AbstractLanguageClass.GetRobotType(Filename);
        }


        public void Find()
        {
            TextBox.ShowFindDialog();
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (sender is Editor)
            {
                TextBox = sender as Editor;
                FileLanguage.RawText = Source.Text + Data.Text;
            }
            else
                throw new NotImplementedException();

            Raiseupdated(TextBox, new FunctionEventArgs(TextBox.Text));
        }

        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            grid.IsAnimated = false;
            grid.Collapse();
            grid.IsCollapsed = true;
            grid.IsAnimated = true;

        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
