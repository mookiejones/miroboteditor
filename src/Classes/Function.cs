using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using miRobotEditor.GUI;
using miRobotEditor.ViewModel;
using Color = System.Windows.Media.Color;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Global Variables
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// XML Configuration File For Docking Manager
        /// </summary>
        public const string _DockConfig =  "dockConfig.xml";

        public static string DockConfig {get{ return Environment.CurrentDirectory + "\\" + _DockConfig;}}
        /// <summary>
        /// Used to help prevent from freezing when network directory doesnt exist
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>  
        public static bool DoesDirectoryExist(string filename)
        {
            var f = new FileInfo(filename);
            if (f.DirectoryName != null)
            {
                var d = new DirectoryInfo(f.DirectoryName);

                try
                {
                    if (Directory.GetDirectories(d.Root.ToString()).Length > 0)
                        return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }


    
        /// <summary>
        /// Log File
        /// </summary>
        public const string LogFile = "logFile.txt";

        /// <summary>
        /// Constant Error Image
        /// </summary>
        public const string ImgError = "..\\..\\Resources\\error.png";

        /// <summary>
        /// Constant Error Info
        /// <remarks>
        /// Used with Message Window</remarks>
        /// </summary>
        public const string ImgInfo = "..\\..\\Resources\\info.png";

        /// <summary>
        /// Constant ObjectBrowser Icon
        /// </summary>
        public const string IconObjectBrowser = "pack://application:,,/Resources/objectbrowser.png";

        /// <summary>
        /// Constant ObjectBrowser Icon
        /// </summary>
        public const string IconProperty = "pack://application:,,/Resources/property-blue.png";

        /// <summary>
        /// Constant Variable Image
        /// </summary>
        public const string ImgConst = "..\\..\\Resources\\vxconstant_icon.png";
        /// <summary>
        /// Struct Variable Image
        /// </summary>
        public const string ImgStruct = "..\\..\\Resources\\vxstruct_icon.png";
        /// <summary>
        /// Method Variable Image
        /// </summary>
        public const string ImgMethod = "..\\..\\Resources\\vxmethod_icon.png";
        /// <summary>
        /// Enum Variable Image
        /// </summary>
        public const string ImgEnum = "..\\..\\Resources\\vxenum_icon.png";
        /// <summary>
        /// Field Variable Image
        /// </summary>
        public const string ImgField = "..\\..\\Resources\\vxfield_icon.png";
        /// <summary>
        /// Value Variable Image
        /// </summary>
        public const string ImgValue = "..\\..\\Resources\\vxvaluetype_icon.png";
        /// <summary>
        /// Signal Variable Image
        /// </summary>
        public const string ImgSignal = "..\\..\\Resources\\vxevent_icon.png";
        /// <summary>
        /// XYZ Position Variable Image
        /// </summary>
        public const string ImgXyz = "..\\..\\Resources\\vxXYZ_icon.png";
        /// <summary>
        /// Source File Image
        /// </summary>
        public const string ImgSrc = "..\\..\\Resources\\srcfile.png";
        /// <summary>
        /// Dat File Image
        /// </summary>
        public const string ImgDat = "..\\..\\Resources\\datfile.png";
        /// <summary>
        /// SPS File Image
        /// </summary>
        public const string ImgSps = "..\\..\\Resources\\spsfile.png";

        /// <summary>
        /// Write To Log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            LogWriter.WriteLog(message);
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
        	LogWriter.WriteLog(message,showmessage?Colors.Red:Colors.Gray);
        	
        	if (showmessage)
            MessageViewModel.ShowMessage(message);
        }
        	
    }
    /// <summary>
    /// TraceWriter
    /// </summary>
    public static class TraceWriter
    {
        /// <summary>
        /// Write Trace Message
        /// </summary>
        /// <param name="message"></param>
        [Localizable(false), System.Diagnostics.DebuggerStepThrough]
        public static void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("{0} : {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), message));
        }
    }
    public static class Utilities
    {
        /// <summary>
        /// Load Bitmap and Convert to BitmapImage
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static BitmapImage LoadBitmap(Bitmap img)
        {
        	
            var result = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                result.BeginInit();
                result.StreamSource = new MemoryStream(ms.ToArray());
                result.EndInit();
            }
            return result;
        }



        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this Image source)
        {
            var bitmap = new Bitmap(source);

            var bitSrc = bitmap.ToBitmapSource();

            bitmap.Dispose();

            return bitSrc;
        }

        public static ImageSource GetIcon(string fileName)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(fileName);
            bi.EndInit();
            return bi;
        }

        /// <summary>
        /// Load Bitmap and Convert to Bitmap Image
        /// </summary>
        /// <param name="fileName">Filename of Image</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmap(string fileName)
        {
            try
            {

                if (File.Exists(fileName))
                {
                    var fi = new FileInfo(fileName);
                    var bitmap = new BitmapImage(new Uri(fi.FullName));
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (ArgumentException)
            {
                // invalid filename syntax
            }
            catch (IOException)
            {
                // other IO error
            }
            return null;
        }

    }
    
  
}
