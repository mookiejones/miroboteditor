using System;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
namespace InlineFormParser.Model
{
	
public class CustomContentWrapper : DispatcherObject, IDisposable
{
	private const int WM_DESTROY = 2;

	private const int WS_CHILD = 1073741824;

	private const int WS_VISIBLE = 268435456;

	private readonly int msgCommand;

	private readonly InlineFormFactoryImpl owner;

	private Control content;

	private IntPtr hwndParent;

	private HwndSource hwndSource;

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

	internal bool IsParentEnabledAndVisible
	{
		get
		{
			if (this.hwndParent != IntPtr.Zero && CustomContentWrapper.IsWindowVisible(this.hwndParent))
			{
				return CustomContentWrapper.IsWindowEnabled(this.hwndParent);
			}
			return false;
		}
	}

	internal InlineFormFactoryImpl Owner
	{
		get
		{
			return this.owner;
		}
	}

	public CustomContentWrapper(InlineFormFactoryImpl owner, IntPtr hwndParent, Control content, int x, int y)
	{
		this.owner = owner;
		this.content = content;
		this.hwndParent = hwndParent;
		this.msgCommand = owner.InlineFormCommandMessage;
		this.hwndSource = new HwndSource(new HwndSourceParameters("ILF")
		{
			WindowStyle = 1342177280,
			ParentWindow = hwndParent,
			PositionX = x,
			PositionY = y
		});
		this.hwndSource.AddHook(this.OnHwndSourceHook);
		this.hwndSource.RootVisual = content;
	}

	~CustomContentWrapper()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	public override string ToString()
	{
		return string.Format("ILF hwnd=0x{0}", this.Handle.ToString("X"));
	}

	protected virtual void Dispose(bool disposing)
	{
		this.content = null;
		if (this.hwndSource != null)
		{
			this.hwndSource.RemoveHook(this.OnHwndSourceHook);
			this.hwndSource.Dispose();
			this.hwndSource = null;
		}
		this.hwndParent = IntPtr.Zero;
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowEnabled(IntPtr hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	private IntPtr OnHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == this.msgCommand)
		{
			switch ((int)wParam)
			{
			default:
				this.owner.Trace.WriteLine(TraceEventType.Error, "{0}: Received invalid InlineFormCommandCode {1}!", this.ToString(), wParam.ToInt32());
				break;
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				break;
			}
			handled = true;
		}
		else if (msg == 2)
		{
			Control content2 = this.content;
			this.owner.Trace.WriteLine(TraceEventType.Verbose, "{0}: Destroyed", this.ToString());
			this.owner.OnWindowDestroyed(this);
			handled = true;
		}
		return IntPtr.Zero;
	}
}
}