using System;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace miRobotEditor.Classes
{
  public  class FileWatcher : FileSystemWatcher 
    {
      public static bool ReloadFile(string filename)
      {
          string message = String.Format("{0}\r\n has been changed.\r\n\r\nWould you like to reload the file?", filename);
          var result = MessageBox.Show(message, "File has Changed, Reload File?", MessageBoxButton.YesNo, MessageBoxImage.Question);

          return result == MessageBoxResult.Yes;
      }
    }
}
