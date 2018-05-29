using System;
using System.Collections.Generic;

namespace DUCK.PackageManager.Editor.UI.Stores
{
	public abstract partial class Store
	{
		private readonly Dictionary<string, List<Action<Flux.Action>>> handlers;
		private readonly List<IStoreState> stateList;

		protected Store()
		{
			handlers = new Dictionary<string, List<Action<Flux.Action>>>();
			stateList = new List<IStoreState>();
		}

		public void Subscribe(string actionType, Action<Flux.Action> handler)
		{
			if (actionType == null) throw new ArgumentNullException("actionType");
			if (handler == null) throw new ArgumentNullException("handler");

			if (!handlers.ContainsKey(actionType))
			{
				handlers.Add(actionType, new List<Action<Flux.Action>>());
			}

			handlers[actionType].Add(handler);
		}

		public List<IState> Dispatch(Flux.Action action)
		{
			// Notify all stores that action has come in
			//  - allowing them to manipulate their state. aka "call reducers"
			if (handlers.ContainsKey(action.Type))
			{
				foreach (var handler in handlers[action.Type])
				{
					handler(action);
				}
			}

			var dirtyState = new List<IState>();

			// Now find any state that has changed
			//  - notify anybody that is connected to those state slices
			foreach (var state in stateList)
			{
				if (state.IsDirty)
				{
					dirtyState.Add(state);
					state.Clean();
				}
			}

			return dirtyState;
		}

		public IState<T> CreateState<T>(T value)
		{
			var state = new State<T>(value);
			stateList.Add(state);
			return state;
		}
	}
}