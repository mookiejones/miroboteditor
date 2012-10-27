using System;
using System.Runtime.Serialization;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    using Global;
    [DataContract(Namespace = "KUKARoboter.Contracts")]
    public class MessageItem : IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData
        {
            get;
            set;
        }
        [DataMember]
        public string Key
        {
            get;
            set;
        }
        [DataMember]
        public string Value
        {
            get;
            set;
        }
        public MessageItem([CanBeEmpty] string key, [CanBeEmpty] string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Key = key;
            Value = value;
        }
    }
}
