using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Controls
{
    /// <summary>
    ///     Interaction logic for KUKAKFDDialog.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class KUKAKFDDialog : Window
// ReSharper restore RedundantExtendsListEntry
    {
        public KUKAKFDDialog()
        {
            InitializeComponent();
        }

        public int Num { get; private set; }


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
            var button = sender as Button;
            if (button != null) Num = Convert.ToInt32(button.Tag);
        }
    }
}