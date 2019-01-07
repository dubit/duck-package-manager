using DUCK.PackageManager.Editor.UI.Flux;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class SearchBar : VisualElement
	{
		private readonly TextField input;

		public SearchBar()
		{
			style.flex = 1;
			style.height = 32;
			style.flexDirection = FlexDirection.Row;

			var label = new Label("Filter...");
			label.style.textAlignment = TextAnchor.MiddleLeft;
			Add(label);

			input = new TextField();
			input.style.Margin(4);
			input.style.fontSize = FontSizes.textInput;
#if UNITY_2018_3_OR_NEWER
			input.style.flexGrow = 1f;
#else
			input.style.flex = 1f;
#endif

			input.RegisterCallback<KeyUpEvent>(HandleValueChanged);

			Add(input);
		}

		private void HandleValueChanged(KeyUpEvent evt)
		{
			Dispatcher.Dispatch(new Action(ActionTypes.PACKAGE_LIST_SEARCH_CHANGED, input.text));
		}
	}
}