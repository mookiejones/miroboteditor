using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using DataGrid = System.Windows.Controls.DataGrid;
using UserControl = System.Windows.Controls.UserControl;

namespace miRobotEditor.GUI.ObjectBrowser
{
    /// <summary>
    /// Interaction logic for ObjectBrowserTool.xaml
    /// </summary>
    public partial class ObjectBrowserTool : UserControl
    {
       
    	public static ObjectBrowserTool _instance;
    	public static ObjectBrowserTool Instance
    	{
    		get
    		{
    			if (_instance==null)
    				_instance = new ObjectBrowserTool();
    			return _instance;
    		}
    		set{_instance=value;}
    	}

        public ObjectBrowserTool()
        {
            InitializeComponent();           
			Instance=this;
        }



        
       
        /// <summary>
        /// Find Out What Can and Cannot Be shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
         // _VisibleTabs = 0;
         // foreach (TabItem t in this.TabControl.Items)
         // {
         //     if (t.Content is DataGrid)
         //         t.Visibility = (t.Content as DataGrid).Items.Count < 2 ? Visibility.Collapsed : Visibility.Visible;
         //
         //     if (t.Content is ListView)
         //         t.Visibility = (t.Content as ListView).Items.Count < 2 ? Visibility.Collapsed : Visibility.Visible;
         //
         //     _VisibleTabs = t.Visibility == Visibility.Visible ? _VisibleTabs++ : _VisibleTabs;
         // }
        }


      

     
        #region
        private string GetDirectory()
        {
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Root Directory for Instance";

            if (DummyDoc.Instance.File.Directory != null && DummyDoc.Instance.File.Directory.Exists)
            fbd.SelectedPath = DummyDoc.Instance.File.DirectoryName;

            var result = fbd.ShowDialog();

            return (result == DialogResult.OK) ? fbd.SelectedPath : string.Empty;
        }

        [Localizable(false)]
        public void ShowWizard(string menuselection)
        {
            string dir = GetDirectory();
            if (!Directory.Exists(dir))
            {
                OutputMessages.Add("Directory not valid", String.Format("{0} Doesnt Exist or None Selected", dir), null);
                return;
            }

            DataContext = new ObjectBrowserModel(dir);
            //            VariableHelper.PositionView = CollectionViewSource.GetDefaultView(VariableHelper.Positions);
            //          VariableHelper.PositionCollection = new CollectionView(VariableHelper.Positions);
            //        VariableHelper.PositionCollection.GroupDescriptions.Add(new PropertyGroupDescription("Path"));
        }
        #endregion

        private void ON_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //search the object hierarchy for a datagrid row
            var source = (DependencyObject)e.OriginalSource;
            var row = TryFindParent<DataGridRow>(source);

            //the user did not click on a row
            if (row == null) return;
            // Open The File
            var cell = sender as DataGrid;

            if (cell == null | !cell.CurrentCell.IsValid) return;
            var item = cell.CurrentCell.Item as IVariable;

            if (item != null) MainWindow.Instance.OpenFile(item);
            e.Handled = true;
        }
        // <summary>
        /// Finds a parent of a given item on the visual tree.
        /// 
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public  T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            //use recursion to proceed with next level
            return TryFindParent<T>(parentObject);
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
 
    }
}
