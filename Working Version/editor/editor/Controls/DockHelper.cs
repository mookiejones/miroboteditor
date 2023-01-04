using AvalonDock;
using AvalonDock.Layout.Serialization;
using miRobotEditor.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace miRobotEditor.Controls
{
    internal static class DockHelper
    {
        

        internal static void LoadLayout(this DockingManager dockingManager)
        {
            if (!File.Exists(Global.DockConfig)) return;
                var xmlLayoutSerializer = new XmlLayoutSerializer(dockingManager);
                using (new StreamReader(Global.DockConfig))
                    xmlLayoutSerializer.Deserialize(Global.DockConfig);

        }
        internal static void SaveLayout(this DockingManager dockingManager)
        {
            var xmlLayoutSerializer = new XmlLayoutSerializer(dockingManager);
            using (var streamWriter = new StreamWriter(Global.DockConfig))
                xmlLayoutSerializer.Serialize(streamWriter);
        }
    }
}
