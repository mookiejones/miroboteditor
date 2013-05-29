using System.Windows.Media;
using System.Threading;
namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmSplashScreen.xaml
    /// </summary>
    public partial class FrmSplashScreen
    {
        delegate void StringParameterDelegate(string text);
        delegate void StringParameterWithStatusDelegate(string text, TypeOfMessage tom);
        delegate void SplashShowCloseDelegate();
        delegate void UpdateProgressDelegate(int value);

        /// <summary>
        /// To ensure splash screen is closed using the API and not by keyboard or any other things
        /// </summary>
        bool _closeSplashScreenFlag;

        /// <summary>
        /// Base constructor
        /// </summary>
        public FrmSplashScreen()
        {
            InitializeComponent();
//            label1.BackColor = Color.Transparent;
            Label1.Foreground = Brushes.Green;

            //progressBar1.Parent = pictureBox1;
            //progressBar1.BackColor = Color.Transparent;

//            progressBar1.Show();
        }
        public int Maximum
        {
            get { return (int)ProgressBar1.Maximum; }
            set { ProgressBar1.Maximum = value; }
        }
        public int Minimum
        {
            get { return (int)ProgressBar1.Minimum; }
            set { ProgressBar1.Minimum = value; }
        }
        public void UpdateProgress(int value)
        {
             if (Dispatcher.Thread == Thread.CurrentThread)
             {
                 ProgressBar1.Value = value;
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
                Label1.Foreground = Brushes.Green;
                Label1.Text = text;
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
                        Label1.Foreground = Brushes.Red;
                        break;
                    case TypeOfMessage.Warning:
                        Label1.Foreground = Brushes.Yellow;
                        break;
                    case TypeOfMessage.Success:
                        Label1.Foreground = Brushes.Green;
                        break;
                }
                Label1.Text = text;
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
        private void SplashScreenClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_closeSplashScreenFlag == false)
                e.Cancel = true;
        }
    }

    public enum TypeOfMessage
    {
        Status,
        Error,
        Message,
        Success,
        Warning
    }

    public static class SplashScreen
    {

        public static void UpdateProgress(int progress)
        {
        }

        public static void ShowSplashScreen()
        {
            Spl.Show();
            Spl.ShowActivated = true;
        }


        private static readonly FrmSplashScreen Spl= new FrmSplashScreen();
        public static void UpdateStatusTextWithStatus(string statusText,TypeOfMessage msgType)
        {
           Spl.UpdateStatusTextWithStatus(statusText,msgType);

        }

    }
    }

