#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:04 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Common.Tracing
{
	public class PerformanceLogBracket : IDisposable
	{
		private bool closed;
		private readonly PrettyTraceSource source;

		internal PerformanceLogBracket(PrettyTraceSource source, string action)
		{
			this.source = source;
			Action = action;
			TimestampOpened = DateTime.Now;
		}

		public string Action { get; }

		public DateTime TimestampOpened { get; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~PerformanceLogBracket()
		{
			Dispose(false);
		}

		public void Close()
		{
			if (!closed)
			{
				source.OnCloseBracket(this);
				closed = true;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!closed) Close();
		}
	}
}