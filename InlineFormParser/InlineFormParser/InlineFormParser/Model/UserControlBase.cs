using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using InlineFormParser.Common.Tracing;

namespace InlineFormParser.Model
{
	
public class UserControlBase : UserControl, IDisposable
{
	protected enum ObjectState
	{
		Unknown,
		Alive,
		Disposing,
		Disposed
	}

	protected ObjectState State { get; private set; }

	public UserControlBase()
	{
		State = ObjectState.Alive;
		Focusable = false;
	}

	~UserControlBase()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (State == ObjectState.Alive)
		{
			State = ObjectState.Disposing;
			try
			{
				OnDisposing(disposing);
			}
			finally
			{
				State = ObjectState.Disposed;
			}
		}
	}

	protected virtual void OnDisposing(bool disposing)
	{
		if (disposing)
		{
			RecClearBindings(this);
		}
	}

	protected void RecClearBindings(DependencyObject node)
	{
		BindingOperations.ClearAllBindings(node);
		int childrenCount = VisualTreeHelper.GetChildrenCount(node);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(node, i);
			RecClearBindings(child);
		}
	}

	protected void HandleWPFException(string category, Exception ex)
	{
		PrettyTraceSource source = TraceSourceFactory.GetSource(PredefinedTraceSource.WPF);
		source.WriteLine(TraceEventType.Error, "WPF problem at \"{0}\" in \"{1}\"!", category, ToString());
		throw ex;
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		try
		{
			base.OnRender(drawingContext);
		}
		catch (Exception ex)
		{
			HandleWPFException("OnRender", ex);
		}
	}

	protected override Size MeasureOverride(Size constraint)
	{
		try
		{
			return base.MeasureOverride(constraint);
		}
		catch (Exception ex)
		{
			HandleWPFException("MeasureOverride", ex);
			return constraint;
		}
	}

	protected override Size ArrangeOverride(Size arrangeBounds)
	{
		try
		{
			return base.ArrangeOverride(arrangeBounds);
		}
		catch (Exception ex)
		{
			HandleWPFException("ArrangeOverride", ex);
			return arrangeBounds;
		}
	}
}
}