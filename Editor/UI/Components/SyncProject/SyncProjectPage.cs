using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.SyncProject
{
	internal class SyncProjectPage : StateBoundComponent
	{
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly Label upToDateText;
		private readonly Button syncButton;

		public SyncProjectPage()
		{
			style.flex = 1;
			style.alignItems = Align.Center;
			style.paddingTop = 32;

			upToDateText = new Label();
			upToDateText.text = "Your project is up to date.";
			upToDateText.style.fontSize = 16;
			upToDateText.Padding(8);

			syncButton = new Button(HandleSyncButtonClicked);
			syncButton.text = "Sync Packages";
			syncButton.Padding(8);

			BindToState(packages.PackageListStatus);

			Init();
		}

		private void HandleSyncButtonClicked()
		{
			Actions.SyncProject();
		}

		protected override VisualElement[] Render()
		{
			var isUpToDate = packages.PackageListStatus.Value != null && packages.PackageListStatus.Value.IsProjectUpToDate;

			return new VisualElement[]
			{
				isUpToDate ? null : syncButton,
				isUpToDate ? upToDateText : null,
			};
		}
	}
}