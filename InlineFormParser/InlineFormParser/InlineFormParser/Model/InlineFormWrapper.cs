using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using InlineFormParser.Controls;

namespace InlineFormParser.Model
{
	
internal class InlineFormWrapper : DispatcherObject, IDisposable
{
	private delegate void NoArgDelegate();

	private const int WM_DESTROY = 2;

	private const int WS_CHILD = 1073741824;

	private const int WS_VISIBLE = 268435456;

	private InlineFormFactoryImpl owner;

	private InlineFormControl content;

	private IntPtr hwndParent;

	private HwndSource hwndSource;

	private InlineFormPopupKeyboardAdapter popupAdapter;

	private int msgCommand;

	private bool destroyPending;

	internal IntPtr Handle
	{
		get
		{
			if (this.hwndSource == null)
			{
				return IntPtr.Zero;
			}
			return this.hwndSource.Handle;
		}
	}

	internal InlineFormFactoryImpl Owner
	{
		get
		{
			return this.owner;
		}
	}

	internal InlineFormControl Content
	{
		get
		{
			return this.content;
		}
	}

	internal bool IsParentEnabledAndVisible
	{
		get
		{
			if (this.hwndParent != IntPtr.Zero && !this.destroyPending && InlineFormWrapper.IsWindowVisible(this.hwndParent))
			{
				return InlineFormWrapper.IsWindowEnabled(this.hwndParent);
			}
			return false;
		}
	}

	internal InlineFormWrapper(InlineFormFactoryImpl owner, IntPtr hwndParent, InlineFormControl content, int x, int y)
	{
		this.owner = owner;
		this.content = content;
		this.hwndParent = hwndParent;
		this.msgCommand = owner.InlineFormCommandMessage;
		this.popupAdapter = new InlineFormPopupKeyboardAdapter(this);
		content.KeyboardAdapter = this.popupAdapter;
		this.hwndSource = new HwndSource(new HwndSourceParameters("ILF")
		{
			WindowStyle = 1342177280,
			ParentWindow = hwndParent,
			PositionX = x,
			PositionY = y
		});
		this.hwndSource.AddHook(this.OnHwndSourceHook);
		this.hwndSource.RootVisual = content;
		InlineFormBase inlineForm = content.InlineForm;
		inlineForm.CurrentFieldIndexChanged += this.OnCurrentFieldIndexChanged;
		inlineForm.FieldValueChanging += this.OnFieldValueChanging;
		inlineForm.RequestParameterList += this.OnRequestParameterList;
		owner.Trace.WriteLine(TraceEventType.Verbose, "{0} created, width={1}", this.ToString(), content.Width);
	}

	~InlineFormWrapper()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (this.content != null)
		{
			InlineFormBase inlineForm = this.content.InlineForm;
			inlineForm.RequestParameterList -= this.OnRequestParameterList;
			inlineForm.FieldValueChanging -= this.OnFieldValueChanging;
			inlineForm.CurrentFieldIndexChanged -= this.OnCurrentFieldIndexChanged;
			this.content = null;
		}
		if (this.popupAdapter != null)
		{
			this.popupAdapter.Dispose();
			this.popupAdapter = null;
		}
		if (this.hwndSource != null)
		{
			this.hwndSource.RemoveHook(this.OnHwndSourceHook);
			this.hwndSource.Dispose();
			this.hwndSource = null;
		}
		this.hwndParent = IntPtr.Zero;
	}

	private void OnCurrentFieldIndexChanged(object sender, CurrentFieldIndexChangedEventArgs e)
	{
		CurrentFieldIndexChangedArgument currentFieldIndexChangedArgument = default(CurrentFieldIndexChangedArgument);
		currentFieldIndexChangedArgument.PreviousFieldIndex = e.PreviousFieldIndex;
		currentFieldIndexChangedArgument.NewFieldIndex = e.NewFieldIndex;
		this.owner.Trace.WriteLine(TraceEventType.Start, "{0}: Notify CurrentFieldIndexChanged, NewFieldIndex=\"{1}\"", this.ToString(), currentFieldIndexChangedArgument.NewFieldIndex);
		try
		{
			InlineFormWrapper.SendMessage(this.hwndSource.Handle, this.owner.InlineFormNotificationMessage, InlineFormNotificationCode.CurrentFieldIndexChanged, ref currentFieldIndexChangedArgument);
		}
		catch (Exception ex)
		{
			this.HandleNotificationException("CurrentFieldIndexChanged", ex);
		}
		finally
		{
			this.owner.Trace.WriteLine(TraceEventType.Stop, "{0}: Notify CurrentFieldIndexChanged completed.", this.ToString());
		}
	}

