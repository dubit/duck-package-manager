using DUCK.PackageManager.Editor.UI.Flux.Components;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.PackageManager.Editor.UI.Styles;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class PackageListView : StateBoundComponent
	{
		private readonly PackageStore packages = RootStore.Instance.Packages;

		private readonly Label errorLabel;
		private readonly ScrollView scrollView;

		public PackageListView()
		{
			BindToState(
				packages.AvailablePackages,
				packages.Error,
				packages.SearchQuery
			);

#if UNITY_2018_3_OR_NEWER
			style.flexGrow = 1f;
#else
			style.flex = 1f;
#endif

			errorLabel = new Label();
#if UNITY_2018_3_OR_NEWER
			errorLabel.style.flexGrow = 1f;
#else
			errorLabel.style.flex = 1;
#endif
			errorLabel.style.fontSize = FontSizes.infoLabel;
			errorLabel.style.textColor = Color.red;

			scrollView = new ScrollView();
#if UNITY_2018_3_OR_NEWER
			scrollView.style.flexGrow = 1f;
#else
			scrollView.style.flex = 1f;
#endif
			scrollView.contentContainer.style.positionRight = 0;
			scrollView.contentContainer.style.positionLeft = 0;

			Init();
		}

		protected override VisualElement[] Render()
		{
			var error = packages.Error.Value;
			var packageList = packages.AvailablePackages.Value;
			var searchQuery = packages.SearchQuery.Value;

			// manipulate state of components
			errorLabel.text = error;

			if (packageList != null)
			{
				scrollView.Clear();

				foreach (var package in packageList.Packages)
				{
					if (searchQuery == null || package.Name.ToLower().Contains(searchQuery.ToLower()))
					{
						scrollView.Add(
							new PackageView(package)
						);
					}
				}
			}

			return new VisualElement[]
			{
				error != null ? errorLabel : null,
				packageList != null && error == null ? scrollView : null,
			};
		}
	}
}