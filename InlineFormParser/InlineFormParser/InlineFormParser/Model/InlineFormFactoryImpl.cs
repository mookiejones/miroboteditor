using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using InlineFormParser.Common.ResourceAccess;
using InlineFormParser.Common.Tracing;
using InlineFormParser.Controls;
using InlineFormParser.Exceptions;

namespace InlineFormParser.Model
{
	public class InlineFormFactoryImpl : AdeComponent, IInlineFormFactory, IStartupCheckable
	{
		private const string msgCommandString = "WM_ILF_COMMAND";

		private const string msgNotificationString = "WM_ILF_NOTIFY";

		private readonly HashSet<IDisposable> activeWrappers = new HashSet<IDisposable>();

		private IDialogService dialogService;

		private int msgCommand;

		private int msgNotification;

		private ComponentResourceAccessor resolver;

		public int InlineFormCommandMessage
		{
			get
			{
				if (msgCommand == 0)
				{
					msgCommand = RegisterWindowMessage("WM_ILF_COMMAND");
				}
				return msgCommand;
			}
		}

		public int InlineFormNotificationMessage
		{
			get
			{
				if (msgNotification == 0)
				{
					msgNotification = RegisterWindowMessage("WM_ILF_NOTIFY");
				}
				return msgNotification;
			}
		}

		internal IDialogService DialogService
		{
			get
			{
				if (dialogService == null)
				{
					dialogService = (GetService(typeof(IDialogService)) as IDialogService);
					if (dialogService == null)
					{
						throw new ServiceNotFoundException("IDialogService");
					}
				}
				return dialogService;
			}
		}

		internal IResourceAccessor Resolver
		{
			get
			{
				if (resolver == null)
				{
					resolver = new FrameworkResourceAccessor(this, "LegacySupport");
				}
				return resolver;
			}
		}

		internal PrettyTraceSource Trace => RootElementBase.Trace;

		public void Check()
		{
			try
			{
				InlineForm.CreateFromXmlString("<InlineForm><Bool Value=\"true\"/></InlineForm>", Resolver);
			}
			catch (Exception innerException)
			{
				throw new StartupCheckFailedException("Cannot create InlineForms!", innerException);
			}
		}

		public T CreateInlineFormWPFControl<T>(string xmlDescription, int width)
		{
			if (!typeof(T).IsAssignableFrom(typeof(InlineFormControl)))
			{
				throw new ArgumentException($"Invalid expected type \"{typeof(T).FullName}\"!");
			}
			if (string.IsNullOrEmpty(xmlDescription))
			{
				throw new ArgumentNullException(nameof(xmlDescription));
			}
			if (width <= 0)
			{
				throw new ArgumentException("width");
			}
			var ilf = InlineForm.CreateFromXmlString(xmlDescription, Resolver);
			return (T)(object)new InlineFormControl(ilf, width);
		}

		public virtual InlineForm CreateInlineForm(string xmlDescription)
		{
			if (string.IsNullOrEmpty(xmlDescription))
			{
				throw new ArgumentNullException(nameof(xmlDescription));
			}
			return InlineForm.CreateFromXmlString(xmlDescription, Resolver);
		}

		public IntPtr CreateInlineFormWindow(Control control, IntPtr hwndParent, int x, int y)
		{
			CustomContentWrapper customContentWrapper = null;
			var stopwatch = Stopwatch.StartNew();
			try
			{
				Trace.WriteLine(TraceEventType.Start, "Creating ILF...");
				if (hwndParent == IntPtr.Zero)
				{
					throw new ArgumentNullException(nameof(hwndParent));
				}
				customContentWrapper = new CustomContentWrapper(this, hwndParent, control, x, y);
				activeWrappers.Add(customContentWrapper);
				return customContentWrapper.Handle;
			}
			catch (Exception exception)
			{
				if (customContentWrapper != null)
				{
					customContentWrapper.Dispose();
				}
				Trace.WriteException(exception, false, "CreateInlineFormWindow failed.");
				return IntPtr.Zero;
			}
			finally
			{
				stopwatch.Stop();
				Trace.WriteLine(TraceEventType.Stop, "Creating ILF took {0}", stopwatch.Elapsed);
			}
		}

		public IntPtr CreateInlineFormWindow(string xmlDescription, IntPtr hwndParent, int x, int y, int width)
		{
			InlineFormWrapper inlineFormWrapper = null;
			var stopwatch = Stopwatch.StartNew();
			try
			{
				Trace.WriteLine(TraceEventType.Start, "Creating ILF...");
				if (hwndParent == IntPtr.Zero)
				{
					throw new ArgumentNullException(nameof(hwndParent));
				}
				InlineFormControl content = CreateInlineFormWPFControl<InlineFormControl>(xmlDescription, width);
				inlineFormWrapper = new InlineFormWrapper(this, hwndParent, content, x, y);
				activeWrappers.Add(inlineFormWrapper);
				return inlineFormWrapper.Handle;
			}
			catch (Exception exception)
			{
				if (inlineFormWrapper != null)
				{
					inlineFormWrapper.Dispose();
				}
				Trace.WriteException(exception, false, "CreateInlineFormWindow failed.");
				return IntPtr.Zero;
			}
			finally
			{
				stopwatch.Stop();
				Trace.WriteLine(TraceEventType.Stop, "Creating ILF took {0}", stopwatch.Elapsed);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			ComponentFramework.Terminated += ComponentFramework_Terminated;
		}

		public void UpdateInlineFormWPFControl<T>(T control, string xmlDescription)
		{
			InlineFormControl inlineFormControl = ((object)control) as InlineFormControl;
			if (inlineFormControl == null)
			{
				throw new InvalidOperationException("Just supporting type of InlineFormControl");
			}
			inlineFormControl.Update(InlineForm.CreateFromXmlString(xmlDescription, Resolver));
		}

		internal void OnWindowDestroyed(IDisposable wrapper)
		{
			if (activeWrappers.Contains(wrapper))
			{
				activeWrappers.Remove(wrapper);
				wrapper.Dispose();
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int RegisterWindowMessage(string lpString);

		private void ComponentFramework_Terminated(object sender, EventArgs e)
		{
			if (activeWrappers.Count > 0)
			{
				Trace.WriteLine(TraceEventType.Warning, "InlineFormFactory has {0} remaining ILFs when terminated!", activeWrappers.Count);
				var array = activeWrappers.ToArray();
				var array2 = array;
				foreach (var disposable in array2)
				{
					disposable.Dispose();
				}
				activeWrappers.Clear();
			}
			if (resolver != null)
			{
				resolver.Dispose();
				resolver = null;
			}
			dialogService = null;
		}
	}
}