	private void OnFieldValueChanging(object sender, FieldValueChangingEventArgs e)
	{
		FieldValueChangingArgument fieldValueChangingArgument = default(FieldValueChangingArgument);
		fieldValueChangingArgument.FieldIndex = e.FieldIndex;
		fieldValueChangingArgument.NewValue = e.NewValue;
		fieldValueChangingArgument.TPString = e.TPString;
		this.owner.Trace.WriteLine(TraceEventType.Start, "{0}: Notify FieldValueChanging, TPString=\"{1}\"", this.ToString(), fieldValueChangingArgument.TPString);
		try
		{
			InlineFormWrapper.SendMessage(this.hwndSource.Handle, this.owner.InlineFormNotificationMessage, InlineFormNotificationCode.FieldValueChanging, ref fieldValueChangingArgument);
			e.Cancel = (this.hwndSource != null && fieldValueChangingArgument.Cancel);
		}
		catch (Exception ex)
		{
			this.HandleNotificationException("FieldValueChanging", ex);
		}
		finally
		{
			this.owner.Trace.WriteLine(TraceEventType.Stop, "{0}: Notify FieldValueChanging completed, Cancel={1}", this.ToString(), fieldValueChangingArgument.Cancel);
		}
	}

	private void OnRequestParameterList(object sender, RequestParameterListEventArgs e)
	{
		RequestParameterListArgument requestParameterListArgument = default(RequestParameterListArgument);
		requestParameterListArgument.FieldIndex = e.FieldIndex;
		requestParameterListArgument.ParamListHandle = e.ParamListHandle;
		requestParameterListArgument.Value = e.FieldValue;
		this.owner.Trace.WriteLine(TraceEventType.Start, "{0}: Notify RequestParameterList, ParamListHandle=\"{1}\"", this.ToString(), requestParameterListArgument.ParamListHandle);
		try
		{
			InlineFormWrapper.SendMessage(this.hwndSource.Handle, this.owner.InlineFormNotificationMessage, InlineFormNotificationCode.RequestParameterList, ref requestParameterListArgument);
		}
		catch (Exception ex)
		{
			this.HandleNotificationException("RequestParameterList", ex);
		}
		finally
		{
			this.owner.Trace.WriteLine(TraceEventType.Stop, "{0}: Notify RequestParameterList completed.", this.ToString());
		}
	}

