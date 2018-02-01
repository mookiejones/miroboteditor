using System;
using System.Collections.Generic;

namespace InlineFormParser.Model
{
	public interface IGlobalStateProvider
	{
		IEnumerable<IGlobalState> KnownStates
		{
			get;
		}

		event EventHandler<RequestingStateOwnerEventArgs> RequestingStateOwner;

		event EventHandler<GlobalStateChangedEventArgs> GlobalStateChanged;

		bool IsStateKnown(string name);

		IGlobalState GetState(string name, bool throwIfNotProvided);
	}
}