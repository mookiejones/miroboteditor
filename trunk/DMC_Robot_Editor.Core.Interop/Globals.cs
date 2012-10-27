using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Forms;
using Globals;
using miRobotEditor;

/// <summary>
/// Global Variables
/// </summary>
public static class Global
    {
    /// <summary>
    /// XML Configuration File For Docking Manager
    /// </summary>
        public const string dockConfig = "dockConfig.xml";

    /// <summary>
    /// Used to help prevent from freezing when network directory doesnt exist
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>  
    public static bool DoesDirectoryExist(string filename)
        {
            var f = new FileInfo(filename);
        var d = new DirectoryInfo(f.DirectoryName);

        try
        {
            if (System.IO.Directory.GetDirectories(d.Root.ToString()).Length > 0)
                return true;
        }
        catch (Exception)
        {
        }
        return false;
    }

//TODO Remove these when you get the resource problem figured out

    //TODO Change this to an XML File
    /// <summary>
    /// Log File
    /// </summary>
    public const string logFile = "logFile.txt";

    /// <summary>
    /// Constant Variable Image
    /// </summary>
    public const string imgConst = @"C:\Programming\Images\programicons\vxconstant_icon.png";
    /// <summary>
    /// Struct Variable Image
    /// </summary>
    public const string imgStruct = @"C:\Programming\Images\programicons\vxstruct_icon.png";
    /// <summary>
    /// Method Variable Image
    /// </summary>
    public const string imgMethod = @"C:\Programming\Images\programicons\vxmethod_icon.png";
    /// <summary>
    /// Enum Variable Image
    /// </summary>
    public const string imgEnum = @"C:\Programming\Images\programicons\vxenum_icon.png";
    /// <summary>
    /// Field Variable Image
    /// </summary>
    public const string imgField = @"C:\Programming\Images\programicons\vxfield_icon.png";
    /// <summary>
    /// Value Variable Image
    /// </summary>
    public const string imgValue = @"C:\Programming\Images\programicons\vxvaluetype_icon.png";
    /// <summary>
    /// Signal Variable Image
    /// </summary>
    public const string imgSignal = @"C:\Programming\Images\programicons\vxevent_icon.png";
    /// <summary>
    /// XYZ Position Variable Image
    /// </summary>
    public const string imgXYZ = @"C:\Programming\Images\programicons\vxXYZ_icon.png";
    /// <summary>
    /// Source File Image
    /// </summary>
    public const string imgSRC = @"C:\Programming\Images\programicons\srcfile.png";
    /// <summary>
    /// Dat File Image
    /// </summary>
    public const string imgDAT = @"C:\Programming\Images\programicons\datfile.png";
    /// <summary>
    /// SPS File Image
    /// </summary>
    public const string imgSPS = @"C:\Programming\Images\programicons\spsfile.png";

    /// <summary>
    /// Write To Log
    /// </summary>
    /// <param name="message"></param>
    public static void WriteLog(string message)
        {
            logWriter.WriteLog(message);
        }

    /// <summary>
    /// Handle Error
    /// </summary>
    /// <param name="message"></param>
        public static void ErrorHandler(string message)
        {
            ErrorHandler(message, false);
        }

    /// <summary>
    /// Error Handler
    /// </summary>
    /// <param name="message"></param>
    /// <param name="showmessage"></param>
        public static void ErrorHandler(string message, bool showmessage)
        {
            Console.WriteLine(message);
            TraceWriter.Trace(message);
            logWriter.WriteLog(message, showmessage?Colors.Red:Colors.Gray);

            if (showmessage)
                MessageBox.Show(message);
        }
    }
