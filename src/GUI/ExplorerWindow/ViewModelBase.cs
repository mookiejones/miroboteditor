using System.Collections.Generic;
using System.ComponentModel;

namespace miRobotEditor.Controls.Infrastructure
{
	public class ViewModelBase : INotifyPropertyChanged
	{

		private readonly Dictionary<string, PropertyChangedEventArgs> eventArgsCache;

		protected ViewModelBase()
		{
			eventArgsCache = new Dictionary<string, PropertyChangedEventArgs>();
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged([Localizable(false)] string propertyName)
		{
			PropertyChangedEventArgs args;
			if (!eventArgsCache.TryGetValue(propertyName, out args))
			{
				args = new PropertyChangedEventArgs(propertyName);
				eventArgsCache.Add(propertyName, args);
			}

			RaisePropertyChanged(args);
		}

		protected void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, args);
		}

		#endregion
	}
}
