using CommunityToolkit.Mvvm.DependencyInjection;
using miRobotEditor.Properties;
using miRobotEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace miRobotEditor
{
    internal static class AppCommands
    {
        static void OpenFile(string filename)
        {
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.Open(filename);
        }

        private static void OpenFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
                OpenFile(file);
        }
        internal  static void ProcessArgs()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            OpenFiles(commandLineArgs);
 
        }
        internal static void LoadOpenFiles()
    {
            var files = Regex.Split(Settings.Default.OpenDocuments, ";")
                .Where(File.Exists);

            
            OpenFiles(files);
 
               
    }
    }
}
