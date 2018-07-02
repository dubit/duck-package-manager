using System.Collections.Generic;

namespace DUCK.PackageManager.Editor.Data
{
	internal class PackageListStatus
	{
		private List<PackageStatus> packages;
		public List<PackageStatus> Packages
		{
			get { return packages; }
		}

		public PackageListStatus()
		{
			packages = new List<PackageStatus>();
		}
	}

	internal class PackageStatus
	{
		public string PackageName { get; set; }
		public bool IsMissing { get; set; }
		public bool IsOnWrongVersion { get; set; }
	}
}