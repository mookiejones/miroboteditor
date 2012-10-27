using System;

namespace miRobotEditor.GUI.ExplorerWindow
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisplayStringAttribute : Attribute
    {
        public string Value { get; private set; }

        public string ResourceKey { get; set; }

        public DisplayStringAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }
    }
}
