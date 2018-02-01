#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:00 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Linq;

#endregion

namespace InlineFormParser.Common.Tracing
{
	public class TraceSourceCounters
	{
		private readonly int[] counters = new int[5];

		public bool HasAnyEntries
		{
			get
			{
				var array = counters;
				return array.Any(num => num > 0);
			}
		}

		public int AllErrors => GetCount(TraceEventType.Error) + GetCount(TraceEventType.Critical);

		public bool HasErrors => AllErrors > 0;

		public bool HasWarningsOrErrors => HasEntriesOf(TraceEventType.Warning) || HasErrors;

		public void Clear()
		{
			for (int i = 0; i < counters.Length; i++) counters[i] = 0;
		}

		public bool IsEventTypeCounted(TraceEventType eventType)
		{
			return EventTypeToIndex(eventType) >= 0;
		}

		public int GetCount(TraceEventType eventType)
		{
			return counters[CheckedEventTypeToIndex(eventType)];
		}

		public bool HasEntriesOf(TraceEventType eventType)
		{
			return GetCount(eventType) > 0;
		}

		internal void IncrementCounter(TraceEventType eventType)
		{
			int num = EventTypeToIndex(eventType);
			if (num >= 0) counters[num]++;
		}

		private int EventTypeToIndex(TraceEventType eventType)
		{
			switch (eventType)
			{
				case TraceEventType.Critical:
					return 0;
				case TraceEventType.Error:
					return 1;
				case TraceEventType.Information:
					return 2;
				case TraceEventType.Warning:
					return 3;
				case TraceEventType.Verbose:
					return 4;
				default:
					return -1;
			}
		}

		private int CheckedEventTypeToIndex(TraceEventType eventType)
		{
			int num = EventTypeToIndex(eventType);
			if (num < 0) throw new ArgumentException($"EventType \"{eventType}\" is not counted.");
			return num;
		}
	}
}