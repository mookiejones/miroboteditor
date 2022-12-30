using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace miRobotEditor.Controls.TextEditor.SyntaxHighlighting
{
    internal sealed  class SyntaxHighlightingHelper
    {
        public   string GetFilename(string name)=> $"{GetType().Namespace}.{name}Highlight.xshd";
       

        public static SyntaxHighlightingHelper Create()
        {
            return new SyntaxHighlightingHelper();
        }
    }
}
