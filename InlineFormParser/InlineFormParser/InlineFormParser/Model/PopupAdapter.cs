using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InlineFormParser.Model
{
	public abstract class PopupAdapter : IDisposable
	{
		private IInputControl currentInputControl;

		public Control Control { get; private set; }

		public IPopupManager PopupManager { get; private set; }

		protected object KeyboardToken { get; } = new object();

		protected virtual bool IsDetectionAllowed
		{
			get
			{
				if (Control.IsVisible)
				{
					return Control.IsEnabled;
				}
				return false;
			}
		}

		public PopupAdapter(Control control, IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}
			Control = control ?? throw new ArgumentNullException(nameof(control));
			PopupManager = (serviceProvider.GetService(typeof(IPopupManager)) as IPopupManager);
			if (PopupManager == null)
			{
				ThrowServiceNotFound(typeof(IPopupManager));
			}
			PopupManager.ContentAreaClicked += OnContentAreaClicked;
		}

		~PopupAdapter()
		{
			Dispose(false);
		}

		protected virtual IInputControl DetectEditControl(Point screenPosition)
		{
			return null;
		}

		protected virtual bool OnPopupOpening(IInputControl editControl)
		{
			return true;
		}

		protected virtual void OnPopupClosed(IInputControl editControl)
		{
		}

		public virtual void OpenPopup(IInputControl editControl)
		{
		}

		protected void OnContentAreaClicked(object sender, LowLevelMouseClickEventArgs e)
		{
			if (Control != null && PopupManager != null && Control.IsVisible)
			{
				try
				{
					Point point = Control.PointFromScreen(e.Position);
					Size renderSize = Control.RenderSize;
					if (point.X >= 0.0 && point.Y >= 0.0 && point.X < renderSize.Width && point.Y < renderSize.Height && IsDetectionAllowed)
					{
						IInputControl inputControl = DetectEditControl(e.Position);
						if (inputControl != null && inputControl.HasFocus && !inputControl.Equals(currentInputControl) && OnPopupOpening(inputControl))
						{
							OpenPopup(inputControl);
						}
					}
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		protected void OnPopupEvent(PopupEvent popupEvent, object callerData, object result)
		{
			switch (popupEvent)
			{
				case PopupEvent.Custom:
					break;
				case PopupEvent.Opened:
					if (ReferenceEquals(callerData, KeyboardToken))
					{
						currentInputControl = PopupManager.OpenPopupInputControl;
					}
					break;
				case PopupEvent.Closed:
					if (currentInputControl != null && ReferenceEquals(callerData, KeyboardToken))
					{
						OnPopupClosed(currentInputControl);
						currentInputControl = null;
					}
					break;
			}
		}

		protected bool IsAnyDropDownListOpen(DependencyObject node)
		{
			ComboBox comboBox = node as ComboBox;
			if (comboBox != null && comboBox.IsDropDownOpen)
			{
				return true;
			}
			int childrenCount = VisualTreeHelper.GetChildrenCount(node);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(node, i);
				if (IsAnyDropDownListOpen(child))
				{
					return true;
				}
			}
			return false;
		}

		protected void ThrowServiceNotFound(Type serviceType)
		{
			throw new ServiceNotFoundException(
				$"The control/view of type \"{GetType().FullName}\" requires the \"{serviceType.Name}\" service for popup keyboards!");
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Control != null)
			{
				if (PopupManager != null)
				{
					PopupManager.ContentAreaClicked -= OnContentAreaClicked;
					PopupManager = null;
				}
				currentInputControl = null;
				Control = null;
			}
		}
	}
}