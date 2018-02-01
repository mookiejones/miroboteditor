#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:09 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public abstract class LimitedStringFieldBase : StringFieldBase
	{
		protected string inputPattern;
		protected int maxLength;

		protected LimitedStringFieldBase()
		{
			MaxLength = 2147483647;
		}

		[Description("The maximal text length in characters")]
		[XmlAttribute("MaxLength")]
		public int MaxLength
		{
			get => maxLength;
			set => maxLength = value;
		}

		[XmlIgnore]
		public override string InputPattern => inputPattern;

		[XmlIgnore]
		public override string InvalidInputMessageText
		{
			get
			{
				if (CurrentInputState == InputState.TooLong) return GetInvalidInputMessageTextTooLong(maxLength);
				return base.InvalidInputMessageText;
			}
		}

		public override string FieldInfoText
		{
			get
			{
				var stringBuilder = new StringBuilder(base.FieldInfoText);
				if (maxLength >= 1)
				{
					if (stringBuilder.Length > 0) stringBuilder.AppendLine();
					stringBuilder.Append(Root.Resolver.GetString(this, "FrmMaxLength", maxLength));
				}

				return stringBuilder.ToString();
			}
		}

		protected override InputState ValidateInput(string value)
		{
			if (maxLength >= 1 && value.Length > maxLength) return InputState.TooLong;
			return base.ValidateInput(value);
		}
	}
}