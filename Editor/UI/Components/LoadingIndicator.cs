using DUCK.PackageManager.Editor.UI.Styles;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class LoadingIndicator : Label
	{
		public LoadingIndicator(string text = null) : base(text)
		{
#if UNITY_2018_3_OR_NEWER
			style.flexGrow = 1f;
#else
			style.flex = 1f;
#endif
			style.textAlignment = TextAnchor.MiddleCenter;
			style.fontSize = FontSizes.infoLabel;
			style.textColor = Color.white;
		}
	}
}