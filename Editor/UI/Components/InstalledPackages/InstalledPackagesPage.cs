using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components.InstalledPackages
{
	internal class InstalledPackagesPage : VisualElement
	{
		public InstalledPackagesPage()
		{
			style.flex = 1;
			Add(new Overlay(new LoadingIndicator("Coming Soon")));
		}
	}
}