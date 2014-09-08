using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Core
{
    public class ErrorMessage:IMessage
    {
        public ErrorMessage(string title, Exception exception)
        {
            Title = title;
            Description = exception.ToString();
        }
        public BitmapImage Icon { get; private set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
