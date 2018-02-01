using System.ComponentModel;
using System.Xml.Serialization;

namespace InlineFormParser.Model
{
	public abstract class AdeComponentConfiguration
	{
		[Browsable(false)]
		[XmlIgnore]
		public bool IsValid { get; protected set; } = true;

		[Browsable(false)]
		[XmlIgnore]
		public string ErrorMessage { get; protected set; } = string.Empty;

		public override string ToString()
		{
			return GetType().Name;
		}

		public virtual void Validate()
		{
		}
	}
}