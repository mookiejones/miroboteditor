using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace miRobotEditor.Controls.TextEditor.Folding
{
    public abstract class AbstractFoldingStrategy
    {
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out int firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        protected abstract IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOfffset);
    }
}