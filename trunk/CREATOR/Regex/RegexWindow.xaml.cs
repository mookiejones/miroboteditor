using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
namespace CREATOR.Regex
{
    /// <summary>
    /// Interaction logic for RegexWindow.xaml
    /// </summary>
    public partial class RegexWindow : Window
    {

        private class CustomTabCommand:ICommand
        {
            private ICommand command;
            public CustomTabCommand(object sender,ICommand c)
            {
                command = c;
            }
            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return command.CanExecute(parameter);
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                command.Execute(parameter);
            }

            #endregion
        }

        private RoutedCommand command;
        void SetupTabSnippetHandler()
        {
            var editingKeyBindings = text.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
            command = new RoutedCommand("dd", typeof(KeyBinding));
            var tabBinding = editingKeyBindings.Single(b => b.Key == Key.Tab && b.Modifiers == ModifierKeys.None);
            text.TextArea.DefaultInputHandler.Editing.InputBindings.Remove(tabBinding);
            var newTabBinding = new KeyBinding(new CustomTabCommand(this, tabBinding.Command), tabBinding.Key,tabBinding.Modifiers);
            text.TextArea.DefaultInputHandler.Editing.InputBindings.Add(newTabBinding);
        }


         

        public string OriginalText { get; set; }

        public RegexWindow()
        {
            InitializeComponent();
            SetupTabSnippetHandler();
            text.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(text.TextArea));
            this.DataContext = this;
            text.TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
        }

        void TextArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextViewPosition? position = text.GetPositionFromPoint(e.GetPosition(this));
            if (position.HasValue) 
            {
                text.TextArea.Caret.Position = position.Value;
            }
        }




        void SearchText(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            System.Text.RegularExpressions.Regex r;
            var sb = new StringBuilder();
            try
            {
                r = new System.Text.RegularExpressions.Regex(Expression.Text, (int)RegexOptions.IgnoreCase + RegexOptions.Multiline);
            Match m = r.Match(OriginalText);

            if ((m.Success)&&(!String.IsNullOrEmpty(Expression.Text.Trim())))
            {
                while (m.Success)
                {
                    for (int i = 1; i < m.Groups.Count;i++ )
                    {
                        sb.AppendLine(m.Groups[i].ToString());
                    }
                      
                    m = m.NextMatch();
                }
            }
            else
            {
                sb.Append(OriginalText);
            }

            }
            catch (System.ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
                sb.Append(OriginalText);
            }
            
            //Disable Saving the Changes
            text.TextChanged-= new EventHandler(text_TextChanged);
            text.IsReadOnly = true;

            text.Text = sb.ToString();

            //Disable Saving the Changes
             text.TextChanged+= new EventHandler(text_TextChanged);
           text.IsReadOnly = false;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
			OriginalText = text.Text;
        }


        private void SaveText(object sender, KeyEventArgs e)
        {
            OriginalText = text.Text;
        }
        

    }
}