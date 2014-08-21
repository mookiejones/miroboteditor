using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
namespace miRobotEditor.Core.Tracing
{
    public static class TraceSourceFactory
    {
        private const SourceLevels DefaultSourceLevels = SourceLevels.Information;
        private static readonly Dictionary<string, PrettyTraceSource> sourcesMap = new Dictionary<string, PrettyTraceSource>();
        private static readonly TraceSourceCounters totalCounters = new TraceSourceCounters();
        private static IPerformanceLogger _performanceLogger;
        public static IEnumerable<PrettyTraceSource> AllSources
        {
            get
            {
                return sourcesMap.Values;
            }
        }
        public static TraceSourceCounters TotalCounters
        {
            get
            {
                return totalCounters;
            }
        }
        public static IPerformanceLogger PerformanceLogger
        {
            get
            {
                return _performanceLogger;
            }
// ReSharper disable once UnusedMember.Global
            set
            {
                _performanceLogger = value;
            }
        }
// ReSharper disable once UnusedMember.Global
        public static bool HasPerformanceLogger
        {
            get
            {
                return _performanceLogger != null;
            }
        }
        public static PrettyTraceSource GetSource(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }
            if (!TraceSourceFactory.sourcesMap.ContainsKey(id))
            {
                var prettyTraceSource = new PrettyTraceSource(id, SourceLevels.Information);
                TraceSourceFactory.sourcesMap[id] = prettyTraceSource;
                TraceSourceFactory.CheckConfigured(prettyTraceSource);
            }
            return TraceSourceFactory.sourcesMap[id];
        }
        public static PrettyTraceSource GetSource(PredefinedTraceSource source)
        {
            return TraceSourceFactory.GetSource(source.ToString());
        }
        public static bool HasSource(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }
            return TraceSourceFactory.sourcesMap.ContainsKey(id);
        }
        public static bool HasSource(PredefinedTraceSource source)
        {
            return TraceSourceFactory.HasSource(source.ToString());
        }
        private static void CheckConfigured(TraceSource source)
        {
            if (TraceSourceFactory.GetSourceConfiguration(source) == null)
            {
                TraceSource source2 = TraceSourceFactory.GetSource(PredefinedTraceSource.Application);
                if (source2 != null && source != source2)
                {
                    source.Listeners.Clear();
                    source.Listeners.AddRange(source2.Listeners);
                    source.Switch.Level = source2.Switch.Level;
                    return;
                }
                source.Switch.Level = System.Diagnostics.SourceLevels.Information;
            }
        }
        private static ConfigurationElement GetSourceConfiguration(TraceSource source)
        {
            ConfigurationSection configurationSection = (ConfigurationSection)ConfigurationManager.GetSection("system.diagnostics");
            if (configurationSection != null)
            {
                ConfigurationElementCollection configurationElementCollection = configurationSection.ElementInformation.Properties["sources"].Value as ConfigurationElementCollection;
                if (configurationElementCollection != null)
                {
                    foreach (ConfigurationElement configurationElement in configurationElementCollection)
                    {
                        string text = configurationElement.ElementInformation.Properties["name"].Value as string;
                        if (text != null && text == source.Name)
                        {
                            return configurationElement;
                        }
                    }
                }
            }
            return null;
        }
    }
}
