using System;
using System.Windows.Media;
using System.Windows.Forms;
using Globals;
using miRobotEditor;

public static class Global
    {
        public const string dockConfig = "dockConfig.xml";

//TODO Remove these when you get the resource problem figured out
    public const string imgConst = @"C:\Programming\Images\programicons\vxconstant_icon.png";
    public const string imgStruct = @"C:\Programming\Images\programicons\vxstruct_icon.png";
    public const string imgMethod = @"C:\Programming\Images\programicons\vxmethod_icon.png";
    public const string imgEnum = @"C:\Programming\Images\programicons\vxenum_icon.png";
    public const string imgField = @"C:\Programming\Images\programicons\vxfield_icon.png";
    public const string imgValue = @"C:\Programming\Images\programicons\vxvaluetype_icon.png";
    public const string imgSignal = @"C:\Programming\Images\programicons\vxevent_icon.png";
    public const string imgXYZ = @"C:\Programming\Images\programicons\vxXYZ_icon.png";
    public const string imgSRC = @"C:\Programming\Images\programicons\srcfile.png";
    public const string imgDAT = @"C:\Programming\Images\programicons\datfile.png";
    public const string imgSPS = @"C:\Programming\Images\programicons\spsfile.png";

    public static void WriteLog(string message)
        {
            logWriter.WriteLog(message);
        }

        public static void ErrorHandler(string message)
        {
            ErrorHandler(message, false);
        }

        public static void ErrorHandler(string message, bool showmessage)
        {
            Console.WriteLine(message);
            TraceWriter.Trace(message);
            logWriter.WriteLog(message, showmessage?Colors.Red:Colors.Gray);

            if (showmessage)
                MessageBox.Show(message);
        }
    }
