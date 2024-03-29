﻿using System.Windows.Controls;
using miRobotEditor.Controls.TextEditor.Snippets;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for SnippetToolTip.xaml
    /// </summary>
    public sealed partial class SnippetToolTip : UserControl
    {
        public string Author { get; set; }
        public string Description { get; set; }
        public string Shortcuts { get; set; }
        public string Title { get; set; }

        public SnippetToolTip(SnippetInfo snippetInfo)
        {
            InitializeComponent();
            DataContext = this;
        }

        public SnippetToolTip()
        {
            InitializeComponent();
        }
    }
}