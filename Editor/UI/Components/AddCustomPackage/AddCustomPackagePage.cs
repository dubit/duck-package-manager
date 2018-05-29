using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components.AddCustomPackage
{
	public class AddCustomPackagePage : VisualElement
	{
		public AddCustomPackagePage()
		{
			style.flex = 1;
			style.paddingTop = 16;

			var nameField = new Label("Package Name (Can be anything as long as it's unique)");
			var nameInput = new TextField();
			nameInput.style.Margin(4);
			nameInput.style.Padding(4);
			nameInput.style.fontSize = FontSizes.textInput;

			var urlLabel = new Label("Git URL");
			urlLabel.style.marginTop = 4;
			var urlInput = new TextField();
			urlInput.style.Margin(4);
			urlInput.style.Padding(4);
			urlInput.style.fontSize = FontSizes.textInput;

			var addButton = new Button(HandleAddButtonClicked);
			addButton.text = "Add";
			addButton.style.marginTop = 8;
			addButton.style.width = 100;
			addButton.style.alignSelf = Align.FlexStart;

			Add(nameField);
			Add(nameInput);
			Add(urlLabel);
			Add(urlInput);
			Add(addButton);

			Add(new Overlay(new LoadingIndicator("Coming Soon")));
		}

		private void HandleAddButtonClicked()
		{
		}
	}
}