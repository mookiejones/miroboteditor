using System.Diagnostics;
using System.Globalization;
using InlineFormParser.Properties;

namespace InlineFormParser.Model
{
	
	public abstract class Command
	{
		public string OriginalName { get; set; } = "";

		public virtual CommandStates GetCommandState(object context, params object[] parameters)
		{
			return CommandStates.Enabled;
		}

		public void Execute(object context, params object[] parameters)
		{
			string format = (context != null) ? string.Format(CultureInfo.CurrentCulture, Resources.ExecutingCommand, ToString(), context.ToString()) : string.Format(CultureInfo.CurrentCulture, Resources.ExecutingAsGlobalCommand, ToString());
			using (Tracing.Indent(Tracing.AdeCommands, format))
			{
				Trace.CorrelationManager.StartLogicalOperation();
				try
				{
					OnExecute(context, parameters);
				}
				finally
				{
					Trace.CorrelationManager.StopLogicalOperation();
				}
			}
		}

		protected abstract void OnExecute(object context, params object[] parameters);

		public override string ToString()
		{
			return OriginalName;
		}
	}
}