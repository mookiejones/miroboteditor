using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using miRobotEditor.Core.Commands;
using miRobotEditor.Resources;

namespace miRobotEditor.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for GotoDialog.xaml
    /// </summary>
    public partial class GotoDialog : Window
    {
      
        /// <summary>
        /// Goto Line
        /// </summary>
        /// <param name="linecount">count of lines in active editor</param>
        public GotoDialog(int linecount = 0)
        {
            this.DataContext = this;

            InitializeComponent();
            LineCount = linecount;
        }



        #region CloseCommand    



        private RelayCommand<string> _closeCommand;

        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand<string> CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand<string>(ExecuteCloseCommand));
            }
        }

        private void ExecuteCloseCommand(string parameter)
        {

            switch (parameter)
                {
                    case "_OK":
                        DialogResult = true;
                        break;
                    case "_Cancel":
                        DialogResult = false;
                        break;
                }
        }
        #endregion



        #region OkCommand
        private RelayCommand _okCommand;

        /// <summary>
        /// Gets the OkCommand.
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(ExecuteOkCommand));
            }
        }

        private void ExecuteOkCommand(object obj)
        {
            throw new NotImplementedException();
        }


        #endregion

        public int LineCount
        {
            get { return (int)GetValue(LineCountProperty); }
            set { SetValue(LineCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineCountProperty =
            DependencyProperty.Register("LineCount", typeof(int), typeof(GotoDialog), new PropertyMetadata(0));

        
    }
}
