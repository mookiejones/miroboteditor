﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using miRobotEditor.Controls;
using Color = System.Windows.Media.Color;

namespace miRobotEditor.Classes
{
    public class Function :ViewModelBase, IVariable
    {
        public string Scope { get; set; }
        public string Returns { get; set; }
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int Offset { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }

        public List<Function> GetFunctions(string filename)
        {

            var result = new List<Function>();
            BitmapImage icon = Utilities.LoadBitmap(Global.imgMethod);
            var m = VariableHelper.FindMatches(DummyDoc.ActiveEditor.FileLanguage.MethodRegex, filename);

            while (m.Success)
            {
                var f = new Function
                            {
                                Offset = m.Index,
                                Icon = icon,
                                Path = filename,
                                Type = m.Groups[1].ToString(),
                                Name = m.Groups[2].ToString()
                            };
                result.Add(f);
                m = m.NextMatch();
            }

            return result;
        }

       
    }
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
                }
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
            logWriter.WriteLog(message, showmessage ? Colors.Red : Colors.Gray);

            if (showmessage)
                MessageBox.Show(message);
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
        /// <param name="Message"></param>
        [Localizable(false), System.Diagnostics.DebuggerStepThrough]
        public static void Trace(string Message)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("{0} : {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), Message));
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
        /// <summary>
        /// Load Bitmap and Convert to Bitmap Image
        /// </summary>
        /// <param name="fileName">Filename of Image</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmap(string fileName)
        {
            // TODO: add some kind of cache to avoid reloading the image whenever the
            // VisualLine is reconstructed
            try
            {

                if (File.Exists(fileName))
                {
                    var bitmap = new BitmapImage(new Uri(fileName));
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
    /// <summary>
    /// Class for Writing To LogFile
    /// </summary>
    public sealed class logWriter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Localizable(false)]
        public logWriter()
        {
            // Delete Current log
            File.Delete(String.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"));
            WriteLog(String.Format("{0} {1} Created", Application.ProductName, Application.ProductVersion));
        }
        /// <summary>
        /// Write to Log
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ex">Exception</param>
        public static void WriteLog(string message, Exception ex)
        {
            WriteLog(message, Colors.Red);
            WriteLog(ex.Message, Colors.Red);
        }
        /// <summary>
        /// Write to Log
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="color">Color Text</param>
        [Localizable(false)]
        public static void WriteLog(string message, Color color)
        {
            using (var stream = new FileStream(string.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"), FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("{0}:={1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), message);

                }
            }
        }

        /// <summary>
        /// Write To Log
        /// </summary>
        /// <param name="message">Message</param>
        public static void WriteLog(string message)
        {
            WriteLog(message, Colors.Gray);
        }



    }
}