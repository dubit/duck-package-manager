using UnityEngine;

namespace DUCK.PackageManager.Editor
{
	internal static class Project
	{
		public static string RootDirectory
		{
			get { return Application.dataPath.Replace("/Assets", ""); }
		}
	}
}