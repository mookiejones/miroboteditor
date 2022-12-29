using System.Collections.Generic;

namespace RobotEditor.Core.Languages
{
    public abstract class AbstractFoldingStrategy
    {
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            var newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        protected abstract IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOfffset);
    }
}
