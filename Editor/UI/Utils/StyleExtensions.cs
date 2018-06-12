using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Utils
{
	internal static class StyleExtensions
	{
		public static void Padding(this IStyle style, int padding)
		{
			style.paddingTop = padding;
			style.paddingBottom = padding;
			style.paddingLeft = padding;
			style.paddingRight = padding;
		}

		public static void Padding(this IStyle style, int t, int r, int b, int l)
		{
			style.paddingTop = t;
			style.paddingRight = r;
			style.paddingBottom = b;
			style.paddingLeft = l;
		}

		public static void PaddingVertical(this IStyle style, int padding)
		{
			style.paddingTop = padding;
			style.paddingBottom = padding;
		}

		public static void PaddingHorizontal(this IStyle style, int padding)
		{
			style.paddingLeft = padding;
			style.paddingRight = padding;
		}

		public static void Margin(this IStyle style, int margin)
		{
			style.marginTop = margin;
			style.marginBottom = margin;
			style.marginLeft = margin;
			style.marginRight = margin;
		}

		public static void Margin(this IStyle style, int t, int r, int b, int l)
		{
			style.marginTop = t;
			style.marginRight = r;
			style.marginBottom = b;
			style.marginLeft = l;
		}

		public static void MarginVertical(this IStyle style, int margin)
		{
			style.marginTop = margin;
			style.marginBottom = margin;
		}

		public static void MarginHorizontal(this IStyle style, int margin)
		{
			style.marginLeft = margin;
			style.marginRight = margin;
		}
	}
}