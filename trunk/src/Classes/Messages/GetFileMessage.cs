using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miRobotEditor.Classes.Messages
{
    class GetFileMessage
    {

        private Action _callbackAction;
        public GetFileMessage(string title, string filter, bool something, Action callback)
        {
            _callbackAction = callback;


        }

        public void Callback(GetFileMessage msg)
        {
            throw new NotImplementedException();
        }

        public void GetFile(MainWindow mainWindow)
        {
            throw new NotImplementedException();
        }

        public bool Result { get; set; }

        public IEnumerable<string> FileNames { get; set; }
    }
}