	private IntPtr OnHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == this.msgCommand)
		{
			switch ((int)wParam)
			{
			case 1:
			{
				GetActualValuesArgument getActualValuesArgument = (GetActualValuesArgument)Marshal.PtrToStructure(lParam, typeof(GetActualValuesArgument));
				this.CommandGetActualValues(ref getActualValuesArgument);
				Marshal.StructureToPtr(getActualValuesArgument, lParam, true);
				break;
			}
			case 2:
			{
				GetCurrentFieldIndexArgument getCurrentFieldIndexArgument = (GetCurrentFieldIndexArgument)Marshal.PtrToStructure(lParam, typeof(GetCurrentFieldIndexArgument));
				this.CommandGetCurrentFieldIndex(ref getCurrentFieldIndexArgument);
				Marshal.StructureToPtr(getCurrentFieldIndexArgument, lParam, true);
				break;
			}
			case 3:
			{
				CheckCanCloseArgument checkCanCloseArgument = (CheckCanCloseArgument)Marshal.PtrToStructure(lParam, typeof(CheckCanCloseArgument));
				this.CommandCheckCanClose(ref checkCanCloseArgument);
				Marshal.StructureToPtr(checkCanCloseArgument, lParam, true);
				break;
			}
			case 4:
				if (!this.destroyPending)
				{
					this.destroyPending = true;
					this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: DelayedDestroy scheduled.", this.ToString());
					base.Dispatcher.BeginInvoke(new NoArgDelegate(this.DelayedCommandDestroy));
				}
				break;
			case 5:
				this.CommandResize(lParam.ToInt32());
				break;
			default:
				this.owner.Trace.WriteLine(TraceEventType.Error, "{0}: Received invalid InlineFormCommandCode {1}!", this.ToString(), wParam.ToInt32());
				break;
			case 0:
				break;
			}
			handled = true;
		}
		else if (msg == 2)
		{
			if (this.content != null)
			{
				this.content.UpdatePendingInputs();
			}
			this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: Destroyed", this.ToString());
			this.owner.OnWindowDestroyed(this);
			handled = true;
		}
		return IntPtr.Zero;
	}

	private void CommandGetActualValues(ref GetActualValuesArgument arg)
	{
		this.content.UpdatePendingInputs();
		arg.TPString = this.content.InlineForm.GetTPString(arg.ModifiedOnly);
		this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: Command GetActualValues, ModifiedOnly={1} TPString=\"{2}\"", this.ToString(), arg.ModifiedOnly, arg.TPString);
	}

	private void CommandGetCurrentFieldIndex(ref GetCurrentFieldIndexArgument arg)
	{
		InlineFormBase inlineForm = this.content.InlineForm;
		arg.CurrentFieldIndex = inlineForm.CurrentFieldIndex + 1;
		arg.DefaultFieldIndex = inlineForm.DefaultFieldIndex + 1;
		this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: Command GetCurrentFieldIndex, CurrentFieldIndex={1}", this.ToString(), arg.CurrentFieldIndex);
	}

	private void CommandCheckCanClose(ref CheckCanCloseArgument arg)
	{
		this.content.UpdatePendingInputs();
		InlineFormBase inlineForm = this.content.InlineForm;
		Field field = inlineForm.FindFirstInvalidField();
		if (field == null)
		{
			arg.FirstInvalidFieldIndex = 0;
		}
		else
		{
			arg.FirstInvalidFieldIndex = field.ChildIndex + 1;
			this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: Command CheckCanClose, ShowDialog={1} FirstInvalidFieldIndex={2}", this.ToString(), arg.ShowDialog, arg.FirstInvalidFieldIndex);
			if (arg.ShowDialog)
			{
				string invalidInputMessageText = field.InvalidInputMessageText;
				string text = this.owner.Resolver.GetString(this, "InlineFormTitle");
				if (!string.IsNullOrEmpty(inlineForm.Title))
				{
					text = text + " " + inlineForm.Title;
				}
				this.owner.DialogService.ModalMessageBox(null, text, MessageBoxSymbol.Error, MessageBoxButtons.Ok, invalidInputMessageText);
			}
		}
	}

	private void DelayedCommandDestroy()
	{
		if (this.Handle != IntPtr.Zero)
		{
			InlineFormWrapper.DestroyWindow(this.Handle);
		}
		this.destroyPending = false;
	}

	private void CommandResize(int newWidth)
	{
		this.content.Resize(newWidth);
	}

	public override string ToString()
	{
		return string.Format("ILF hwnd=0x{0}", this.Handle.ToString("X"));
	}

	private void HandleNotificationException(string notification, Exception ex)
	{
		this.owner.Trace.WriteException(ex, false, "{0}: Exception during processing of \"{1}\" notification!", this.ToString(), notification);
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DestroyWindow(IntPtr hwnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowEnabled(IntPtr hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, [MarshalAs(UnmanagedType.I4)] InlineFormNotificationCode code, ref CurrentFieldIndexChangedArgument arg);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, [MarshalAs(UnmanagedType.I4)] InlineFormNotificationCode code, ref FieldValueChangingArgument arg);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, [MarshalAs(UnmanagedType.I4)] InlineFormNotificationCode code, ref RequestParameterListArgument arg);
}
}