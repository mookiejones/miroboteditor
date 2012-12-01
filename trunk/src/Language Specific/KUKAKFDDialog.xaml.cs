using System;
using System.ComponentModel;
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

        public int Num
        {
            get;
            private set;
        }


        [Localizable(false)]
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Width = 592;

            foreach (Button b in GridGroup.Children)
            {
                b.IsEnabled = true;
                b.Visibility = Visibility.Visible;
            }
            foreach (Button b in GridGroup.Children)
            {
                if (String.IsNullOrEmpty(b.Content.ToString()))
                {
                    Width -= 81;
                    b.Visibility = Visibility.Hidden;
                }
                if (b.Content.ToString() == " ")
                    b.IsEnabled = false;
            }
        }

        private void Selected(object sender, RoutedEventArgs e)
        {
            Num = Convert.ToInt32((sender as Button).Tag);
        }
    }
}
