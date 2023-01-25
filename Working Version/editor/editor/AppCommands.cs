using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.DependencyInjection;
using miRobotEditor.Properties;
using miRobotEditor.ViewModel;

namespace miRobotEditor
{
    internal static class AppCommands
    {
        private static void OpenFile(string filename)
        {
            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            _ = instance.Open(filename);
        }

        private static void OpenFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                OpenFile(file);
            }
        }
        internal static void ProcessArgs()
        {
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1);
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
