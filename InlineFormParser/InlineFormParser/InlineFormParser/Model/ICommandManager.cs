using System.Collections.Generic;

namespace InlineFormParser.Model
{
	public interface ICommandManager
	{
		IEnumerable<ICommandInfo> CommandInfos
		{
			get;
		}

		ICommandInfo GetCommandInfo(string commandName);

		bool ContainsCommand(string commandName);
	}
}