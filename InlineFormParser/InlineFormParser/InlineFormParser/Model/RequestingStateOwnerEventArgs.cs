using System;

namespace InlineFormParser.Model
{
	public class RequestingStateOwnerEventArgs : EventArgs
	{
		public IOwnedGlobalState OwnedState { get; }

		public string StateName => OwnedState.State.Name;

		public bool IsOwned { get; private set; }

		public RequestingStateOwnerEventArgs(IOwnedGlobalState ownedState)
		{
			OwnedState = ownedState ?? throw new ArgumentNullException(nameof(ownedState));
		}

		public void SetOwned(object initialValue)
		{
			if (IsOwned)
			{
				throw new InvalidOperationException($"Global state \"{OwnedState.State.Name}\" is already owned!");
			}
			IsOwned = true;
			OwnedState.SetValue(initialValue);
		}
	}
}