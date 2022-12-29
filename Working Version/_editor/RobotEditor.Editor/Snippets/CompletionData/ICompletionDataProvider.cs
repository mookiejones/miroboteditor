using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace RobotEditor.Editor.Snippets.CompletionData
{
    public interface ICompletionDataProvider : IDisposable
    {
        IEnumerable<ICompletionData> ProvideData(CompletionContextInfo context);
    }
}