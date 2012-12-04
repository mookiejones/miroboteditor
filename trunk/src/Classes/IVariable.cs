using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public interface IVariable
    {
        bool IsSelected { get; set; }
        BitmapImage Icon { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        string Path { get; set; }
        string Value { get; set; }
        string Comment { get; set; }
        string Declaration{get;set;}
        int Offset { get; set; }

    }
}
