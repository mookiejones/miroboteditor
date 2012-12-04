using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class Variable:IVariable
    {
        public bool IsSelected { get; set; }
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
        public int Offset { get; set; }
        public string Comment { get; set; }
		public string Declaration { get; set; }
    }
}
