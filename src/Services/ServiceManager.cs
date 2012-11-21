/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/19/2012
 * Time: 1:32 PM
 * 
 */
using System;
using System.IO;
using System.Diagnostics;
namespace miRobotEditor.Services
{
	/// <summary>
	/// Maintains a list of services that can be shutdown in the reverse order of their initialization.
	/// Maintains references to the core service implementations.
	/// </summary>
	public abstract class ServiceManager : IServiceProvider
	{
		volatile static ServiceManager instance = new DefaultServiceManager();
		
		/// <summary>
		/// Gets the static ServiceManager instance.
		/// </summary>
		public static ServiceManager Instance {
			get { return instance; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				instance = value;
			}
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public abstract object GetService(Type serviceType);
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public T GetService<T>() where T : class
		{
			return GetService(typeof(T)) as T;
		}
		
		/// <summary>
		/// Gets a service. Throws an exception if service is not found.
		/// </summary>
		public object GetRequiredService(Type serviceType)
		{
			object service = GetService(serviceType);
			if (service == null)
				throw new ServiceNotFoundException();
			return service;
		}
		
		/// <summary>
		/// Gets a service. Throws an exception if service is not found.
		/// </summary>
		public T GetRequiredService<T>() where T : class
		{
			return (T)GetRequiredService(typeof(T));
		}
		
		/// <summary>
		/// Gets the logging service.
		/// </summary>
		public virtual ILoggingService LoggingService {
			get { return (ILoggingService)GetRequiredService(typeof(ILoggingService)); }
		}
		
		/// <summary>
		/// Gets the message service.
		/// </summary>
		public virtual IMessageService MessageService {
			get { return (IMessageService)GetRequiredService(typeof(IMessageService)); }
		}
	}
	
	sealed class DefaultServiceManager : ServiceManager
	{
		static ILoggingService loggingService = new TextWriterLoggingService(new DebugTextWriter());
		static IMessageService messageService = new TextWriterMessageService(Console.Out);
		
		public override ILoggingService LoggingService {
			get { return loggingService; }
		}
		
		public override IMessageService MessageService {
			get { return messageService; }
		}
		
		public override object GetService(Type serviceType)
		{
			if (serviceType == typeof(ILoggingService))
				return loggingService;
			else if (serviceType == typeof(IMessageService))
				return messageService;
			else
				return null;
		}
	}
	public interface ILoggingService
	{
		void Debug(object message);
		void DebugFormatted(string format, params object[] args);
		void Info(object message);
		void InfoFormatted(string format, params object[] args);
		void Warn(object message);
		void Warn(object message, Exception exception);
		void WarnFormatted(string format, params object[] args);
		void Error(object message);
		void Error(object message, Exception exception);
		void ErrorFormatted(string format, params object[] args);
		void Fatal(object message);
		void Fatal(object message, Exception exception);
		void FatalFormatted(string format, params object[] args);
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
	}
		/// <summary>
	/// Interface for the MessageService.
	/// </summary>
	public interface IMessageService
	{
		/// <summary>
		/// Shows an error.
		/// </summary>
		void ShowError(string message);
		
		/// <summary>
		/// Shows an exception.
		/// </summary>
		void ShowException(Exception ex, string message);
		
		/// <summary>
		/// Shows a warning message.
		/// </summary>
		void ShowWarning(string message);
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		bool AskQuestion(string question, string caption);
		
		/// <summary>
		/// Shows a custom dialog.
		/// </summary>
		/// <param name="caption">The title of the dialog.</param>
		/// <param name="dialogText">The description shown in the dialog.</param>
		/// <param name="acceptButtonIndex">
		/// The number of the button that is the default accept button.
		/// Use -1 if you don't want to have an accept button.
		/// </param>
		/// <param name="cancelButtonIndex">
		/// The number of the button that is the cancel button.
		/// Use -1 if you don't want to have a cancel button.
		/// </param>
		/// <param name="buttontexts">The captions of the buttons.</param>
		/// <returns>The number of the button that was clicked, or -1 if the dialog was closed  without clicking a button.</returns>
		int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts);
		string ShowInputBox(string caption, string dialogText, string defaultValue);
		void ShowMessage(string message, string caption);
		
		/// <summary>
		/// Show a message informing the user about a save error.
		/// </summary>
		void InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot);
		
		/// <summary>
		/// Show a message informing the user about a save error,
		/// and allow him to retry/save under alternative name.
		/// </summary>
		ChooseSaveErrorResult ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled);
	}
	public sealed class ChooseSaveErrorResult
	{
		public bool IsRetry { get; private set; }
		public bool IsIgnore { get; private set; }
		public bool IsSaveAlternative { get { return AlternativeFileName != null; } }
		public string AlternativeFileName { get; private set; }
		
		private ChooseSaveErrorResult() {}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="ChooseSaveErrorResult is immutable")]
		public readonly static ChooseSaveErrorResult Retry = new ChooseSaveErrorResult { IsRetry = true };
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="ChooseSaveErrorResult is immutable")]
		public readonly static ChooseSaveErrorResult Ignore = new ChooseSaveErrorResult { IsIgnore = true };
		
		public static ChooseSaveErrorResult SaveAlternative(string alternativeFileName)
		{
			if (alternativeFileName == null)
				throw new ArgumentNullException("alternativeFileName");
			return new ChooseSaveErrorResult { AlternativeFileName = alternativeFileName };
		}
	}
		/// <summary>
	/// TextWriter that writes into System.Diagnostics.Debug
	/// </summary>
	public class DebugTextWriter : TextWriter
	{
		public override System.Text.Encoding Encoding {
			get {
				return System.Text.Encoding.Unicode;
			}
		}
		
		public override void Write(char value)
		{
			Debug.Write(value.ToString());
		}
		
		public override void Write(char[] buffer, int index, int count)
		{
			Debug.Write(new string(buffer, index, count));
		}
		
		public override void Write(string value)
		{
			Debug.Write(value);
		}
		
		public override void WriteLine()
		{
			Debug.WriteLine(string.Empty);
		}
		
		public override void WriteLine(string value)
		{
			Debug.WriteLine(value);
		}
	}
	public class ServiceNotFoundException:Exception{
		
	}
	/// <summary>
	/// IMessageService implementation that writes messages to a text writer.
	/// User input is not implemented by this service.
	/// </summary>
	public class TextWriterMessageService : IMessageService
	{
		readonly TextWriter writer;
		
		public TextWriterMessageService(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
		}
		
		public void ShowError(string message)
		{
			writer.WriteLine(message);
		}
		
		public void ShowException(Exception ex, string message)
		{
			if (message != null) {
				writer.WriteLine(message);
			}
			if (ex != null) {
				writer.WriteLine(ex.ToString());
			}
		}
		
		public void ShowWarning(string message)
		{
			writer.WriteLine(message);
		}
		
		public bool AskQuestion(string question, string caption)
		{
			writer.WriteLine(caption + ": " + question);
			return false;
		}
		
		public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			writer.WriteLine(caption + ": " + dialogText);
			return cancelButtonIndex;
		}
		
		public string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			writer.WriteLine(caption + ": " + dialogText);
			return defaultValue;
		}
		
		public void ShowMessage(string message, string caption)
		{
			writer.WriteLine(caption + ": " + message);
		}
		
		public void InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
		}
		
		public ChooseSaveErrorResult ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
			return ChooseSaveErrorResult.Ignore;
		}
	}
}
