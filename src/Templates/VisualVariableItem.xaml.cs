using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using miRobotEditor.Classes;

namespace miRobotEditor.Templates
{
    /// <summary>
    /// Interaction logic for VisualVariableItem.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class VisualVariableItem : DataGrid
// ReSharper restore RedundantExtendsListEntry
    {
        public VisualVariableItem()
        {
            InitializeComponent();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //search the object hierarchy for a datagrid row
            var source = (DependencyObject)e.OriginalSource;
            var row = TryFindParent<DataGridRow>(source);

            //the user did not click on a row
            if (row == null) return;
            // Open The File
            var cell = sender as DataGrid;

            if (cell != null && false | !cell.CurrentCell.IsValid) return;
            if (cell != null)
            {
                var item = cell.CurrentCell.Item as IVariable;

                if ((item != null)&&(File.Exists(item.Path))) Workspace.Instance.OpenFile(item);
            }
            e.Handled = true;
        }
        public T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);
            //use recursion to proceed with next level
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
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                var parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        private void ToolTip_Opening(object sender, ToolTipEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
