using System;
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

        public string FileName { get; set; }

        public Editor Source
        {
            get { return source; }
        }

        public Editor Data
        {
            get { return data; }
        }


        public Editor TextBox { get; set; }

        #endregion


        public DummyDoc()
        {
            Instance = this;

            InitializeComponent();            
            ShowGrid = false;
            FileLanguage = new LanguageBase();
            DataContext = this;
        }

        
        public void Close()
        {
        }

        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:

                        Data.Load(Path.GetDirectoryName(FileName) + "\\" + FileLanguage.DataName);
                        Data.Visibility = Visibility.Visible;
                        grid.Visibility = Visibility.Visible;
                        Grid.SetRow(grid, 1);
                        Grid.SetRow(data, 2);
                        break;
                    case false:
                        Data.Visibility = Visibility.Collapsed;
                        grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
     
        public void CanReload(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = File.Exists(FileName);
        }

        public  void ExecuteReload(object sender, ExecutedRoutedEventArgs e)
        {
            Load(FileName);           
        }

        public void Load(string filename)
        {
            FileName = filename;

            FileLanguage = AbstractLanguageClass.GetRobotType(FileName);
            grid.IsAnimated = false;
            //TODO Set Icon For File

            //TODO Get Files Based in language 

            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.SourceText)) && (FileLanguage.SourceName != null))
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Source.Load(Path.GetDirectoryName(FileName) + "\\" + FileLanguage.SourceName)));
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Source.Load(FileName)));
            if ((FileLanguage is Languages.KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
                Dispatcher.Invoke(DispatcherPriority.Normal,new Action(() => Data.Load(Path.GetDirectoryName(FileName) + "\\" + FileLanguage.DataName)));
            }
            else
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = false));
            }

            // Select Original File
            TextBox = source.FileName == filename ? source : data;
            grid.IsAnimated = true;
           
        }

        private void ReloadFile(object sender, EventArgs e)
        {
            // Dispatcher is used because of the FileChanged Events
            if (!String.IsNullOrEmpty(Source.FileName))
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Source.Reload()));
            // Dispatcher is used because of the FileChanged Events
            if (!String.IsNullOrEmpty(data.FileName))
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

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            Instance = this;
            if (!(sender is Editor)) return;
            TextBox = sender as Editor;
            FileLanguage=AbstractLanguageClass.GetRobotType(FileName);
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
