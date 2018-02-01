#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:10 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public abstract class StringFieldBase : ValueFieldBase<string>
	{
		protected StringFieldBase()
		{
		}

		protected StringFieldBase(string initialValue)
			: base(initialValue)
		{
		}

		protected override string FormatValue(string value)
		{
			return value;
		}

		protected override bool TryParseInput(string input, out string value)
		{
			value = input.Trim();
			return true;
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			NormalizeString(ref initialValue);
			base.OnCreated(parent, childIndex);
		}

		protected string GetInvalidInputMessageTextTooLong(int maxLength)
		{
			if (string.IsNullOrEmpty(Label)) return Root.Resolver.GetString(this, "MsgInvalidInputTooLongNoLabel", maxLength);
			return Root.Resolver.GetString(this, "MsgInvalidInputTooLong", Label, maxLength);
		}
	}
}