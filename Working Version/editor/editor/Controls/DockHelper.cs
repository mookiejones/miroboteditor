using System.IO;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using miRobotEditor.Classes;

namespace miRobotEditor.Controls
{
    internal static class DockHelper
    {


        internal static void LoadLayout(this DockingManager dockingManager)
        {
            if (!File.Exists(Global.DockConfig))
            {
                return;
            }

            XmlLayoutSerializer xmlLayoutSerializer = new XmlLayoutSerializer(dockingManager);
            using (new StreamReader(Global.DockConfig))
            {
                xmlLayoutSerializer.Deserialize(Global.DockConfig);
            }
        }
        internal static void SaveLayout(this DockingManager dockingManager)
        {
            XmlLayoutSerializer xmlLayoutSerializer = new XmlLayoutSerializer(dockingManager);
            using (StreamWriter streamWriter = new StreamWriter(Global.DockConfig))
            {
                xmlLayoutSerializer.Serialize(streamWriter);
            }
        }
    }
}
