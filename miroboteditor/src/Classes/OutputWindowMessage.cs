using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class OutputMessages 
    {
        public static ObservableCollection<IOutputMessages> Messages { get { return _messages; } set { _messages = value;} }
        private static ObservableCollection<IOutputMessages> _messages = new ObservableCollection<IOutputMessages>(); 
        
        public static void Add(string title, string description, BitmapImage icon)
        {
            Messages.Add(new OutputWindowMessage { Title = title, Icon = icon, Description = description });
        }

    }
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
    public interface IOutputMessages
    {
        OutputWindowMessage Add(string title, string description, BitmapImage icon);
        BitmapImage Icon { get; set; }
        string Time { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}
