using System.Collections.Generic;
using System.Linq;
using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace DUCK.PackageManager.Editor.UI.Flux
{
	internal class FluxRootComponent : VisualElement
	{
		private readonly Store store;

		public FluxRootComponent(Store rootStore)
		{
			store = rootStore;
#if UNITY_2018_3_OR_NEWER
			style.flexGrow = 1f;
#else
			style.flex = 1f;
#endif
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