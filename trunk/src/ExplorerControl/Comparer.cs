using System;
using System.Collections;
using System.IO;

namespace miRobotEditor.GUI.ExplorerControl
{
    internal class Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var directoryInfo = (DirectoryInfo) x;
            var directoryInfo2 = (DirectoryInfo) y;
            string name = directoryInfo.Name;
            string name2 = directoryInfo2.Name;
            return String.CompareOrdinal(name, name2);
        }
    }
}