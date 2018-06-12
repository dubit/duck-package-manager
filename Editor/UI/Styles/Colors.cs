using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace DUCK.PackageManager.Editor.UI.Styles
{
	public static class Colors
	{
		public static readonly Color background = C("#2D2D2D");
		public static readonly Color foreground = C("#474747");
		public static readonly Color activeTabBackground = foreground;
		public static readonly Color inactiveTabBackground = C("#373737");
		public static readonly Color hoveredTabBackground = activeTabBackground * 1.1f;
		public static readonly Color activeTabText = C("#FFFFFF");
		public static readonly Color inactiveTabText = C("#878787");

		public static readonly Color packageRowBackground = C("#878787");
		public static readonly Color packageRowText = C("#2D2D2D");
		public static readonly Color errorText = C("#DD0000");

		private static Color C(string hex)
		{
			Color color;
			return ColorUtility.TryParseHtmlString(hex, out color) ? color : Color.black;
		}
	}
}