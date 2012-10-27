using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class OutputWindowMessage:IOutputMessages
    {

        public string Time { get { return DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture); } }
        public string Title { get;  set; }
        public string Description { get;  set; }
        public BitmapImage Icon { get; set; }

        public OutputWindowMessage Add(string title, string description, BitmapImage icon)
        {
            return new OutputWindowMessage {Title=title,Icon=icon,Description=description};
        }
       


    }
}
