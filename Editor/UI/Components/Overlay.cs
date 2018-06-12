using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class Overlay : VisualElement
	{
		public Overlay(params VisualElement[] children)
		{
			style.positionTop = 0;
			style.positionRight = 0;
			style.positionBottom  = 0;
			style.positionLeft = 0;
			style.positionType = PositionType.Absolute;
			style.opacity = 0.5f;
			style.backgroundColor = Color.black;

			foreach (var visualElement in children)
			{
				Add(visualElement);
			}
		}
	}
}