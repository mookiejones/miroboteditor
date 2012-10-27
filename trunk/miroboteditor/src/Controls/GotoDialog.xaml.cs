using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for GotoDialog.xaml
    /// </summary>
    public partial class GotoDialog
    {
        private static GotoDialog Instance = null;

        public static void ShowSingleInstance()
        {
            if (Instance == null)
            {
                Instance = new GotoDialog();
                Instance.Show();
            }
            else
            {
                Instance.Activate();
            }
        }

        private class GotoEntry : IComparable<GotoEntry>
        {
            public object Tag;
            public string Text { get; private set; }
            private readonly IImage _image;
            private readonly int _matchType;

            public ImageSource ImageSource
            {
                get { return _image.ImageSource; }
            }

            public GotoEntry(string text, IImage image, int matchType)
            {
                this.Text = text;
                this._image = image;
                this._matchType = matchType;
            }

            public int CompareTo(GotoEntry other)
            {
                var mt = _matchType.CompareTo(other._matchType);
                if (mt != 0)
                    return -mt;
                return System.String.Compare(Text, other.Text, System.StringComparison.Ordinal);
            }




        }

        private readonly Editor _editor;
        public int SelectedLine { get; private set; }

        public GotoDialog()
        {
            InitializeComponent();
            textBox.Focus();
        }

        public GotoDialog(Editor editor)
        {
            if (_editor.LineCount > 0)
            {
                InitializeComponent();
                _editor = editor;
                WriteLabel();
            }
        }

        private void WriteLabel()
        {
       //     lblLineNumber.Text = String.Format("&Line Number (1-{0}):", _editor.LineCount);
        }

        private void txtGotoLine_KeyUp(object sender, KeyEventArgs e)
        {
            SelectedLine = Convert.ToInt32(textBox.Text);
        }

        private void txtBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (listbox.SelectedItem == null)
                return;

            switch (e.Key)
            {

                case Key.Up:
                    e.Handled = true;
                    ChangeIndex(-1);
                    break;
                case Key.Down:
                    e.Handled = true;
                    ChangeIndex(+1);
                    break;
                case Key.PageUp:
                    e.Handled = true;
                    ChangeIndex((int) Math.Round(-listbox.ActualHeight/20));
                    break;
                case Key.PageDown:
                    e.Handled = true;
                    ChangeIndex((int) Math.Round(+listbox.ActualHeight/20));
                    break;
            }
        }

        private void ChangeIndex(int increment)
        {
            int index = listbox.SelectedIndex;
            index = Math.Max(0, Math.Min(listbox.Items.Count - 1, index + increment));
            listbox.SelectedIndex = index;
            listbox.ScrollIntoView(listbox.Items[index]);
        }

        private System.Collections.Generic.HashSet<string> visibleEntries = new HashSet<string>();
        private List<GotoEntry> newItems = new List<GotoEntry>();

        private void Text_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string text = textBox.Text.Trim();
            listbox.ItemsSource = null;
            newItems.Clear();
            visibleEntries.Clear();
            if ((text.Length == 0) | (text.Length == 1 && !char.IsDigit(text, 0)))
                return;

            int commaPos = text.IndexOf(',');
            if (commaPos < 0)
            {
                // use "_file, ##" or "_file: ##" syntax for line numbers
                commaPos = text.IndexOf(':');
            }
            if (char.IsDigit(text, 0))
            {
                ShowLineNumberItem(text);
            }
            else if (commaPos > 0)
            {
                string file = text.Substring(0, commaPos).Trim();
                string line = text.Substring(commaPos + 1).Trim();
                Match m = Regex.Match(line, @"^\w+ (\d+)$");
                if (m.Success)
                {
                    // remove the word "line" (or some localized version of it)
                    line = m.Groups[1].Value;
                }
                int lineNr;
                if (!int.TryParse(line, out lineNr))
                    lineNr = 0;
      //          AddSourceFiles(file, lineNr);
            }
            else
            {
                throw new NotImplementedException();
                // AddSourceFiles(text, 0);
                // foreach (IClass c in SearchClasses(text))
                // {
                //     AddItem(c, GetMatchType(text, c.Name));
                // }
                // AddAllMembersMatchingText(text);
            }
            newItems.Sort();
            this.listbox.ItemsSource = newItems;
            if (newItems.Count > 0)
            {
                listbox.SelectedItem = newItems[0];
            }
        }


      

        private void AddAllMembersMatchingText( string text)
        {
          

          // foreach (string f in _editor.Robot.Fields)
          //     AddItemIfMatchText(text, f);
          //
          // foreach (string m in _editor.Robot.Methods)
          //     AddItemIfMatchText(text, m);


        }


        private void ShowLineNumberItem(string text)
        {
            int num;
            if (int.TryParse(text, out num))
            {
               
                if (_editor != null)
                {
                    num = Math.Min(_editor.Document.Lines.Count, Math.Max(1, num));
                 //   AddItem(StringParser.Parse("${res:Dialog.Goto.GotoLine} ") + num, ClassBrowserIconService.GotoArrow, num, int.MaxValue);
                }
            }
        }


        private const int MatchType_NoMatch = -1;
        private const int MatchType_ContainsMatch_CaseInsensitive = 0;
        private const int MatchType_ContainsMatch = 1;
        private const int MatchType_StartingMatch_CaseInsensitive = 2;
        private const int MatchType_StartingMatch = 3;
        private const int MatchType_FullMatch_CaseInsensitive = 4;
        private const int MatchType_FullMatch = 5;

        private static int GetMatchType(string searchText, string itemText)
        {
            if (itemText.Length < searchText.Length)
                return MatchType_NoMatch;
            int indexInsensitive = itemText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (indexInsensitive < 0)
                return MatchType_NoMatch;
            // This is a case insensitive match
            int indexSensitive = itemText.IndexOf(searchText, StringComparison.Ordinal);
            if (itemText.Length == searchText.Length)
            {
                // this is a full match
                if (indexSensitive == 0)
                    return MatchType_FullMatch;
                else
                    return MatchType_FullMatch_CaseInsensitive;
            }
            else if (indexInsensitive == 0)
            {
                // This is a starting match
                if (indexSensitive == 0)
                    return MatchType_StartingMatch;
                else
                    return MatchType_StartingMatch_CaseInsensitive;
            }
            else
            {
                if (indexSensitive >= 0)
                    return MatchType_ContainsMatch;
                else
                    return MatchType_ContainsMatch_CaseInsensitive;
            }
        }

        private void AddItem(string text, IImage image, object tag, int matchType)
        {
            if (!visibleEntries.Add(text))
                return;
            GotoEntry item = new GotoEntry(text, image, matchType);
            item.Tag = tag;
            newItems.Add(item);
        }


     

        private void cancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listbox.SelectedItem == null)
                    return;
                object tag = ((GotoEntry) listbox.SelectedItem).Tag;
                if (tag is int)
                {
                    _editor.FindText(((GotoEntry) listbox.SelectedItem).Tag.ToString());
                }

            }
            finally
            {
                Close();
            }
        }


    }



    public interface IImage
    {
        /// <summary>
        /// Gets the image as WPF ImageSource.
        /// </summary>
        ImageSource ImageSource { get; }

        Bitmap Bitmap { get; }

        Icon Icon { get; }

    }
}
