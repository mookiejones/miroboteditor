using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace InlineFormParser.Model
{
	
internal static class Tracing
{
	internal class TraceIndent : IDisposable
	{
		private TraceSource traceSoure;

		private DateTime start;

		private string message = "";

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal TraceIndent(TraceSource source)
		{
			traceSoure = source;
			start = DateTime.Now;
			traceSoure.TraceEvent(TraceEventType.Start, 0);
		}

		internal TraceIndent(TraceSource source, string format, params object[] args)
		{
			traceSoure = source;
			start = DateTime.Now;
			if (args.Length == 0)
			{
				message = format;
			}
			else
			{
				message = string.Format(CultureInfo.CurrentCulture, format, args);
			}
			traceSoure.TraceEvent(TraceEventType.Start, 0, message);
		}

		public void Dispose()
		{
			if (!string.IsNullOrEmpty(message))
			{
				traceSoure.TraceEvent(TraceEventType.Stop, 0, "\t[{0}] for ({1})", DateTime.Now - start, message);
			}
			else
			{
				traceSoure.TraceEvent(TraceEventType.Stop, 0);
			}
			traceSoure.Flush();
		}
	}

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal static TraceSource AdeComponentFramework = new TraceSource("Ade.AdeComponentFramework", SourceLevels.All);

	internal static TraceSource AdeCommands = new TraceSource("Ade.Commands", SourceLevels.All);

	internal static TraceSource AdeExceptions = new TraceSource("Ade.Exceptions", SourceLevels.All);

	internal static TraceSwitch strictErrorChecking = new TraceSwitch("Ade.StrictErrorChecking", "StrictErrorChecking");

	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal static bool Strict => strictErrorChecking.Level > TraceLevel.Off;

	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal static TraceIndent Indent(TraceSource source)
	{
		return new TraceIndent(source);
	}

	internal static TraceIndent Indent(TraceSource source, string format, params object[] args)
	{
		return new TraceIndent(source, format, args);
	}

	internal static void ExceptionCreatedLine(string format, params object[] args)
	{
		if (AdeExceptions.Switch.ShouldTrace(TraceEventType.Verbose))
		{
			AdeExceptions.TraceInformation(string.Format(CultureInfo.InvariantCulture, format, args) + "\n" + new StackTrace(3).ToString(), "EXCEPTION");
		}
		else if (AdeExceptions.Switch.ShouldTrace(TraceEventType.Warning))
		{
			AdeExceptions.TraceInformation(string.Format(CultureInfo.InvariantCulture, format, args), "EXCEPTION");
		}
	}
}

}