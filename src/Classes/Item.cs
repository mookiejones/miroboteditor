using System;
using GalaSoft.MvvmLight;

namespace miRobotEditor.Classes
{
    public sealed class Item : ViewModelBase
    {
        public Item(string type, string description)
        {
            Type = type;
            Description = description;
        }


        public int Index { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return String.Format("{0};{1}", Type, Description);
        }
    }
}