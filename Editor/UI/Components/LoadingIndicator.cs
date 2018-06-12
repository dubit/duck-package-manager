using DUCK.PackageManager.Editor.UI.Styles;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class LoadingIndicator : Label
	{
		public LoadingIndicator(string text = null) : base(text)
		{
			style.flex = 1;
			style.textAlignment = TextAnchor.MiddleCenter;
			style.fontSize = FontSizes.infoLabel;
			style.textColor = Color.white;
		}
	}
}