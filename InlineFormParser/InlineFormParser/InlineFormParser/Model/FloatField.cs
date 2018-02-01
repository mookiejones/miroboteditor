#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:08 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class FloatField : NumericFieldBase<double>
	{
		private const string defaultFormatString = "0.######";

		public FloatField()
			: base(1.0)
		{
		}

		[XmlAttribute("Format")]
		[Description("The format string for displaying the value")]
		public string FormatString { get; set; } = "0.######";

		public override PopupKeyboardTypes KeyboardType
		{
			get
			{
				var popupKeyboardTypes = base.KeyboardType;
				if (Minimum >= 0.0) popupKeyboardTypes |= PopupKeyboardTypes.NumericOptionNoSign;
				return popupKeyboardTypes;
			}
		}

		public override string InputPattern => !(Minimum < 0.0) ? "^[0-9]*\\.?[0-9]*$" : "^-?[0-9]*\\.?[0-9]*$";

		protected override string FormatValue(double value)
		{
			return value.ToString(FormatString, CultureInfo.InvariantCulture);
		}

		protected override bool TryParseInput(string input, out double value)
		{
			return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
		}

		public override void Increment()
		{
			CurrentValue = Math.Min(CurrentValue + Step, Maximum);
		}

		public override void Decrement()
		{
			CurrentValue = Math.Max(CurrentValue - Step, Minimum);
		}
	}
}