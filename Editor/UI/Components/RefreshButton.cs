using DUCK.PackageManager.Editor.UI.Flux;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	public class RefreshButton : VisualElement
	{
		public RefreshButton()
		{
			var button = new Button(HandleClicked);
			Add(button);
			button.text = "Refresh";
		}

		private void HandleClicked()
		{
			Actions.ReadPackagesJson();
			Actions.FetchPackages();
		}
	}
}