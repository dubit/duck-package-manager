using DUCK.PackageManager.Editor.UI.Flux;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components
{
	public class SearchBar : VisualElement
	{
		private readonly TextField input;

		public SearchBar()
		{
			style.height = 32;
			style.flexDirection = FlexDirection.Row;

			var label = new Label("Filter...");
			label.style.textAlignment = TextAnchor.MiddleLeft;
			Add(label);

			input = new TextField();
			input.style.Margin(4);
			input.style.fontSize = FontSizes.textInput;
			input.style.flex = 1;

			input.RegisterCallback<KeyUpEvent>(HandleValueChanged);

			Add(input);
		}

		private void HandleValueChanged(KeyUpEvent evt)
		{
			Dispatcher.Dispatch(new Action(ActionTypes.PACKAGE_LIST_SEARCH_CHANGED, input.text));
		}
	}
}