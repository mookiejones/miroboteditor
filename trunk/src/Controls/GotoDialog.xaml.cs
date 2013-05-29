using System.Windows;
using System.Windows.Media;
using System.Drawing;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for GotoDialog.xaml
    /// </summary>
    public partial class GotoDialog 
    {
       

        public GotoDialog()
        {
                InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
                switch (btn.Content.ToString())
                {
                    case "_OK":
                        DialogResult = true;
                        break;
                    case "_Cancel":
                        DialogResult = false;
                        break;
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
