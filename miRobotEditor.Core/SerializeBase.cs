using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
namespace miRobotEditor.Core
{
    public abstract class SerializeBase : DependencyObject
    {

        public abstract string SerializeFileName { get; set; }

        public void Serialize() { }
        public void Deserialize() { }

    }
}
