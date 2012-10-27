using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using DataGrid = System.Windows.Controls.DataGrid;
using UserControl = System.Windows.Controls.UserControl;

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
            DataContext = new ObjectBrowserModel();
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


        private void OpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void DataGrid_ContextMenuOpening_1(object sender, ContextMenuEventArgs e)
        {

        }

        private void OnMouseDouble_Click(object sender, MouseButtonEventArgs e)
        {
            // Open The File
            var cell = sender as DataGrid;

            if (cell == null | !cell.CurrentCell.IsValid) return;
                var item = cell.CurrentCell.Item as IVariable ;

            if (item != null) MainWindow.Instance.OpenFile(item);
        }

        #region
        private string GetDirectory()
        {
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Root Directory for Instance";
            fbd.SelectedPath = "C:\\work\\backups\\tampa";

            var result = fbd.ShowDialog();

            return (result == DialogResult.OK) ? fbd.SelectedPath : string.Empty;
        }
     
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


      
    }
}