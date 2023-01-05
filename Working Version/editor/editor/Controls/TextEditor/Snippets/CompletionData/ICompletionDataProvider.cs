using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace miRobotEditor.Controls.TextEditor.Snippets.CompletionData
{
    public interface ICompletionDataProvider : IDisposable
    {
        IEnumerable<ICompletionData> ProvideData(CompletionContextInfo context);
    }
}