using System.Diagnostics.SymbolStore;
using DUCK.PackageManager.Editor.UI.Components;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.SyncProject
{
	public class SyncProjectPage : VisualElement
	{
		public SyncProjectPage()
		{
			style.flex = 1;
			style.alignItems = Align.Center;
			style.paddingTop = 32;

			var syncButton = new Button(HandleSyncButtonClicked);
			syncButton.text = "Sync Packages";
			syncButton.Padding(8);
			Add(syncButton);

			Add(new Overlay(new LoadingIndicator("Coming Soon")));
		}

		private void HandleSyncButtonClicked()
		{
		}
	}
}