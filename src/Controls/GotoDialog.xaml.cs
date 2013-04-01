using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using miRobotEditor.GUI;

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
        

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(EnteredText.Text, out _selectedLine)) return;

                var d = Editor.Document.GetLineByNumber(_selectedLine);
                Editor.CaretOffset = d.Offset;
                Editor.TextArea.Caret.BringCaretToView();
                Editor.ScrollToLine(_selectedLine);
            }
            finally
            {
                Close();
            }
        }


        private void GotoDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Description.Text = String.Format(Properties.Resources.GotoDialog_GotoDialog_OnLoaded_Enter_Line_Number__1__0___, Editor.LineCount);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
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
