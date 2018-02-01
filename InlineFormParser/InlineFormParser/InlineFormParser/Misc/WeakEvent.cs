#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:42 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Misc
{
	public sealed class WeakEvent
	{
		private Action removeEventHandler;

		private WeakEvent(Action removeEventHandler)
		{
			this.removeEventHandler = removeEventHandler;
		}

		public static WeakEvent Register<S>(S subscriber, Action<EventHandler> addEventhandler,
			Action<EventHandler> removeEventHandler, Action<S, EventArgs> action) where S : class
		{
			return Register(subscriber, eventHandler => delegate(object sender, EventArgs e) { eventHandler(sender, e); },
				addEventhandler, removeEventHandler, action);
		}

		public static WeakEvent Register<S, TEventArgs>(S subscriber, Action<EventHandler<TEventArgs>> addEventhandler,
			Action<EventHandler<TEventArgs>> removeEventHandler, Action<S, TEventArgs> action)
			where S : class where TEventArgs : EventArgs
		{
			return Register(subscriber, eventHandler => eventHandler, addEventhandler, removeEventHandler, action);
		}

		public static WeakEvent Register<S, TEventHandler, TEventArgs>(S subscriber,
			Func<EventHandler<TEventArgs>, TEventHandler> getEventHandler, Action<TEventHandler> addEventHandler,
			Action<TEventHandler> removeEventHandler, Action<S, TEventArgs> action) where S : class
			where TEventHandler : class
			where TEventArgs : EventArgs
		{
			var weakReference = new WeakReference(subscriber);
			TEventHandler eventHandler = null;
			eventHandler = getEventHandler(delegate(object sender, TEventArgs e)
			{
				var val = weakReference.Target as S;
				if (val != null)
				{
					action(val, e);
				}
				else
				{
					removeEventHandler(eventHandler);
					eventHandler = null;
				}
			});
			addEventHandler(eventHandler);
			return new WeakEvent(delegate { removeEventHandler(eventHandler); });
		}

		public void Unregister()
		{
			if (removeEventHandler != null)
			{
				removeEventHandler();
				removeEventHandler = null;
			}
		}
	}
}