#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:59 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Common.Tracing
{
	public class PrettyTraceSource : TraceSource
	{
		internal PrettyTraceSource(string name, SourceLevels defaultLevel)
			: base(name, defaultLevel)
		{
		}

		public TraceSourceCounters Counters { get; } = new TraceSourceCounters();

		public void WriteLine(string message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			TraceInformation(message);
			IncCounters(TraceEventType.Information);
		}

		public void WriteLine(string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));
			TraceInformation(format, args);
			IncCounters(TraceEventType.Information);
		}

		public void WriteLine(TraceEventType eventType, string message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			TraceEvent(eventType, 0, message);
			IncCounters(eventType);
		}

		public void WriteLine(TraceEventType eventType, string format, params object[] args)
		{
			if (format == null) throw new ArgumentNullException(nameof(format));
			TraceEvent(eventType, 0, format, args);
			IncCounters(eventType);
		}

		public void WriteException(Exception exception, bool includeStackTrace)
		{
			WriteException(exception, includeStackTrace, string.Empty);
		}

		public void WriteException(Exception exception, bool includeStackTrace, string format, params object[] args)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));
			if (format == null) throw new ArgumentNullException(nameof(format));
			var stringBuilder = new StringBuilder();
			if (format != string.Empty)
			{
				stringBuilder.AppendFormat(format, args);
				stringBuilder.AppendLine();
			}

			stringBuilder.AppendLine(GetExceptionHeadline(exception));
			stringBuilder.Append(GetExceptionDetails(exception, includeStackTrace));
			WriteLine(TraceEventType.Error, stringBuilder.ToString());
		}

		public static string GetExceptionHeadline(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(exception.Message);
			stringBuilder.AppendFormat(" ({0})", exception.GetType().Name);
			var ex = exception as FileNotFoundException;
			if (ex != null)
			{
				var text = exception.Message.ToUpperInvariant();
				var text2 = ex.FileName != null ? ex.FileName.Trim() : null;
				if (!string.IsNullOrEmpty(text2) && !text.Contains(text2.ToUpperInvariant()))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("FileName=\"{0}\"", text2);
				}

				var text3 = ex.FusionLog != null ? ex.FusionLog.Trim() : null;
				if (!string.IsNullOrEmpty(text3) && !text.Contains(text3.ToUpperInvariant()))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("FusionLog=\"{0}\"", text3);
				}
			}

			if (!string.IsNullOrEmpty(exception.Source))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("Source=\"{0}\"", exception.Source);
			}

			return stringBuilder.ToString();
		}

		public static string GetExceptionDetails(Exception exception, bool includeStackTrace)
		{
			if (exception == null) throw new ArgumentNullException(nameof(exception));
			var stringBuilder = new StringBuilder();
			var innerException = exception.InnerException;
			if (includeStackTrace)
			{
				AppendStackTrace(exception, stringBuilder);
				if (innerException != null) stringBuilder.AppendLine();
			}

			while (innerException != null)
			{
				stringBuilder.AppendLine("Caused by:");
				stringBuilder.Append(GetExceptionHeadline(innerException));
				if (includeStackTrace)
				{
					stringBuilder.AppendLine();
					AppendStackTrace(innerException, stringBuilder);
				}

				innerException = innerException.InnerException;
				if (innerException != null) stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}

		public PerformanceLogBracket OpenPerformanceLogBracket(string action)
		{
			if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));
			TryPerformanceLog(action, "_Start");
			WriteLine(TraceEventType.Start, "Starting action \"{0}\" ...", action);
			return new PerformanceLogBracket(this, action);
		}

		public void PerformanceLogMilestone(string milestone)
		{
			if (string.IsNullOrEmpty(milestone)) throw new ArgumentNullException(nameof(milestone));
			TryPerformanceLog(milestone, string.Empty);
			WriteLine(TraceEventType.Verbose, "Milestone \"{0}\" reached", milestone);
		}

		internal void OnCloseBracket(PerformanceLogBracket bracket)
		{
			TryPerformanceLog(bracket.Action, "_End");
			var timeSpan = DateTime.Now - bracket.TimestampOpened;
			WriteLine(TraceEventType.Stop, "Action \"{0}\" completed, took {1}.", bracket.Action, timeSpan);
		}

		private void TryPerformanceLog(string action, string postFix)
		{
			var performanceLogger = TraceSourceFactory.PerformanceLogger;
			if (performanceLogger != null)
			{
				var text = $"{Name}:{action}{postFix}";
				try
				{
					performanceLogger.LogTimestamp(text);
				}
				catch (Exception exception)
				{
					WriteException(exception, false, "Writing timestamp for key \"{0}\" by \"{1}\" failed.", text,
						performanceLogger.GetType().FullName);
				}
			}
		}

		private string MakeFullPerformanceLogKey(string action)
		{
			throw new NotImplementedException();
		}

		private void IncCounters(TraceEventType eventType)
		{
			if (Switch.ShouldTrace(eventType))
			{
				Counters.IncrementCounter(eventType);
				TraceSourceFactory.TotalCounters.IncrementCounter(eventType);
			}
		}

		private static void AppendStackTrace(Exception exception, StringBuilder result)
		{
			result.Append(exception.StackTrace);
		}
	}
}