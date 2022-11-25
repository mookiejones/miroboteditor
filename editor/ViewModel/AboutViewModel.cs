using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public sealed class AboutViewModel : ViewModelBase
    {
        public static string Copyright
        {
            get { return "Copyright 2012-2013 Charles Berman"; }
        }

        public static string Name
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        private static string NetVersion
        {
            get { return Environment.Version.ToString(); }
        }

        private static string OSVersion
        {
            get { return Environment.OSVersion.ToString(); }
        }

        private static string CurrentCulture
        {
            get { return CultureInfo.CurrentCulture.Name; }
        }

        private static string WorkingSetMemory
        {
            get { return GC.GetTotalMemory(true).ToString(CultureInfo.InvariantCulture); }
        }

        public string Info
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(" Name                : " + Name);
                stringBuilder.AppendLine(" .Net Version        : " + NetVersion);
                stringBuilder.AppendLine(" OS Version          : " + OSVersion);
                stringBuilder.AppendLine(" Current Culture     : " + CurrentCulture);
                stringBuilder.AppendLine(" Working Set Memory  : " + WorkingSetMemory);
                return stringBuilder.ToString();
            }
        }
    }
}