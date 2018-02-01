using System;

namespace InlineFormParser.Model
{
	public interface ICommandArgument
	{
		string Name
		{
			get;
		}

		Type ValueType
		{
			get;
		}

		bool IsRequired
		{
			get;
		}
	}
}