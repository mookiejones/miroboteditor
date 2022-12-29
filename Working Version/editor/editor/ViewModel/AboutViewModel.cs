using System;
using System.Globalization;
using System.Reflection;
using System.Text; 

namespace miRobotEditor.ViewModel
{
    public sealed class AboutViewModel : ViewModelBase
    {
        public static string Copyright => "Copyright 2012-2013 Charles Berman";

        public static string Name => Assembly.GetExecutingAssembly().GetName().Name;

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static string NetVersion => Environment.Version.ToString();

        private static string OSVersion => Environment.OSVersion.ToString();

        private static string CurrentCulture => CultureInfo.CurrentCulture.Name;

        private static string WorkingSetMemory => GC.GetTotalMemory(true).ToString(CultureInfo.InvariantCulture);

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