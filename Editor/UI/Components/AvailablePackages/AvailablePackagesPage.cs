using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components.AvailablePackages
{
	internal class AvailablePackagesPage : StateBoundComponent
	{
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly SearchBar searchBar;
		private readonly PackageListView packageListView;
		private readonly LoadingIndicator loadingIndicator;

		public AvailablePackagesPage()
		{
			style.flex = 1;

			BindToState(
				packages.IsFetchingPackages
			);

			loadingIndicator = new LoadingIndicator("Fetching Packages...");
			searchBar = new SearchBar();
			packageListView = new PackageListView();

			Init();
		}

		protected override VisualElement[] Render()
		{
			var isFetching = packages.IsFetchingPackages.Value;

			return new VisualElement[]
			{
				isFetching ? loadingIndicator : null,
				isFetching ? null : searchBar,
				isFetching ? null : packageListView,
			};
		}
	}
}