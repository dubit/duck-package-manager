using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components.InstalledPackages
{
	public class InstalledPackagesPage : VisualElement
	{
		public InstalledPackagesPage()
		{
			style.flex = 1;
			Add(new Overlay(new LoadingIndicator("Coming Soon")));
		}
	}
}