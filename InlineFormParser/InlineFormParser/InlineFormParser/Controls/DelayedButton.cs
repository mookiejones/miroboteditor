using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace InlineFormParser.Controls
{
	
	public class DelayedButton : Button
	{
		private const int delayMilliseconds = 200;

		private DispatcherTimer timer;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				if (!base.IsPressed)
				{
					base.IsPressed = true;
					base.CaptureMouse();
				}
				if (this.timer == null)
				{
					this.timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, base.Dispatcher);
					this.timer.Tick += this.OnTimer;
					this.timer.Interval = TimeSpan.FromMilliseconds(200.0);
					this.timer.Start();
				}
				e.Handled = true;
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.ReleaseMouseCapture();
			if (base.IsPressed)
			{
				base.IsPressed = false;
			}
			if (this.timer != null && !e.Handled)
			{
				this.StopTimer();
				this.RaiseClick();
				e.Handled = true;
			}
			base.OnMouseLeftButtonUp(e);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", base.GetType().Name, (base.Name != null) ? base.Name.ToString() : "?");
		}

		private void OnTimer(object sender, EventArgs e)
		{
			if (this.timer != null)
			{
				this.StopTimer();
				this.RaiseClick();
			}
		}

		private void StopTimer()
		{
			this.timer.Stop();
			this.timer.Tick -= this.OnTimer;
			this.timer = null;
		}

		private void RaiseClick()
		{
			this.OnClick();
		}
	}
}
