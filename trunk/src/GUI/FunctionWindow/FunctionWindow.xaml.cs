using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI
{
    /// <summary>
    /// Interaction logic for FunctionPanel.xaml
    /// </summary>
    public partial class FunctionWindow : UserControl
    {

        private static FunctionWindow _instance =new FunctionWindow();
        public static FunctionWindow Instance { get ; set; }
        public FunctionWindow()
        {
            InitializeComponent();
            this.DataContext = new FunctionWindowViewModel();
            Instance = this;
        }

        #region Properties

        public bool FormattingEnabled { get; set; }

        #endregion

        

      
        internal sealed class FunctionInfo
        {
            public string Values { get; private set; }
            public Regex MatchString { get; private set; }

            public FunctionInfo(string values, Regex r)
            {
                MatchString = r;
                Values = values;
            }
        }

        private void ListBoxMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {//
         // for (var i = 0; i < liItems.Items.Count; i++)
         // {
         //     var lbi = liItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
         //     if (lbi == null) continue;
         //     if (IsMouseOverTarget(lbi, e.GetPosition(lbi)))
         //     {
         //         break;
         //     }
         // }
        }

        private static bool IsMouseOverTarget(Visual target, Point point)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(point);
        }

        private void DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           // if ((liItems.SelectedItem == null) | (DummyDoc.Instance == null))
           //     return;
           // var item = liItems.SelectedItem as FunctionItem;
           // if (item != null)
           // {
           //     DummyDoc.Instance.TextBox.SelectText(item.Text);
           //
           //     var d = DummyDoc.Instance.TextBox.Document.GetLineByOffset(item.Offset);
           //     DummyDoc.Instance.TextBox.JumpTo(d.LineNumber, 0);
           // }

        }


        private void VisiblityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                FunctionWindowViewModel.Instance.Timer.Start();
            else
                FunctionWindowViewModel.Instance.Timer.Stop();
        }


        private void gMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
         
            var i = (FunctionItem)((ListViewItem)sender).Content;
            DummyDoc.Instance.TextBox.SelectText(i.Text);
            DummyDoc.Instance.TextBox.JumpTo(i);
        }
    }

    public class FunctionItems:INotifyPropertyChanged
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public ObservableCollection<string> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
