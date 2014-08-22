using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using miRobotEditor.Annotations;

namespace miRobotEditor.Forms
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Helper to show or close given splash window3
    /// </summary>
    public static class Splasher
    {
        public static Window Splash { get; set; }

        /// <summary>
        ///     Show splash screen
        /// </summary>
        public static void ShowSplash()
        {
            if (Splash != null)
            {
                Splash.Show();
            }
        }

        /// <summary>
        ///     Close splash screen
        /// </summary>
        public static void CloseSplash()
        {
            if (Splash == null) return;
            Splash.Close();
        }
    }

    /// <summary>
    ///     Message listener, singlton pattern.
    ///     Inherit from DependencyObject to implement DataBinding.
    /// </summary>
    public class MessageListener : DependencyObject
    {
        /// <summary>
        /// </summary>
        private static MessageListener _listener;

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof (string), typeof (MessageListener),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        private MessageListener()
        {
        }

        /// <summary>
        ///     Get MessageListener instance
        /// </summary>
        public static MessageListener Instance
        {
            get { return _listener ?? (_listener = new MessageListener()); }
        }

        /// <summary>
        ///     Get or set received message
        /// </summary>
        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(string message)
        {
            Message = message;
            Debug.WriteLine(Message);
            DispatcherHelper.DoEvents();
        }
    }

    /// <summary>
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        ///     Simulate Application.DoEvents function of <see cref=" System.Windows.Forms.Application" /> class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary />
        /// <returns />
        private static object ExitFrames([NotNull] object frame)
        {
            if (frame == null) throw new ArgumentNullException("frame");
            ((DispatcherFrame) frame).Continue = false;

            return null;
        }
    }
}