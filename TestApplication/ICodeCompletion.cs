using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
namespace TestApplication
{
   public interface ICodeCompletion
    {
        string Description{get; set;}
        string Title { get;  set; }
        ImageSource Icon { get;  set; }
    }
}
