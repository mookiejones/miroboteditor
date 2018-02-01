using System;

namespace InlineFormParser.Model
{
	public interface IGlobalState
	{
		string Name
		{
			get;
		}

		object Value
		{
			get;
		}

		event EventHandler ValueChanged;
	}
}