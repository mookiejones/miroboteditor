using System;

namespace InlineFormParser.Model
{
	public class ViewConnectionChangedEventArgs : EventArgs
	{
		public string ViewSystemName { get; }

		public IView View { get; }

		public DisplaySource DisplaySource { get; }

		public ViewConnectionChangedEventArgs(string viewSystemName, IView view, DisplaySource source)
		{
			if (string.IsNullOrEmpty(viewSystemName))
			{
				throw new ArgumentNullException(nameof(viewSystemName));
			}

			if (source == (DisplaySource)null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			ViewSystemName = viewSystemName;
			View = view ?? throw new ArgumentNullException(nameof(view));
			DisplaySource = source;
		}
	}
	
	public interface IView
	{
		bool IsConnected
		{
			get;
		}

		IViewModel ConnectedViewModel
		{
			get;
		}

		bool CanDisconnect
		{
			get;
		}

		bool ContainsFocus
		{
			get;
		}

		IServiceProvider ServiceProvider
		{
			get;
			set;
		}

		void Connect(IViewModel viewModel, string viewSystemName, object connectionArgument);

		void Disconnect();

		void SetFocus();

		void RequestClose();
	}
}