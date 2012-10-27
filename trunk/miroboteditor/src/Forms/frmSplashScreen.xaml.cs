using System.Windows;
using System.Windows.Media;
using System.Threading;
namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmSplashScreen.xaml
    /// </summary>
    public partial class FrmSplashScreen : Window
    {
          delegate void StringParameterDelegate(string text);
        delegate void StringParameterWithStatusDelegate(string text, TypeOfMessage tom);
        delegate void SplashShowCloseDelegate();
        delegate void UpdateProgressDelegate(int value);

        /// <summary>
        /// To ensure splash screen is closed using the API and not by keyboard or any other things
        /// </summary>
        bool _closeSplashScreenFlag = false;

        /// <summary>
        /// Base constructor
        /// </summary>
        public FrmSplashScreen()
        {
            InitializeComponent();
//            label1.BackColor = Color.Transparent;
            label1.Foreground = Brushes.Green;

            //progressBar1.Parent = pictureBox1;
            //progressBar1.BackColor = Color.Transparent;

//            progressBar1.Show();
        }
        public int Maximum
        {
            get { return (int)progressBar1.Maximum; }
            set { progressBar1.Maximum = value; }
        }
        public int Minimum
        {
            get { return (int)progressBar1.Minimum; }
            set { progressBar1.Minimum = value; }
        }
        public void UpdateProgress(int value)
        {
             if (Dispatcher.Thread == Thread.CurrentThread)
             {
                 progressBar1.Value = value;
             }
             else
             {
                 Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateProgressDelegate(UpdateProgress), new object[] { value });    
             }




        }
        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public void ShowSplashScreen()
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                Show();
                //Application.Run(this);
            }
            else
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new SplashShowCloseDelegate(ShowSplashScreen));
            }
          
          
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public void CloseSplashScreen()
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                _closeSplashScreenFlag = true;
                Close();
            }
            else
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new SplashShowCloseDelegate(CloseSplashScreen));
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="text">Status Text </param>
        public void UpdateStatusText(string text)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                // Must be on the UI thread if we've got this far
                label1.Foreground = Brushes.Green;
                label1.Text = text;
            }
            else
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new StringParameterDelegate(UpdateStatusText), new object[] { text });
            }
        }


        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="tom">Type of Message</param>
        public void UpdateStatusTextWithStatus(string text, TypeOfMessage tom)
        {          
            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                // Must be on the UI thread if we've got this far
                switch (tom)
                {
                    case TypeOfMessage.Error:
                        label1.Foreground = Brushes.Red;
                        break;
                    case TypeOfMessage.Warning:
                        label1.Foreground = Brushes.Yellow;
                        break;
                    case TypeOfMessage.Success:
                        label1.Foreground = Brushes.Green;
                        break;
                }
                label1.Text = text;
            }
            else
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new StringParameterWithStatusDelegate(UpdateStatusTextWithStatus), new object[] { text, tom });
            }

        }

        /// <summary>
        /// Prevents the closing of form other than by calling the CloseSplashScreen function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplashScreen_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_closeSplashScreenFlag == false)
                e.Cancel = true;
        }
    }
    /// <summary>
    /// Defined types of messages: Success/Warning/Error.
    /// </summary>
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Information,
        Error,
    }
    /// <summary>
    /// Initiate instance of SplashScreen
    /// </summary>
    public static class SplashScreen
    {
        static FrmSplashScreen sf = null;

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public static void ShowSplashScreen()
        {
            if (sf == null)
            {
                sf = new FrmSplashScreen();
                sf.ShowSplashScreen();
            }
        }
        public static void UpdateProgress(int value)
        {
            if (sf != null)
            {
                sf.UpdateProgress(value);
            }
        }
        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public static void CloseSplashScreen()
        {
            if (sf != null)
            {
                sf.CloseSplashScreen();
                sf = null;
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="text">Message</param>
        public static void UpdateStatusText(string text)
        {
            if (sf != null)
                sf.UpdateStatusText(text);
        }

        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="tom">Type of Message</param>
        public static void UpdateStatusTextWithStatus(string text, TypeOfMessage tom)
        {

            if (sf != null)
                sf.UpdateStatusTextWithStatus(text, tom);
        }
    }
    }

