using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using miRobotEditor.GUI;
using miRobotEditor.Commands;
using System.Windows.Input;
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
            switch (btn.Content.ToString())
            {
                case "_OK":
                    this.DialogResult = true;
                    break;
                case "_Cancel":
                    this.DialogResult = false;
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
