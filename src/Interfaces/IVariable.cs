using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Interfaces
{
    public interface IVariable
    {
        bool IsSelected { get; set; }
        BitmapImage Icon { get; set; }
        string Name { get; set; }

        [Localizable(false)]
        string Type { get; set; }

        string Path { get; set; }
        string Value { get; set; }
        string Comment { get; set; }
        string Declaration { get; set; }
        string Description { get; set; }
        int Offset { get; set; }
    }
}
