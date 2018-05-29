using System;
using System.Collections.Generic;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Flux.Components
{
	public abstract class StateBoundComponent : VisualElement
	{
		private class ValueSelector
		{
			private readonly Func<IState, object> selectorFunc;

			public ValueSelector(Func<IState, object> selectorFunc = null)
			{
				this.selectorFunc = selectorFunc;
			}

			public static ValueSelector Create<TInput, TResult>(Func<TInput, TResult> selectorFunc)
			{
				return new ValueSelector(s => selectorFunc((TInput)s.GetValue()));
			}

			public object ComputeValue(IState state)
			{
				return selectorFunc != null ? selectorFunc(state) : state.GetValue();
			}
		}

		protected interface IDerivedState<TResult>
		{
			TResult Value { get; }
		}

		protected class DerivedState<TInput, TResult> : IDerivedState<TResult>
		{
			public TResult Value
			{
				get { return selectorFunc(state.Value); }
			}

			private readonly IState<TInput> state;
			private readonly Func<TInput, TResult> selectorFunc;

			public DerivedState(IState<TInput> state, Func<TInput, TResult> selectorFunc)
			{
				this.state = state;
				this.selectorFunc = selectorFunc;
			}
		}

		public IEnumerable<IState> BoundStates
		{
			get { return boundStates.Keys; }
		}
		private readonly Dictionary<IState, ValueSelector> boundStates = new Dictionary<IState, ValueSelector>();
		private readonly Dictionary<IState, object> renderedState = new Dictionary<IState, object>();

		protected void BindToState(params IState[] states)
		{
			foreach (var state in states)
			{
				boundStates.Add(state, new ValueSelector());
			}
		}

		protected IDerivedState<TResult> BindToDerivedState<TInput, TResult>(IState<TInput> state, Func<TInput, TResult> derive)
		{
			var derivedState = new DerivedState<TInput, TResult>(state, derive);
			boundStates.Add(state, ValueSelector.Create(derive));
			return derivedState;
		}

		public void Update()
		{
			if (ShouldReRender())
			{
				InternalRender();
			}

			foreach (var boundState in boundStates.Keys)
			{
				renderedState[boundState] = boundStates[boundState].ComputeValue(boundState);
			}
		}

		protected void Init()
		{
			InternalRender();
		}

		protected abstract VisualElement[] Render();

		private bool ShouldReRender()
		{
			foreach (var boundState in boundStates.Keys)
			{
				var effectiveValue = boundStates[boundState].ComputeValue(boundState);
				if (!renderedState.ContainsKey(boundState) || !Equals(effectiveValue, renderedState[boundState]))
				{
					return true;
				}
			}

			return false;
		}

		private void InternalRender()
		{
			var children = Render();
			Clear();
			if (children != null)
			{
				foreach (var child in children)
				{
					if (child != null)
					{
						Add(child);
					}
				}
			}
		}
	}
}