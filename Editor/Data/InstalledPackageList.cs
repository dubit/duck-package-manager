using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.PackageManager.Editor.Data
{
	[Serializable]
	public class InstalledPackageList
	{
		[SerializeField]
		private List<InstalledPackage> packages;
		public List<InstalledPackage> Packages
		{
			get { return packages; }
		}

		public InstalledPackageList()
		{
			packages = new List<InstalledPackage>();
		}
	}

	[Serializable]
	public class InstalledPackage
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
		private string version;
		public string Version
		{
			get { return version; }
		}

		public InstalledPackage(string name, string version, string gitUrl)
		{
			this.name = name;
			this.version = version;
			this.gitUrl = gitUrl;
		}
	}
}