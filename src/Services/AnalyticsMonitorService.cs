/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/19/2012
 * Time: 1:28 PM
 * 
 */
using System;
using System.ComponentModel;

namespace miRobotEditor.Services
{
		/// <summary>
	/// Allows marking the end-time of feature uses.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	public interface IAnalyticsMonitorTrackedFeature
	{
		void EndTracking();
	}
	
	/// <summary>
	/// Description of AnalyticsMonitor.
	/// </summary>
	[Localizable(false)]
	public static class AnalyticsMonitorService
	{
	
		/// <summary>
		/// Tracks an exception that has occurred.
		/// </summary>
		public static void TrackException(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
				var monitor = ServiceManager.Instance.GetService<IAnalyticsMonitor>();
			if (monitor != null) {
				monitor.TrackException(exception);
			}
		}
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureName">Name of the feature</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(string featureName)
		{
			return TrackFeature(featureName, null);
		}
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod)
		{
			if (featureName == null)
				throw new ArgumentNullException("featureName");
			
			if (activationMethod != null)
				LoggingService.Debug("Activated feature '" + featureName + "', activation=" + activationMethod);
			else
				LoggingService.Debug("Activated feature '" + featureName + "'");
			
			var monitor = ServiceManager.Instance.GetService<IAnalyticsMonitor>();
			if (monitor != null) {
				return monitor.TrackFeature(featureName, activationMethod) ?? DummyFeature.Instance;
			}
		    return DummyFeature.Instance;
		}
		
		sealed class DummyFeature : IAnalyticsMonitorTrackedFeature
		{
			public static readonly DummyFeature Instance = new DummyFeature();
			
			public void EndTracking()
			{
			}
		}
		
		
		/// <summary>
		/// Tracks a feature use.
		/// </summary>
		/// <param name="featureClass">Class containing the feature</param>
		/// <param name="featureName">Name of the feature</param>
		/// <param name="activationMethod">Method used to 'activate' the feature (e.g. Menu, Toolbar, Shortcut, etc.)</param>
		/// <returns>Object that can be used to 'end' the feature use, if measuring time spans is desired.</returns>
		public static IAnalyticsMonitorTrackedFeature TrackFeature(Type featureClass, string featureName = null, string activationMethod = null)
		{
			if (featureClass == null)
				throw new ArgumentNullException("featureClass");
			return featureName != null ? TrackFeature(featureClass.FullName + "/" + featureName, activationMethod) : TrackFeature(featureClass.FullName, activationMethod);
		}
	}
		/// <summary>
	/// Interface for AnalyticsMonitorService.
	/// </summary>
	/// <remarks>Implementations of this interface must be thread-safe.</remarks>
	public interface IAnalyticsMonitor
	{
		void TrackException(Exception exception);
		IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod);
	}
}
