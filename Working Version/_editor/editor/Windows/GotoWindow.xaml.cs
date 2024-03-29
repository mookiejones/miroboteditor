﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Windows
{
    /// <summary>
    ///     Interaction logic for GotoWindow.xaml
    /// </summary>
    public sealed partial class GotoWindow
    {
        public GotoWindow()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var text = button.Content.ToString();
                if (!String.IsNullOrEmpty(text))
                {
                    if (text != "_OK")
                    {
                        if (text == "_Cancel")
                        {
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        DialogResult = true;
                    }
                }
            }
        }
    }
}