using System;
using miRobotEditor.Model;

namespace miRobotEditor.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            DataItem item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}