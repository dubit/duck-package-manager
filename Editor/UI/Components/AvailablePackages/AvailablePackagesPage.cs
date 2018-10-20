using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components.AvailablePackages
{
	internal class AvailablePackagesPage : StateBoundComponent
	{
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly SearchBar searchBar;
		private readonly PackageListView packageListView;
		private readonly LoadingIndicator loadingIndicator;
		private readonly VisualElement topBar;

		public AvailablePackagesPage()
		{
			style.flex = 1;

			BindToState(
				packages.IsFetchingPackages
			);

			loadingIndicator = new LoadingIndicator("Fetching Packages...");
			
			// TopBar
			topBar = new VisualElement();
			topBar.style.height = 32;
			topBar.style.flexDirection = FlexDirection.Row;
			topBar.Add(new RefreshButton());
			topBar.Add(searchBar = new SearchBar());
			packageListView = new PackageListView();

			Init();
		}

		protected override VisualElement[] Render()
		{
			var isFetching = packages.IsFetchingPackages.Value;

			return new VisualElement[]
			{
				isFetching ? loadingIndicator : null,
				isFetching ? null : topBar,
				isFetching ? null : packageListView,
			};
		}
	}
}