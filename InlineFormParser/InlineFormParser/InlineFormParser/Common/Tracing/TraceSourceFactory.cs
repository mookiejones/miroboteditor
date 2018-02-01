#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:02 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

#endregion

namespace InlineFormParser.Common.Tracing
{
	public static class TraceSourceFactory
	{
		private const SourceLevels defaultSourceLevels = SourceLevels.Information;

		private static readonly Dictionary<string, PrettyTraceSource>
			sourcesMap = new Dictionary<string, PrettyTraceSource>();

		public static IEnumerable<PrettyTraceSource> AllSources => sourcesMap.Values;

		public static TraceSourceCounters TotalCounters { get; } = new TraceSourceCounters();

		public static IPerformanceLogger PerformanceLogger { get; set; }

		public static bool HasPerformanceLogger => PerformanceLogger != null;

		public static PrettyTraceSource GetSource(string id)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentException("id");
			if (!sourcesMap.ContainsKey(id))
			{
				var prettyTraceSource = new PrettyTraceSource(id, SourceLevels.Information);
				sourcesMap[id] = prettyTraceSource;
				CheckConfigured(prettyTraceSource);
			}

			return sourcesMap[id];
		}

		public static PrettyTraceSource GetSource(PredefinedTraceSource source)
		{
			return GetSource(source.ToString());
		}

		public static bool HasSource(string id)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentException("id");
			return sourcesMap.ContainsKey(id);
		}

		public static bool HasSource(PredefinedTraceSource source)
		{
			return HasSource(source.ToString());
		}

		private static void CheckConfigured(TraceSource source)
		{
			if (GetSourceConfiguration(source) == null)
			{
				TraceSource source2 = GetSource(PredefinedTraceSource.Application);
				if (source2 != null && source != source2)
				{
					source.Listeners.Clear();
					source.Listeners.AddRange(source2.Listeners);
					source.Switch.Level = source2.Switch.Level;
				}
				else
				{
					source.Switch.Level = SourceLevels.Information;
				}
			}
		}

		private static ConfigurationElement GetSourceConfiguration(TraceSource source)
		{
			var configurationSection = (ConfigurationSection) ConfigurationManager.GetSection("system.diagnostics");
			var configurationElementCollection =
				configurationSection?.ElementInformation.Properties["sources"].Value as ConfigurationElementCollection;
			if (configurationElementCollection == null) return null;
			foreach (ConfigurationElement item in configurationElementCollection)
			{
				var text = item.ElementInformation.Properties["name"].Value as string;
				if (text != null && text == source.Name) return item;
			}

			return null;
		}
	}
}