using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using miRobotEditor.Languages;

namespace miRobotEditor.GUI.ExplorerWindow
{
    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    public partial class ExplorerWindow:UserControl,INotifyPropertyChanged 
    {
        #region Private Members
        private AbstractLanguageClass _robot = new LanguageBase();
        #endregion

        #region Public Properties
            public string SelectedPath { get { return tbSelectedPath.Text; } }
        public AbstractLanguageClass Robot
        {
            get { return _robot; } 
            set
            {
                _robot = value;
                NotifyPropertyChanged("FileLanguage");
            }
        }
        public FileExplorerViewModel FileExplorerViewModel { get; set; }
        #endregion

        public ExplorerWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            DataContextChanged += FileExplorerDataContextChanged;           
        }
        
        void FileExplorerDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var explorerWindow = DataContext as ExplorerWindow;
            if (explorerWindow != null)
            {
                var vm = explorerWindow.FileExplorerViewModel;

                if (vm != null)
                {
                    vm.Initialize();
                }
            }
        }

        #region Expand, Collapse
        private void ExpandAllNodes(bool expand)
        {
            foreach (object item in directoryView.Items)
            {
                var treeItem = directoryView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    ExpandAll(treeItem, expand);
                    treeItem.IsExpanded = expand;
                }
            }
        }
        private static void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                var childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                var item = childControl as TreeViewItem;
                if (item != null) item.IsExpanded = expand;
            }
        }
        #endregion

        
        #region ShowFiles (for class FileExplorer)
        public bool ShowFiles
        {
            get { return (bool)GetValue(ShowFilesProperty); }
            set
            {
                SetValue(ShowFilesProperty, value);

            }
        }

        public static readonly DependencyProperty ShowFilesProperty = DependencyProperty.Register("ShowFiles", typeof(bool), typeof(ExplorerWindow), new FrameworkPropertyMetadata(true, OnShowFilesChanged));

        public void SetShowFiles()
        {
            var vm = DataContext as FileExplorerViewModel;

            if (vm != null)
            {             
                ExpandAllNodes(false);
            }

        }

        private static void OnShowFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExplorerWindow)d).SetShowFiles();
        }

        #endregion

		
        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            FileExplorerViewModel = new FileExplorerViewModel();
            DataContext = this;          
        }



        private  string _filter;

        public String Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                NotifyPropertyChanged("Filter");
            }
        }

        private void cbFilter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as ExplorerWindow;
            if (vm != null)
            {
                Filter = cbFilter.Text;
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged !=null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}
