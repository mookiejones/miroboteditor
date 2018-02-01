using System;
using System.ComponentModel;

namespace InlineFormParser.Model
{

	public interface IAdeComponentFramework : IServiceProvider
	{
		bool UserInteractive
		{
			get;
			set;
		}

		event EventHandler StartupCompleting;

		event EventHandler StartupCompleted;

		event CancelEventHandler Terminating;

		event EventHandler Terminated;

		bool IsComponentAvailable(string name);

		IComponent GetComponent(string name);

		bool CanTerminate();

		void Terminate();

		void ReportUnexpectedException(Exception exception, string message);
	}

}