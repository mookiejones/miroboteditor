using System.Collections.Generic;
using System.Runtime.Serialization;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    using Global;
    [DataContract(Namespace = "KUKARoboter.Contracts")]
    public class MessageInfo : IExtensibleDataObject
    {
        [DataMember]
        public int Code { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }

        [DataMember]
        public Dictionary<string, string> ItemList { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> MessageArguments { get; set; }

        [DataMember]
        public string MessageKey { get; set; }

        [DataMember]
        public string MessageResource { get; set; }

        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public ExceptionSource Source { get; set; }

        public MessageInfo()
        {
            Source = ExceptionSource.Unknown;
            ItemList = new Dictionary<string, string>();
        }
        public MessageInfo([CanBeUnknown] ExceptionSource source, [CanBeNegative, CanBeZero] int code)
        {
            Source = source;
            Code = code;
            ItemList = new Dictionary<string, string>();
        }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Message))
            {
                return Message;
            }
            return MessageKey;
        }
    }
}

