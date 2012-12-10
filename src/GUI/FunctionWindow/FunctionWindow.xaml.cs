using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI
{
    /// <summary>
    /// Interaction logic for FunctionPanel.xaml
    /// </summary>
    public partial class FunctionWindow : UserControl
    {
        public FunctionWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Properties

        public bool FormattingEnabled { get; set; }

        #endregion

        

        public void UpdateFunctions(object data)
        {
            var fi = data as FunctionInfo;
            if (fi != null) UpdateFunctions(fi.Values, fi.Items);
        }
        public FunctionItem SelectedItem
        {
            get
            {
                if (liItems.SelectedItem != null)
                    return liItems.SelectedItem as FunctionItem;
                     return null;
            }
        }

        public void Clear()
        {
            liItems.Items.Clear();
            InvalidateVisual();
        }

        public void UpdateFunctions(String values, List<string> items)
        {
            if ((liItems.Items != null))
            {
                Clear();
                foreach (var t in items)
                {
                    AddItems(GetMatches(values, t));
                }
            }
            liItems.InvalidateVisual();
            InvalidateVisual();
        }

        private void AddItems(List<FunctionItem> value)
        {
            for (var i = 0; i <= (value.Count - 1); i++)
            {
                liItems.Items.Add(value[i]);
            }
        }

        private static List<FunctionItem> GetMatches(string text, string matchstring)
        {

            var result = new List<FunctionItem>();

            var r = new Regex(matchstring, RegexOptions.IgnoreCase);
            var m = r.Match(text);
            while (m.Success)
            {
                var g = m.Groups[1];
                var s = g.ToString();
                var returns = m.Groups[3].ToString();
                var name = m.Groups[4].ToString();
                var parameters = m.Groups[5].ToString();
                result.Add(new FunctionItem(s, name, returns, parameters,m.Index));
                m = m.NextMatch();
            }
            return result;
        }

      

        private FunctionItem _currentitem;


        

        internal System.Threading.ParameterizedThreadStart UpdateFunctions()
        {
            throw new NotImplementedException();
        }

        private void LiItemsToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (_currentitem != null)
                liItems.ToolTip = _currentitem.Tooltip;
        }

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentitem = liItems.SelectedItem as FunctionItem;
        }

        internal sealed class FunctionInfo
        {
            public string Values { get; private set; }
            public List<string> Items { get; private set; }

            public FunctionInfo(string values, List<string> items)
            {
                Items = items;
                Values = values;
            }
        }

        private void ListBoxMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            for (var i = 0; i < liItems.Items.Count; i++)
            {
                var lbi = liItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (lbi == null) continue;
                if (IsMouseOverTarget(lbi, e.GetPosition(lbi)))
                {
                    break;
                }
            }
        }

        private static bool IsMouseOverTarget(Visual target, Point point)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(point);
        }

        private void DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((liItems.SelectedItem == null) | (DummyDoc.Instance == null))
                return;
            var item = liItems.SelectedItem as FunctionItem;
            if (item != null)
            {
                DummyDoc.Instance.TextBox.SelectText(item.Text);

                var d = DummyDoc.Instance.TextBox.Document.GetLineByOffset(item.Offset);
                DummyDoc.Instance.TextBox.JumpTo(d.LineNumber, 0);
            }

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
