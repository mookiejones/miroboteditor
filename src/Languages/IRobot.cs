using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Languages
{
    public interface IRobot
    {
        /// <summary>
        /// Source _file String
        /// </summary>
        string SourceFile { get; }
        string DataFile { get; }

        bool DataFileExists { get; }
        string DataFileName { get; }
        string ConfigName { get; }
        string ContextPromptLexem { get; }
        Collection<string> FunctionItems { get; }
        string GetFile(string fileName, string filetype);

        //TODO Was XmlFoldingStrategy
        XmlFoldingStrategy FoldingStrategy { get; set; }
        System.IO.Stream Intellisense { get; }
        ShiftClass ShiftProgram(IDocument doc, ShiftViewModel shift);
        IList<ICompletionData> CodeCollection { get; }
    }
}
