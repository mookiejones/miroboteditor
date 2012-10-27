using System;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for KUKAKFDDialog.xaml
    /// </summary>
    public partial class KUKAKFDDialog : Window
    {
        public KUKAKFDDialog()
        {
            InitializeComponent();
        }

        public int num
        {
            get;
            private set;
        }
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 592;

            foreach (Button b in GridGroup.Children)
            {
                b.IsEnabled = true;
                b.Visibility = System.Windows.Visibility.Visible;
            }
            foreach (Button b in GridGroup.Children)
            {
                if (String.IsNullOrEmpty(b.Content.ToString()))
                {
                    Width -= 81;
                    b.Visibility = System.Windows.Visibility.Hidden;
                }
                if (b.Content.ToString() == " ")
                    b.IsEnabled = false;
            }
        }

        private void Selected(object sender, RoutedEventArgs e)
        {
            num = Convert.ToInt32((sender as Button).Tag);
        }
    }
}
