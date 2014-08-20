using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace miRobotEditor.Interfaces
{
   public abstract class AbstractFoldingStrategy
    {

       public void UpdateFoldings(FoldingManager manager, TextDocument document)
       {
            int firstErrorOffset;
                IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
                manager.UpdateFoldings(newFoldings, firstErrorOffset);
       }

       public abstract IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOfffset);
      public  abstract IEnumerable<NewFolding> CreateNewFoldings(ITextSource document);
    }
}
