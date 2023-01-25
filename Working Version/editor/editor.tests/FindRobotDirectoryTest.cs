using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using miRobotEditor.Controls.FileExplorer;
using miRobotEditor.Controls.TextEditor.Language;
using miRobotEditor.Languages;

namespace editor.tests
{
    [TestClass]
    public class FindRobotDirectoryTest
    {
        const string fileName= "C:\\Temp\\red dwarf\\KRC\\R1\\cell.src";
        [TestMethod]
        public void FindDirectoryTest()
        {
            
            var abstractLanguageClass = new KUKA(fileName);
            abstractLanguageClass.GetRootDirectory();
            RobotDirectoryHelper.FindDirectory(fileName);
        }
    }
}
