using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.GUI.ObjectBrowser
{
    /// <summary>
    /// Interaction logic for frmObjectBrowser.xaml
    /// </summary>
    public partial class frmObjectBrowser : UserControl
    {
        public frmObjectBrowser()
        {
            InitializeComponent();
            UpdateVisibility();
            DataContext = this;
        }

        /// <summary>
        /// Force Update of visible items.
        /// </summary>
        public Visibility UpdateVisibility()
        {
            VisibilityChanged(null,new DependencyPropertyChangedEventArgs());
            return _VisibleTabs > 0 ? Visibility.Visible : Visibility.Collapsed;
        }


        private int _VisibleTabs;

        /// <summary>
        /// Find Out What Can and Cannot Be shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           _VisibleTabs = 0;
           foreach (TabItem t in this.TabControl.Items)
           {
               if (t.Content is DataGrid)
                   t.Visibility = (t.Content as DataGrid).Items.Count < 2 ? Visibility.Collapsed : Visibility.Visible;

               if (t.Content is ListView)
                   t.Visibility = (t.Content as ListView).Items.Count < 2 ? Visibility.Collapsed : Visibility.Visible;

               _VisibleTabs = t.Visibility == Visibility.Visible ? _VisibleTabs++ : _VisibleTabs;
           }
        }

       
    }
}