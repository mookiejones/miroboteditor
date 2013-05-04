using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVE6POS
{
   public class E6Position:ViewModelBase
    {
        private string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }

        private string _x = string.Empty;
        public string X { get { return _x; } set { _x = value; RaisePropertyChanged("X"); } }

        private string _y = string.Empty;
        public string Y { get { return _y; } set { _y = value; RaisePropertyChanged("Y"); } }

        private string _z = string.Empty;
        public string Z { get { return _z; } set { _z = value; RaisePropertyChanged("Z"); } }

        private string _a = string.Empty;
        public string A { get { return _a; } set { _a = value; RaisePropertyChanged("A"); } }

        private string _b = string.Empty;
        public string B { get { return _b; } set { _b = value; RaisePropertyChanged("B"); } }

        private string _c = string.Empty;
        public string C { get { return _c; } set { _c = value; RaisePropertyChanged("C"); } }

        private string _apo = string.Empty;
        public string APO { get { return _apo; } set { _apo = value; RaisePropertyChanged("APO"); } }

    }
}
