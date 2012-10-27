using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using miRobotEditor.Robot;

namespace miRobotEditor.GUI.ExplorerWindow
{
    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    public partial class ExplorerWindow:UserControl,INotifyPropertyChanged 
    {
        #region Private Members
        private string copyPath = string.Empty;
        private Robot.AbstractRobotClass _robot = new RobotBase();
        #endregion

        #region Public Properties
            public string SelectedPath { get { return tbSelectedPath.Text; } }
        public Robot.AbstractRobotClass Robot
        {
            get { return _robot; } 
            set
            {
                _robot = value;
                NotifyPropertyChanged("Robot");
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

        #region Window Commands
        // Executed event handler. 
        private void CopyFile(object sender, ExecutedRoutedEventArgs e)
        {
            // Calls a method to close the file and release resources.
            copyPath =SelectedPath;
        }

        // CanExecute event handler. 
        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SelectedPath))
            {

                // Call a method to determine if there is a file open. 
                // If there is a file open, then set CanExecute to true. 
                var fi = new FileInfo(tbSelectedPath.Text);
                e.CanExecute = fi.Exists;
            }
        }
        // Executed event handler. 
        private void PasteFile(object sender, ExecutedRoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SelectedPath))
            {
                var fi = new FileInfo(copyPath);
                string Target = System.IO.Path.Combine(SelectedPath, fi.Name);

                // Does Target Exist
                if (File.Exists(Target)) return;
                // Calls a method to close the file and release resources.
                var di = new DirectoryInfo(SelectedPath);

                if (di.Exists)
                {
                   

                    try
                    {

                        File.Copy(copyPath, System.IO.Path.Combine(SelectedPath, fi.Name));
                    }
                    catch (IOException ex)
                    {
                    	Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        // CanExecute event handler. 
        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {

            if (!String.IsNullOrEmpty(SelectedPath))
            {
                // Call a method to determine if there is a file open. 
                // If there is a file open, then set CanExecute to true. 
                var fi = new FileInfo(tbSelectedPath.Text);
                e.CanExecute = copyPath != String.Empty;
            }
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {

            if (!String.IsNullOrEmpty(SelectedPath))
            {
                var fi = new FileInfo(SelectedPath);
                e.CanExecute = fi.Exists;
            }
        }

        private void DeleteFile(object sender, ExecutedRoutedEventArgs e)
        {
            File.Delete(SelectedPath);
        }

        private void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {

            if (!String.IsNullOrEmpty(SelectedPath))
            {
                var fi = new FileInfo(SelectedPath);
                e.CanExecute = fi.Exists;
            }
        }

        private void OpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            File.Delete(SelectedPath);
        }

        #endregion


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
