using UnityEngine;

namespace DUCK.PackageManager.Editor
{
	public static class Project
	{
		public static string RootDirectory
		{
			get { return Application.dataPath.Replace("/Assets", ""); }
		}
	}
}