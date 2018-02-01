#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:49 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Globalization;

#endregion

namespace InlineFormParser.Model
{
	public class BoolField : ValueFieldBase<bool>
	{
		public override bool DisplayLabelInternally => true;

		protected override string FormatValue(bool value)
		{
			return value.ToString(CultureInfo.InvariantCulture).ToUpper();
		}

		protected override bool TryParseInput(string input, out bool value)
		{
			return bool.TryParse(input, out value);
		}
	}
}