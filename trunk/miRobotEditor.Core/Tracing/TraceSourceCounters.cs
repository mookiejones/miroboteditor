using System;
using System.Diagnostics;

namespace miRobotEditor.Core.Tracing
{
    public class TraceSourceCounters
    {
        private int[] counters = new int[5];
        public bool HasAnyEntries
        {
            get
            {
                int[] array = this.counters;
                for (int i = 0; i < array.Length; i++)
                {
                    int num = array[i];
                    if (num > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public int AllErrors
        {
            get
            {
                return this.GetCount(TraceEventType.Error) + this.GetCount(TraceEventType.Critical);
            }
        }
        public bool HasErrors
        {
            get
            {
                return this.AllErrors > 0;
            }
        }
        public bool HasWarningsOrErrors
        {
            get
            {
                return this.HasEntriesOf(TraceEventType.Warning) || this.HasErrors;
            }
        }
        public void Clear()
        {
            for (int i = 0; i < this.counters.Length; i++)
            {
                this.counters[i] = 0;
            }
        }
        public bool IsEventTypeCounted(TraceEventType eventType)
        {
            return this.EventTypeToIndex(eventType) >= 0;
        }
        public int GetCount(TraceEventType eventType)
        {
            return this.counters[this.CheckedEventTypeToIndex(eventType)];
        }
        public bool HasEntriesOf(TraceEventType eventType)
        {
            return this.GetCount(eventType) > 0;
        }
        internal void IncrementCounter(TraceEventType eventType)
        {
            int num = this.EventTypeToIndex(eventType);
            if (num >= 0)
            {
                this.counters[num]++;
            }
        }
        private int EventTypeToIndex(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    return 0;
                case TraceEventType.Error:
                    return 1;
                case (TraceEventType)3:
                    break;
                case TraceEventType.Warning:
                    return 3;
                default:
                    if (eventType == TraceEventType.Information)
                    {
                        return 2;
                    }
                    if (eventType == TraceEventType.Verbose)
                    {
                        return 4;
                    }
                    break;
            }
            return -1;
        }
        private int CheckedEventTypeToIndex(TraceEventType eventType)
        {
            int num = this.EventTypeToIndex(eventType);
            if (num < 0)
            {
                throw new ArgumentException(string.Format("EventType \"{0}\" is not counted.", eventType));
            }
            return num;
        }
    }
}