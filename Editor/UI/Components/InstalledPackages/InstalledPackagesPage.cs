using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components.InstalledPackages
{
	internal class InstalledPackagesPage : VisualElement
	{
		public InstalledPackagesPage()
		{
#if UNITY_2018_3_OR_NEWER
			style.flexGrow = 1f;
#else
			style.flex = 1f;
#endif
			Add(new Overlay(new LoadingIndicator("Coming Soon")));
		}
	}
}