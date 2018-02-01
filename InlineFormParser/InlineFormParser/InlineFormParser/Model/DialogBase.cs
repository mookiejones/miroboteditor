using System;
using System.Windows.Automation.Peers;
using System.Windows.Media;
namespace InlineFormParser.Model
{

	public class DialogBase : ReflectableUserControl
	{
		private IServiceProvider serviceProvider;

		public IDialogHandler DialogHandler { get; private set; }

		public DialogBase()
		{
			Background = (TryFindResource("DialogBackgroundBrush") as Brush);
		}

		public void Initialize(IDialogHandler handler, IServiceProvider serviceProvider)
		{
			if (DialogHandler != null)
			{
				throw new InvalidOperationException("Dialog cannot be initialized twice.");
			}
			DialogHandler = handler ?? throw new ArgumentNullException(nameof(handler));
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			OnInitialized();
		}

		protected virtual void OnInitialized()
		{
		}

		protected void Close(object answerData)
		{
			DialogHandler.Close(answerData);
			DialogHandler = null;
		}

		protected object GetService(Type serviceType)
		{
			return serviceProvider.GetService(serviceType);
		}
	}
	public class ReflectableUserControl : UserControlBase
	{
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new ReflectableUserControlAutomationPeer(this);
		}
	}
}