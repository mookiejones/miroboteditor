using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class OutputMessages 
    {
        public static ObservableCollection<IOutputMessages> Messages { get { return _messages; } set { _messages = value;} }
        private static ObservableCollection<IOutputMessages> _messages = new ObservableCollection<IOutputMessages>(); 
        
        public static void Add(string title, string description, BitmapImage icon )
        {

            Messages.Add(new OutputWindowMessage { Title = title, Icon = icon, Description = description });
        }
        public static void AddError(Exception ex)
        {
#if DEBUG
            Messages.Add(new OutputWindowMessage ());
            
#endif
        }

    }
}