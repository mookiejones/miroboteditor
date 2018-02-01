using System;
using System.Collections.Generic;
using InlineFormParser.Common.Attributes;

namespace InlineFormParser.Model
{
	
	public interface IViewManager
	{
		ILogicalDisplayProvider MainDisplayProvider
		{
			get;
			set;
		}

		IEnumerable<IViewConfiguration> ViewConfigurations
		{
			get;
		}

		event EventHandler<ViewConnectionChangedEventArgs> ViewConnected;

		event EventHandler<ViewConnectionChangedEventArgs> ViewDisconnecting;

		event EventHandler<DisplayEventArgs> DisplaySelecting;

		event EventHandler<DisplayEventArgs> DisplaySelected;

		event EventHandler<DisplayEventArgs> DisplayEmpty;

		bool IsViewKnown(string viewSystemName);

		void OpenView(string viewSystemName);

		void OpenView(string viewSystemName, [CanBeNull] object connectionArgument);

		void OpenView(string viewSystemName, [CanBeNull] object connectionArgument, [CanBeNull] object displayParameters);

		void CloseView(string viewSystemName);

		void ForceCloseView(string viewSystemName);

		void ToggleView(string viewSystemName);

		void ToggleView(string viewSystemName, [CanBeNull] object connectionArgument);

		bool IsViewOpen(string viewSystemName);

		IView GetOpenView(string viewSystemName);

		bool CanOpenView(string viewSystemName);

		bool CanCloseView(string viewSystemName);

		bool CanToggleView(string viewSystemName);

		void CloseAllViews(bool forceDisconnect);

		DisplaySource GetDisplayForView(string viewSystemName);

		LogicalDisplay GetDisplay(DisplaySource source);

		LogicalDisplay GetOpenDisplay(string viewSystemName);

		IView GetViewOnDisplay(DisplaySource source);

		string GetNameOfView(IView view);

		string[] GetAssociatedViewNames(string viewModelName);

		DisplayMapping[] GetDisplayMappings(string category);

		CommandCall[] GetAllViewCommands(string viewSystemName);

		CommandCall GetViewCommand(string viewSystemName, string commandSystemName);

		IViewConfiguration GetViewConfiguration(string viewSystemName);
	}
	
	public class DisplayMapping
	{
		public string Category { get; }

		public string Source { get; }

		public DisplaySource Target { get; }

		public DisplayMapping(string category, string source, DisplaySource target)
		{
			if (string.IsNullOrEmpty(category))
			{
				throw new ArgumentNullException(nameof(category));
			}
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (target == (DisplaySource)null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			Category = category;
			Source = source;
			Target = target;
		}
	}
}