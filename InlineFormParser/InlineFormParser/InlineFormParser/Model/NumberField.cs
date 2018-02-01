#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:51 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Globalization;

#endregion

namespace InlineFormParser.Model
{
	public class NumberField : NumericFieldBase<int>
	{
		public NumberField()
			: base(1)
		{
			Maximum = 2147483647;
		}

		public override PopupKeyboardTypes KeyboardType
		{
			get
			{
				var popupKeyboardTypes = base.KeyboardType | PopupKeyboardTypes.NumericOptionNoDot;
				if (Minimum >= 0) popupKeyboardTypes |= PopupKeyboardTypes.NumericOptionNoSign;
				return popupKeyboardTypes;
			}
		}

		public override string InputPattern
		{
			get
			{
				if (Minimum >= 0) return "^([0-9][0-9]*|0)$";
				return "^([0-9|-][0-9]*|0)$";
			}
		}

		protected override string FormatValue(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		protected override bool TryParseInput(string input, out int value)
		{
			return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
		}

		public override void Increment()
		{
			CurrentValue = Math.Min(CurrentValue + Step, Maximum);
		}

		public override void Decrement()
		{
			CurrentValue = Math.Max(CurrentValue - Step, Minimum);
		}

		private delegate void StringArgDelegate(string arg);
	}
}