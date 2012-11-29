using System.Collections.Generic;
using System.Collections.ObjectModel;
using miRobotEditor.Controls;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.GUI;

namespace miRobotEditor.Languages
{
    public interface IRobot
    {
        /// <summary>
        /// Source _file String
        /// </summary>
        string SourceFile { get; }
        string DataFile { get; }

        System.Windows.Forms.ToolStripItem[] AddMenuItems();

        bool DataFileExists { get; }
        string DataFileName { get; }
        string ConfigName { get; }
        string ContextPromptLexem { get; }
        Collection<string> FunctionItems { get; }
        string GetFile(string fileName, string filetype);
        AbstractFoldingStrategy FoldingStrategy { get; set; }
        System.IO.Stream Intellisense { get; }
        ShiftClass shiftProgram(DummyDoc doc, Forms.FrmShift shift);
        IList<ICompletionData> CodeCollection { get; }
    }
}
