using System.Windows.Controls;
namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for CustomToolTip.xaml
    /// </summary>
    public partial class CustomToolTip : ToolTip 
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Additional { get; set; }
        public CustomToolTip()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
