using System;
using System.Collections;
using System.IO;

namespace miRobotEditor.ExplorerControl
{
    class Comparer:IComparer
    {
        public int Compare(object x, object y)
        {
            var directoryInfo = (DirectoryInfo)x;
            var directoryInfo2 = (DirectoryInfo)y;
            var name = directoryInfo.Name;
            var name2 = directoryInfo2.Name;
            return String.CompareOrdinal(name, name2);
        }
    }
}