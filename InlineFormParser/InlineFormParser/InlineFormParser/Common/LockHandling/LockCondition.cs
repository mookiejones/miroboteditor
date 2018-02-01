#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:37 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using InlineFormParser.Misc;

#endregion

namespace InlineFormParser.Common.LockHandling
{
	public abstract class LockCondition : ILockCondition, IDisposable
	{
		protected LockCondition()
		{
			Targets = new List<object>();
			RegisteredRequeryEvents = new Dictionary<string, WeakEvent>();
		}

		public Dictionary<string, WeakEvent> RegisteredRequeryEvents { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual bool IsConditionFullfilled => false;

		public IList<object> Targets { get; private set; }

		public event EventHandler RequerySuggested;

		public abstract void GetLockInfo(out IEnumerable<string> result);

		public void RaiseRequerySuggested(EventArgs e)
		{
			if (RequerySuggested != null) RequerySuggested(this, e);
		}

		~LockCondition()
		{
			Dispose(false);
		}

		public void RegisterRequeryEvent<TArgs>(string uniqueName, Action<EventHandler<TArgs>> add,
			Action<EventHandler<TArgs>> remove) where TArgs : EventArgs
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(uniqueName)) throw new ArgumentNullException(nameof(uniqueName));
			if (RegisteredRequeryEvents.ContainsKey(uniqueName))
				throw new ArgumentException($"RequeryEvent already registered with the given name [{uniqueName}]");
			RegisteredRequeryEvents.Add(uniqueName,
				WeakEvent.Register(this, eventHandler => eventHandler, add, remove,
					delegate(LockCondition s, TArgs e) { RaiseRequerySuggested(e); }));
		}

		public void RegisterRequeryEvent<TEventHandler, TEventArgs>(string uniqueName,
			Func<EventHandler<TEventArgs>, TEventHandler> getEventHandler, Action<TEventHandler> add,
			Action<TEventHandler> remove) where TEventHandler : class where TEventArgs : EventArgs
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(uniqueName)) throw new ArgumentNullException(nameof(uniqueName));
			if (RegisteredRequeryEvents.ContainsKey(uniqueName))
				throw new ArgumentException($"RequeryEvent already registered with the given name [{uniqueName}]");
			RegisteredRequeryEvents.Add(uniqueName,
				WeakEvent.Register(this, getEventHandler, add, remove,
					delegate(LockCondition s, TEventArgs e) { RaiseRequerySuggested(e); }));
		}

		protected virtual void Dispose(bool disposing)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (RegisteredRequeryEvents != null)
			{
				foreach (var value in RegisteredRequeryEvents.Values) value.Unregister();
				RegisteredRequeryEvents = null;
			}

			Targets = null;
		}
	}
}