using System;
using UnityEngine;

namespace DUCK.PackageManager.Editor.Data
{
	[Serializable]
	internal class AvailablePackageList
	{
		[SerializeField]
		private AvailablePackage[] packages;
		public AvailablePackage[] Packages
		{
			get { return packages; }
		}
	}

	[Serializable]
	internal class AvailablePackage
	{
		[SerializeField]
		private string name;
		public string Name
		{
			get { return name; }
		}

		[SerializeField]
		private string gitUrl;
		public string GitUrl
		{
			get { return gitUrl; }
		}

		[SerializeField]
		private string[] versions;
		public string[] Versions
		{
			get { return versions; }
		}
	}
}