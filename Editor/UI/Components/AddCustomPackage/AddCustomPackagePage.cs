using System.Linq;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components.AddCustomPackage
{
	internal class AddCustomPackagePage : VisualElement
	{
		private TextField nameInput;
		private TextField urlInput;
		private TextField versionInput;
		private Label errorLabel;

		public AddCustomPackagePage()
		{
			style.flexGrow = 1;
			style.paddingTop = 16;

			var nameField = new Label("Package Name (Can be anything as long as it's unique)");
			nameInput = new TextField();
			nameInput.style.Margin(4);
			nameInput.style.Padding(4);
			nameInput.style.fontSize = FontSizes.textInput;

			var urlLabel = new Label("Git URL (Must be https)");
			urlLabel.style.marginTop = 4;
			urlInput = new TextField();
			urlInput.style.Margin(4);
			urlInput.style.Padding(4);
			urlInput.style.fontSize = FontSizes.textInput;

			var versionLabel = new Label("Version (Optional) (Git tag or branch)");
			versionLabel.style.marginTop = 4;
			versionInput = new TextField();
			versionInput.style.Margin(4);
			versionInput.style.Padding(4);
			versionInput.style.fontSize = FontSizes.textInput;

			errorLabel = new Label();
			errorLabel.style.marginTop = 4;
			errorLabel.style.marginBottom = 4;

			var addButton = new Button(HandleAddButtonClicked);
			addButton.text = "Add";
			addButton.style.marginTop = 8;
			addButton.style.width = 100;
			addButton.style.alignSelf = Align.FlexStart;

			Add(nameField);
			Add(nameInput);
			Add(urlLabel);
			Add(urlInput);
			Add(versionLabel);
			Add(versionInput);
			Add(errorLabel);
			Add(addButton);
		}

		private void HandleAddButtonClicked()
		{
			// clear error
			errorLabel.text = "";

			var name = nameInput.value;
			var url = urlInput.value;

			if (string.IsNullOrEmpty(name))
			{
				errorLabel.text = "Error! Package name cannot be empty";
				return;
			}

			var installedPackages = RootStore.Instance.Packages.InstalledPackages;
			// name must not be already used by another installed package
			if (installedPackages.Value.Packages.Any(p => p.Name == name))
			{
				errorLabel.text = "Error! A package with this name already exists";
				return;
			}

			if (string.IsNullOrEmpty(url))
			{
				errorLabel.text = "Error! Git url cannot be empty";
				return;
			}

			// url must start with https and end
			if (!url.StartsWith("https://") || !url.EndsWith(".git"))
			{
				errorLabel.text = "Error! git url must be a https git url";
				return;
			}

			// If we get to here we can install
			Actions.InstallCustomPackage(name, url, versionInput.value);
		}
	}
}
