using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace InlineFormParser.Model
{
	public class CommandCallArgument : AdeComponentConfigurationListItem
	{
		[Description("The argument name")]
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlText]
		[Description("The argument value as a string")]
		public string ValueString { get; set; }

		public CommandCallArgument()
		{
		}

		public CommandCallArgument(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			Name = name;
			ValueString = string.Empty;
		}

		public override void Validate()
		{
			base.Validate();
			ConfigurationHelpers.CheckStringProperty(this, "Name");
			ConfigurationHelpers.CheckPropertyNotNull(this, "ValueString");
			IsValid = true;
		}
	}
}