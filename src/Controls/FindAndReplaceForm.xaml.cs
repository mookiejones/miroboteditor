using System.Text.RegularExpressions;
using System.Windows;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for FindAndREplaceForm.xaml
    /// </summary>
    public partial class FindAndReplaceForm : Window
    {
        private static FindAndReplaceForm _instance;
        public static FindAndReplaceForm Instance
        {
            get { return _instance ?? (_instance = new FindAndReplaceForm()); }
            set { _instance = value; }
        }

      

        #region Properties
     

        public Regex RegexPattern
        {
            get
            {
                var pattern = UseRegex.IsChecked == false ? Regex.Escape(LookFor) : LookFor;
                int options = MatchCase.IsChecked == true? 0 :  1;
                return new Regex(pattern,(RegexOptions)options);
            }
        }

        public string RegexString
        {
            get
            {
                if (UseRegex.IsChecked == false)
                    return Regex.Escape(LookFor);
                return LookFor;
            }
        }
        public string LookFor { get; set; }
        public string ReplaceWith { get; set; }
        public string SearchResult { get; set; }
        #endregion

        public FindAndReplaceForm()
        {
            InitializeComponent();           
            DataContext = this;
        }
        
        private void FindPrevious(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void FindNext(object sender, RoutedEventArgs e)
        {

            DummyDoc.Instance.TextBox.FindText();
        }

        private void Replace(object sender, RoutedEventArgs e)
        {
            DummyDoc.Instance.TextBox.ReplaceText();
        }

        private void ReplaceAll(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void HighlightAll(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void FindAll(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

    }
}
