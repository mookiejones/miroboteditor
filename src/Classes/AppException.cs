using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public class AppException:IException
    {
        public Exception Exception { get; set; }
    }
}
