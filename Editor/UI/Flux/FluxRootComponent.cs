using System.Collections.Generic;
using System.Linq;
using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Flux
{
	public class FluxRootComponent : VisualElement
	{
		private readonly Store store;

		public FluxRootComponent(Store rootStore)
		{
			store = rootStore;
			style.flex = 1;
			Dispatcher.OnActionDispatched += HandleDispatch;
		}

		private void HandleDispatch(Action action)
		{
			var dirtyState = store.Dispatch(action);

			UpdateChildren(this, dirtyState);
		}

		private void UpdateChildren(VisualElement element, List<IState> dirtyState)
		{
			foreach (var child in element.Children().ToList())
			{
				var stateBoundComponent = child as StateBoundComponent;
				if (stateBoundComponent != null)
				{
					if (stateBoundComponent.BoundStates.Any(dirtyState.Contains))
					{
						stateBoundComponent.Update();
					}
				}

				UpdateChildren(child, dirtyState);
			}
		}
	}
}