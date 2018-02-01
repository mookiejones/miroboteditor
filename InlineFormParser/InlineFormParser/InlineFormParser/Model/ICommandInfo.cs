using InlineFormParser.Common.Attributes;

namespace InlineFormParser.Model
{
	public interface ICommandInfo : ICheckableCommand
	{
		string SystemName
		{
			get;
		}

		string ComponentName
		{
			get;
		}

		ICommandArgument[] Arguments
		{
			get;
		}

		ICommandManager CommandManager
		{
			get;
		}

		CommandStates GetCommandState([CanBeNull] object context, [CanBeNull] params object[] parameters);

		void Execute([CanBeNull] object context, [CanBeNull] params object[] parameters);

		void ValidateArguments(object[] parameters);

		CommandStates GetCommandStateForCall(CommandCall call, [CanBeNull] object context);

		void ExecuteCall(CommandCall call, [CanBeNull] object context);
	}
}