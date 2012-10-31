using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class OutputMessages 
    {
        public static ObservableCollection<IOutputMessages> Messages { get { return _messages; } set { _messages = value;} }
        private static ObservableCollection<IOutputMessages> _messages = new ObservableCollection<IOutputMessages>();

        [Localizable(false)]
        public static void Add(string title, string description, BitmapImage icon )
        {
            try
            {
                Messages.Add(new OutputWindowMessage {Title = title, Icon = icon, Description = description});
            }
            catch
            {
            }
        }

        [Localizable(false)]
        public static void AddError(Exception ex)
        {
#if DEBUG
            var sb = new StringBuilder();
            var st = new StackTrace();
            string stackIndent = "";
            
            for (int i = 0; i < st.FrameCount; )
            {
                try
                {
                    // Note that at this level, there are four 
                    // stack frames, one for each method invocation.
                    StackFrame sf = st.GetFrame(i);
                    sb.AppendLine();
                    sb.AppendFormat(stackIndent + " Method: {0}", sf.GetMethod());
                    sb.AppendFormat(stackIndent + " File: {0}", sf.GetFileName());
                    sb.AppendFormat(stackIndent + " Line Number: {0}", sf.GetFileLineNumber());
                    stackIndent += "  ";
                   
                                                                 Messages.Add(new OutputWindowMessage
                                                                                  {
                                                                                      Title = "Error",
                                                                                      Description = sb.ToString(),
                                                                                      Icon = null

                                                                                  });
                                                                 using (var writer = new StreamWriter(Global.logFile, true))
                                                                 {
                                                                     writer.Write(sb.ToString());
                                                                 }

                }
                catch
                {
                }
                return;
            }
            
#endif
           
        }

    }
}