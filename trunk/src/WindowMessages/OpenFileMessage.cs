using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miRobotEditor.WindowMessages
{
   public class OpenFileMessage
    {
        public string FileName { get; set; }
        
        #region · Constructor ·
        public OpenFileMessage(string filename)
       {
           FileName = filename;
       }
        #endregion
    
            
    }
}
