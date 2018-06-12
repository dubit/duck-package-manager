using DUCK.PackageManager.Editor.UI.Components.AddCustomPackage;
using DUCK.PackageManager.Editor.UI.Components.AvailablePackages;
using DUCK.PackageManager.Editor.UI.Components.InstalledPackages;
using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.SyncProject;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class TabPageContainer : StateBoundComponent
	{
		private readonly TabPageStore tabPages = RootStore.Instance.TabPages;
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly VisualElement availablePackagesPage;
		private readonly VisualElement installedPackagesPage;
		private readonly VisualElement addCustomPackagePage;
		private readonly VisualElement syncProjectPage;

		private readonly VisualElement loadingOverlay;
		private readonly LoadingIndicator loadingText;

		public TabPageContainer()
		{
			BindToState(
				tabPages.CurrentPage,
				packages.IsWorking
			);

			style.flex = 1;
			style.backgroundColor = Colors.foreground;

			availablePackagesPage = new AvailablePackagesPage();
			installedPackagesPage = new InstalledPackagesPage();
			addCustomPackagePage = new AddCustomPackagePage();
			syncProjectPage = new SyncProjectPage();
			loadingOverlay = new Overlay(loadingText = new LoadingIndicator());

			Init();
		}

		protected override VisualElement[] Render()
		{
			var currentPage = tabPages.CurrentPage.Value;
			VisualElement currentPageComponent = null;
			switch (currentPage)
			{
				case Tabs.AVAILABLE_PACKAGES:
					currentPageComponent = availablePackagesPage;
					break;
				case Tabs.INSTALLED_PACKAGES:
					currentPageComponent = installedPackagesPage;
					break;
				case Tabs.ADD_CUSTOM_PACKAGE:
					currentPageComponent = addCustomPackagePage;
					break;
				case Tabs.SYNC_PROJECT_PAGE:
					currentPageComponent = syncProjectPage;
					break;
			}

			var isWorking = packages.IsWorking.Value;

			var shouldDisplayOverlay = isWorking;
			if (shouldDisplayOverlay)
			{
				loadingText.text = packages.Operation.Value;
			}

			return new []
			{
				currentPageComponent,
				shouldDisplayOverlay ? loadingOverlay : null
			};
		}
	}
}