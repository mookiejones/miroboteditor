using System;

namespace miRobotEditor.Core.Tracing
{
    public sealed class PerformanceLogBracket : IDisposable
    {
        private PrettyTraceSource source;
        private string action;
        private DateTime timestampOpened;
        private bool closed;
        public string Action
        {
            get
            {
                return this.action;
            }
        }
        public DateTime TimestampOpened
        {
            get
            {
                return this.timestampOpened;
            }
        }
        internal PerformanceLogBracket(PrettyTraceSource source, string action)
        {
            this.source = source;
            this.action = action;
            this.timestampOpened = DateTime.Now;
        }
        ~PerformanceLogBracket()
        {
            this.Dispose(false);
        }
        public void Close()
        {
            if (!this.closed)
            {
                this.source.OnCloseBracket(this);
                this.closed = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.closed)
            {
                this.Close();
            }
        }
    }
}