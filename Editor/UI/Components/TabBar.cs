using DUCK.PackageManager.Editor.UI.Flux;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class TabBar : VisualElement
	{
		private TabButton[] tabs;
		private TabButton activeTab;

		public TabBar()
		{
			style.height = 32;
			style.flexDirection = FlexDirection.Row;

			AddTabButtons(
				new TabButton(Tabs.AVAILABLE_PACKAGES, "Available Packages", true),
				new TabButton(Tabs.INSTALLED_PACKAGES, "Installed Packages"),
				new TabButton(Tabs.ADD_CUSTOM_PACKAGE, "Add Custom Package"),
				new TabButton(Tabs.SYNC_PROJECT_PAGE, "Sync Project")
			);
		}

		private void AddTabButtons(params TabButton[] tabs)
		{
			this.tabs = tabs;

			foreach (var tab in tabs)
			{
				Add(tab);
				tab.OnClick += HandleTabSelected;

				if (tab.IsActive)
				{
					activeTab = tab;
				}
			}
		}

		private void HandleTabSelected(TabButton selected)
		{
			foreach (var tab in tabs)
			{
				tab.SetActive(tab == selected);
			}

			if (selected != activeTab)
			{
				activeTab = selected;
				Dispatcher.Dispatch(new Action(ActionTypes.CHANGE_TAB_PAGE, selected.ID));
			}
		}
	}
}