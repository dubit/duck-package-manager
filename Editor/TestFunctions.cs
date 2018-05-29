using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.Tasks.Web;
using UnityEditor;
using UnityEngine;

namespace DUCK.PackageManager.Editor
{
	public static class TestFunctions
	{
		[MenuItem("DUCK/Packages/Git Submodule Add")]
		public static void AddSubmodule()
		{
			new AddSubmoduleTask(
				"https://github.com/mminer/consolation.git",
				Settings.RelativePackagesDirectoryPath+ "consolation"
			).Execute();
		}

		[MenuItem("DUCK/Packages/Git Status")]
		public static void GitStatus()
		{
			GitTask.Run("status");
		}

		[MenuItem("DUCK/Packages/Remove Package")]
		public static void RemovePackage()
		{
			new RemoveSubmoduleTask("Assets/Duck/Packages/consolation").Execute();
		}

		[MenuItem("DUCK/Packages/Get Request")]
		public static void GetRequest()
		{
			var task = new HttpGetRequestTask("https://api.myjson.com/bins/qhq3u");
			task.Execute(() =>
			{
				var packageList = JsonUtility.FromJson<AvailablePackageList>(task.Text);
				Debug.Log("Available Packages:");
				foreach (var package in packageList.Packages)
				{
					foreach (var version in package.Versions)
					{
						Debug.Log(package.Name + ":" + version);
					}
				}
			});
		}
	}
}