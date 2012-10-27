using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Interaction logic for Activation.xaml
    /// </summary>
    public partial class Activation : Window
    {
        private readonly WebBrowser _browser = new WebBrowser();

        public Activation()
        {
            InitializeComponent();
        }

        internal virtual void A(WebBrowser browser)
        {

            if (_browser != null)
            {
            }
        }

        private bool IsEmailValid(string email)
        {
            if ((email.IndexOf("@", System.StringComparison.Ordinal) > 0) && (email.IndexOf(".", System.StringComparison.Ordinal) > 0))
            {
                return true;
            }
            MessageBox.Show("Email Address not Valid");
            return false;
        }
    }
}
