using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI.OutputWindow
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class OutputWindow : UserControl
    {
        /// <summary>
        /// Adds A Message to the Output Window
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public void Add(string title, string description)
        {
           Add(null,title,description);
        }
        /// <summary>
        /// Adds a Message to the output window
        /// </summary>
        /// <param name="Icon"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public void Add(BitmapImage Icon,string title, string description)
        {
            var msg = new OutputWindowMessage();
            msg.Icon = Icon;
            msg.Title = title;
            msg.Description = description;
            OutputMessages.Messages.Add(msg);
        }
        public OutputWindow()
        {           
            DataContext = this;
        }

        private void ClearItems(object sender, RoutedEventArgs e)
        {
           OutputMessages.Messages.Clear();
        }
    }
}
