#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:25 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using InlineFormParser.Common.Tracing;

#endregion

namespace InlineFormParser.Model
{
	public static class CultureSwitch
	{
		private static readonly PrettyTraceSource trace = TraceSourceFactory.GetSource(PredefinedTraceSource.Resources);

		private static readonly int numPriorities = Enum.GetValues(typeof(CultureSwitchListenerPriority)).Length;

		private static readonly EventHandler<CultureChangedEventArgs>[] cultureChanged =
			new EventHandler<CultureChangedEventArgs>[numPriorities];

		private static CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

		private static readonly object cultureLock = new object();

		private static readonly object handlerLock = new object();

		public static bool IsAsync { get; internal set; }

		public static CultureInfo CurrentCulture
		{
			get
			{
				lock (cultureLock)
				{
					return currentCulture;
				}
			}
			set
			{
				bool flag = false;
				CultureInfo cultureInfo = null;
				if (value == null) throw new ArgumentNullException("CurrentCulture");
				lock (cultureLock)
				{
					cultureInfo = currentCulture;
					if (!string.Equals(value.Name, cultureInfo.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						currentCulture = value;
						Thread.CurrentThread.CurrentUICulture = value;
						flag = true;
					}
				}

				if (flag)
					lock (handlerLock)
					{
						Stopwatch stopwatch = Stopwatch.StartNew();
						try
						{
							trace.WriteLine(TraceEventType.Start, "CultureSwitch: Switching from \"{0}\" to  \"{1}\" ...", cultureInfo.Name,
								currentCulture.Name);
							CultureChangedEventArgs e = new CultureChangedEventArgs(cultureInfo, currentCulture, IsAsync);
							EventHandler<CultureChangedEventArgs>[] array = cultureChanged;
							foreach (EventHandler<CultureChangedEventArgs> eventHandler in array)
								if (eventHandler != null)
								{
									Delegate[] invocationList = eventHandler.GetInvocationList();
									for (int j = 0; j < invocationList.Length; j++)
									{
										EventHandler<CultureChangedEventArgs> eventHandler2 =
											(EventHandler<CultureChangedEventArgs>) invocationList[j];
										try
										{
											eventHandler2(null, e);
										}
										catch (Exception exception)
										{
											trace.WriteException(exception, true, "Invocation of \"CultureChanged\" event handler failed!");
										}
									}
								}
						}
						finally
						{
							stopwatch.Stop();
							trace.WriteLine(TraceEventType.Stop, "CultureSwitch: Culture change took {0}.", stopwatch.Elapsed);
						}
					}
			}
		}

		public static event EventHandler<CultureChangedEventArgs> CultureChanged
		{
			add => AddCultureChangedListener(CultureSwitchListenerPriority.Normal, value);
			remove => RemoveCultureChangedListener(CultureSwitchListenerPriority.Normal, value);
		}

		public static unsafe void AddCultureChangedListener(CultureSwitchListenerPriority priority,
			EventHandler<CultureChangedEventArgs> eventHandler)
		{
			if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
			lock (handlerLock)
			{
				EventHandler<CultureChangedEventArgs>[] array;
				EventHandler<CultureChangedEventArgs>[] array2 = array = cultureChanged;
				IntPtr intPtr = (IntPtr) (void*) (int) priority;
				array2[(int) priority] =
					(EventHandler<CultureChangedEventArgs>) Delegate.Combine(array[(long) intPtr], eventHandler);
			}

			trace.WriteLine(TraceEventType.Verbose, "CultureSwitch: Added listener \"{0}\" with priority \"{1}\", total={2}.",
				eventHandler.Target.GetType().Name, priority, cultureChanged[(int) priority].GetInvocationList().Length);
		}

		public static unsafe void RemoveCultureChangedListener(CultureSwitchListenerPriority priority,
			EventHandler<CultureChangedEventArgs> eventHandler)
		{
			if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
			lock (handlerLock)
			{
				EventHandler<CultureChangedEventArgs>[] array;
				EventHandler<CultureChangedEventArgs>[] array2 = array = cultureChanged;
				IntPtr intPtr = (IntPtr) (void*) (int) priority;
				array2[(int) priority] =
					(EventHandler<CultureChangedEventArgs>) Delegate.Remove(array[(long) intPtr], eventHandler);
			}

			trace.WriteLine(TraceEventType.Verbose, "CultureSwitch: Removed listener \"{0}\" with priority \"{1}\", total={2}.",
				eventHandler.Target.GetType().Name, priority,
				cultureChanged[(int) priority] != null ? cultureChanged[(int) priority].GetInvocationList().Length : 0);
		}

		public static void AssertNoMoreCultureChangedListeners(bool cleanUpRemainingHandler = false)
		{
			StringBuilder stringBuilder = null;
			for (int i = 0; i < numPriorities; i++)
			{
				EventHandler<CultureChangedEventArgs> eventHandler = cultureChanged[i];
				if (eventHandler != null)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("CultureSwitch: Remaining listeners:");
					}

					Delegate[] invocationList = eventHandler.GetInvocationList();
					Delegate[] array = invocationList;
					foreach (Delegate @delegate in array)
					{
						stringBuilder.AppendFormat("Priority={0}, Type={1}", (CultureSwitchListenerPriority) i,
							@delegate.Target.GetType().FullName);
						stringBuilder.AppendLine();
					}

					if (cleanUpRemainingHandler) cultureChanged[i] = null;
				}
			}

			if (stringBuilder != null)
				trace.WriteLine(TraceEventType.Error, stringBuilder.ToString());
			else
				trace.WriteLine("CultureSwitch: No more listeners.");
		}
	}
}