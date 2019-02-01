using System;
using DUCK.PackageManager.Editor.UI.Styles;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace DUCK.PackageManager.Editor.UI.Components
{
	internal class TabButton : VisualElement
	{
		public string ID { get; set; }
		public bool IsActive { get; set; }

		private readonly Label label;

		private bool isMouseOver;

		public event Action<TabButton> OnClick;

		public TabButton(string id, string text, bool isActive = false)
		{
			ID = id;

			style.marginRight = 4;

			label = new Label(text);
			label.style.fontSize = 14;
#if UNITY_2018_3_OR_NEWER
			label.style.flexGrow = 1f;
#else
			label.style.flex = 1f;
#endif
			label.style.textAlignment = TextAnchor.MiddleCenter;

			this.AddManipulator(new Clickable(HandleClick));
			RegisterCallback<MouseEnterEvent>(HandleMousEnter);
			RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);

			Add(label);

			SetActive(isActive);
		}

		private void HandleMouseLeave(MouseLeaveEvent evt)
		{
			isMouseOver = false;
			UpdateStyle();
		}

		private void HandleMousEnter(MouseEnterEvent evt)
		{
			isMouseOver = true;
			UpdateStyle();
		}

		private void HandleClick()
		{
			if (OnClick != null)
			{
				OnClick(this);
			}
		}

		public void SetActive(bool active)
		{
			IsActive = active;
			UpdateStyle();
		}

		private void UpdateStyle()
		{
			var backgroundColor = IsActive ?
				Colors.activeTabBackground:
				isMouseOver ? Colors.hoveredTabBackground : Colors.inactiveTabBackground;

			label.style.textColor = IsActive ? Colors.activeTabText : Colors.inactiveTabText;

			style.MarginVertical(IsActive ? 0 : 4);
			style.PaddingVertical(IsActive ? 4 : 0);

			style.backgroundColor = backgroundColor;
		}
	}
}