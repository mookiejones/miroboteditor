using System;
using System.Collections.Generic;

namespace InlineFormParser.Model
{
	
	public interface ILogicalDisplayProvider
	{
		IEnumerable<LogicalDisplay> AvailableDisplays
		{
			get;
		}

		event EventHandler<DisplayClosedItselfEventArgs> DisplayClosedItself;

		LogicalDisplay GetDisplay(string id);
	}
}