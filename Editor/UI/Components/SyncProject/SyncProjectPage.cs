using System;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.Tasks;
using DUCK.PackageManager.Editor.UI.Utils;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace DUCK.PackageManager.Editor.UI.SyncProject
{
	internal class SyncProjectPage : VisualElement
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

			//
		}

		private void HandleSyncButtonClicked()
		{

		}
	}
}