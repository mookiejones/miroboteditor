using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using miRobotEditor.Classes;
using miRobotEditor.Controls;

namespace miRobotEditor.GUI.FunctionWindow
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

        

        public void UpdateFunctions(object Data)
        {
            var fi = Data as FunctionInfo;
            if (fi != null) UpdateFunctions(fi.Values, fi.Items);
        }

        public object SelectedItem
        {
            get
            {
                if (liItems.SelectedItem != null)
                    return liItems.SelectedItem;
                     return null;
            }
        }

        public void Clear()
        {
            liItems.Items.Clear();
            InvalidateVisual();
        }

        public void UpdateFunctions(String values, Collection<string> items)
        {
            if ((liItems.Items != null))
            {
                Clear();
                foreach (string t in items)
                {
                    AddItems(getMatches(values, t));
                }
            }
            liItems.InvalidateVisual();
            InvalidateVisual();
        }

        private void AddItems(Collection<FunctionItem> value)
        {
            for (var i = 0; i <= (value.Count - 1); i++)
            {
                liItems.Items.Add(value[i]);
            }
        }

        private static Collection<FunctionItem> getMatches(string Text, string matchstring)
        {

            var Result = new Collection<FunctionItem>();

            var r = new Regex(matchstring, RegexOptions.IgnoreCase);
            var m = r.Match(Text);
            while (m.Success)
            {
                var g = m.Groups[1];
                var text = g.ToString();
                var returns = m.Groups[3].ToString();
                var name = m.Groups[4].ToString();
                var parameters = m.Groups[5].ToString();
                Result.Add(new FunctionItem(text, name, returns, parameters,m.Index));
                m = m.NextMatch();
            }
            return Result;
        }

      

        private FunctionItem currentItem;


        

        internal System.Threading.ParameterizedThreadStart UpdateFunctions()
        {
            throw new NotImplementedException();
        }

        private void liItems_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (currentItem != null)
                liItems.ToolTip = currentItem.Tooltip;
        }

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            currentItem = liItems.SelectedItem as FunctionItem;
        }

        internal sealed class FunctionInfo
        {
            public string Values { get; private set; }
            public Collection<string> Items { get; private set; }

            public FunctionInfo(string values, Collection<string> items)
            {
                Items = items;
                Values = values;
            }
        }

        private void ListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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
