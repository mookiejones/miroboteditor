using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using miRobotEditor.GUI.Editor;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for GotoDialog.xaml
    /// </summary>
    public partial class GotoDialog 
    {
       
        public Editor Editor { get; set; }

        public GotoDialog()
        {
                InitializeComponent();
        }

       

        
        private int _selectedLine;
        

        private void cancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
               if ( int.TryParse(EnteredText.Text, out _selectedLine))
                {

                    var d = Editor.Document.GetLineByNumber(_selectedLine);
                    Editor.CaretOffset = d.Offset;
                    Editor.TextArea.Caret.BringCaretToView();
                    Editor.ScrollToLine(_selectedLine);
                }
            }
            finally
            {
                Close();
            }
        }


        private void GotoDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Description.Text = String.Format("Enter Line Number (1-{0}):", Editor.LineCount);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
