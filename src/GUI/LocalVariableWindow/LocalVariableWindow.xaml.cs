using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.GUI
{

    /// <summary>
    /// Interaction logic for LocalVariableWindow.xaml
    /// </summary>
    public partial class LocalVariableWindow : UserControl 
    {

    	private static LocalVariableWindow _instance = new LocalVariableWindow();
        public static LocalVariableWindow Instance { get { return _instance ?? new LocalVariableWindow(); } }

        public LocalVariableWindow()
        {
            InitializeComponent();
        }
    }
}
