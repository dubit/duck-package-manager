using System.Linq;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Tasks.Web;
using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Styles;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class PackageView : StateBoundComponent
	{
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly IDerivedState<bool> isPackageInstalled;

		private readonly AvailablePackage package;
		private readonly Label packageName;
		private readonly Label packageVersion;
		private readonly Button infoButton;
		private readonly Button installButton;
		private readonly Button switchVersionButton;
		private readonly Label installedLabel;
		private readonly Button removeButton;
		private readonly PopupField<string> versionSelector;

		public PackageView(AvailablePackage package)
		{
			this.package = package;

			style.height = 32;
			style.flexDirection = FlexDirection.Row;
			style.marginBottom = style.marginTop = style.marginLeft = style.marginRight = 4;
			style.alignItems = Align.Center;
			style.borderRadius = 4;

			packageName = new Label(package.Name);
			packageName.style.fontSize = FontSizes.packageRow;
#if UNITY_2018_3_OR_NEWER
			packageName.style.flexGrow = 1f;
#else
			packageName.style.flex = 1f;
#endif
			packageName.style.textColor = Colors.packageRowText;

			versionSelector = new PopupField<string>(package.Versions.ToList(), package.Versions.Last());
			versionSelector.style.width = 100;
			versionSelector.style.height = 24;
			versionSelector.style.paddingRight = 12;
			versionSelector.style.textAlignment = TextAnchor.MiddleCenter;
			versionSelector.style.fontSize = FontSizes.packageRow;
			versionSelector.OnValueChanged(HandleVersionSelectorValueChanged);

			infoButton = new Button(HandleInfoClicked);
			infoButton.style.width = 40;
			infoButton.text = "Info";

			installButton = new Button(HandleInstallClicked);
			installButton.style.width = 60;
			installButton.text = "Install";

			switchVersionButton = new Button(HandleSwitchVersionClicked);
			switchVersionButton.style.width = 60;
			switchVersionButton.text = "Install";

			installedLabel = new Label("Installed");
			installedLabel.style.width = 60;
			installedLabel.style.fontSize = FontSizes.packageRow;
			installedLabel.style.textColor = Colors.packageRowText;

			removeButton = new Button(HandleRemoveClicked);
			removeButton.style.width = 30;
			removeButton.text = "X";

			style.backgroundColor = Colors.packageRowBackground;

			isPackageInstalled = BindToDerivedState(packages.InstalledPackages,
				installedPackages => installedPackages.Packages.Any(p => p.Name == package.Name));

			Init();
		}

		private void HandleVersionSelectorValueChanged(ChangeEvent<string> evt)
		{
			Init();
		}

		private void HandleRemoveClicked()
		{
			Actions.RemovePackage(package);
		}

		private void HandleInfoClicked()
		{
			Application.OpenURL(package.GitUrl);
		}

		private void HandleInstallClicked()
		{
			Actions.InstallPackage(package, versionSelector.value);
		}

		private void HandleSwitchVersionClicked()
		{
			Actions.SwitchVersion(package, versionSelector.value);
		}

		protected override VisualElement[] Render()
		{
			var isInstalled = isPackageInstalled.Value;
			VisualElement installColumnComponent = installButton;

			var isOnOldVersion = false;

			if (isInstalled)
			{
				var installedPackage = packages.InstalledPackages.Value.Packages.First(p => p.Name == package.Name);
				var versionSelected = versionSelector.value;
				var versionInstalled = installedPackage.Version;
				isOnOldVersion = IsOnOldVersion(versionInstalled);

				if (versionSelected == versionInstalled)
				{
					installColumnComponent = installedLabel;
				}
				else
				{
					installColumnComponent = switchVersionButton;
				}
			}

			packageName.text = package.Name;
			if (isOnOldVersion)
			{
				packageName.text += " (Upgrade available)";
			}

			return new VisualElement[]
			{
				packageName,
				infoButton,
				versionSelector,
				installColumnComponent,
				removeButton,
			};
		}

		private bool IsOnOldVersion(string installedVersion)
		{
			return package.Versions[package.Versions.Length - 1] != installedVersion;
		}
	}
}