using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace miRobotEditor.Classes
{
    public static class Global
    {
        #region Image Paths

        public const string DockConfigPath = "dockConfig.xml";
        public const string LogFile = "logFile.txt";
        public const string ImgError = "..\\..\\Images\\resources-error.png";
        public const string ImgInfo = "..\\..\\Images\\resources-info.png";
        public const string IconObjectBrowser = "pack://application:,,/Resources/resources-objectbrowser.png";
        public const string IconProperty = "pack://application:,,/Resources/property-blue.png";
        public const string ImgConst = "..\\..\\Images\\resources-vxconstant_icon.png";
        public const string ImgStruct = "..\\..\\Images\\resources-vxstruct_icon.png";
        public const string ImgMethod = "..\\..\\Images\\resources-vxmethod_icon.png";
        public const string ImgEnum = "..\\..\\Images\\resources-vxenum_icon.png";
        public const string ImgField = "..\\..\\Images\\resources-vxfield_icon.png";
        public const string ImgValue = "..\\..\\Images\\resources-vxvaluetype_icon.png";
        public const string ImgSignal = "..\\..\\Images\\resources-vxevent_icon.png";
        public const string ImgXyz = "..\\..\\Images\\resources-vxXYZ_icon.png";
        public const string ImgSrc = "..\\..\\Images\\resources-srcfile.png";
        public const string ImgDat = "..\\..\\Images\\resources-datfile.png";
        public const string ImgSps = "..\\..\\Images\\resources-spsfile.png";

        #endregion Image Paths

        public static string StartupPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string Version
        {
            get
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                return executingAssembly.GetName().Version.ToString();
            }
        }

        public static string ProductName => Assembly.GetExecutingAssembly().GetName().ToString();

        private static string GetDockConfig()
        {
            string result = AppDomain.CurrentDomain.BaseDirectory + "dockConfig.xml";
            return result;
        }
        [Localizable(false)]
        public static string DockConfig => GetDockConfig();

        public static bool DoesDirectoryExist(string filename)
        {
            FileInfo fileInfo = new(filename);
            bool result;
            if (fileInfo.DirectoryName != null)
            {
                DirectoryInfo directoryInfo = new(fileInfo.DirectoryName);
                try
                {
                    if (Directory.GetDirectories(directoryInfo.Root.ToString()).Length > 0)
                    {
                        result = true;
                        return result;
                    }
                }
                catch
                {
                    result = false;
                    return result;
                }
            }
            result = false;
            return result;
        }


    }
}