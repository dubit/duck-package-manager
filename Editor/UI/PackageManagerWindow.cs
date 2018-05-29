using DUCK.PackageManager.Editor.UI.Components;
using DUCK.PackageManager.Editor.UI.Flux;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEditor;
using UnityEditor.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI
{
	public class PackageManagerWindow : EditorWindow
	{
		[MenuItem("DUCK/Package Manager")]
		public static void ShowWindow()
		{
			GetWindow<PackageManagerWindow>(false, "DuckPackages", true);
		}

		private void OnEnable()
		{
			var root = this.GetRootVisualContainer();

			var store = RootStore.Instance;

			var fluxComponent = new FluxRootComponent(store)
			{
				new TabBar(),
				new TabPageContainer()
			};

			fluxComponent.style.backgroundColor = Colors.background;
			fluxComponent.style.Margin(4);

			root.Add(fluxComponent);

			Actions.ReadPackagesJson();
			Actions.FetchPackages();
		}
	}
}