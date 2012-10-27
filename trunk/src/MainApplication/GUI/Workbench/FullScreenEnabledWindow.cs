using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using miRobotEditor.Core;

namespace MainApplication.GUI.Workbench
{
    class FullScreenEnabledWindow : Window
    {
        public static readonly DependencyProperty FullScreenProperty =
            DependencyProperty.Register("FullScreen", typeof(bool), typeof(FullScreenEnabledWindow));

        public bool FullScreen
        {
            get { return (bool)GetValue(FullScreenProperty); }
            set { SetValue(FullScreenProperty, value); }
        }

        WindowState previousWindowState = WindowState.Maximized;
        double oldLeft, oldTop, oldWidth, oldHeight;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FullScreenProperty)
            {
                if ((bool)e.NewValue)
                {
                    // enable fullscreen mode
                    // remember previous window state
                    if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                        previousWindowState = WindowState;
                    oldLeft = Left;
                    oldTop = Top;
                    oldWidth = Width;
                    oldHeight = Height;

                    var interop = new WindowInteropHelper(this);
                    interop.EnsureHandle();
                    var screen = Screen.FromHandle(interop.Handle);

                    var bounds = screen.Bounds.ToWpf().TransformFromDevice(this);

                    ResizeMode = ResizeMode.NoResize;
                    Left = bounds.Left;
                    Top = bounds.Top;
                    Width = bounds.Width;
                    Height = bounds.Height;
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;

                }
                else
                {
                    ClearValue(WindowStyleProperty);
                    ClearValue(ResizeModeProperty);
                    ClearValue(MaxWidthProperty);
                    ClearValue(MaxHeightProperty);
                    WindowState = previousWindowState;

                    Left = oldLeft;
                    Top = oldTop;
                    Width = oldWidth;
                    Height = oldHeight;
                }
            }
        }
    }
}
