using System.Collections.Generic;
using System.Linq;

namespace DUCK.PackageManager.Editor.Data
{
	internal class PackageListStatus
	{
		public bool IsProjectUpToDate
		{
			get { return packages.All(p => !p.IsMissing && !p.IsOnWrongVersion); }
		}

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
		public string RequiredVersion { get; set; }
		public string GitUrl { get; set; }

		public override string ToString()
		{
			return PackageName + " is " +
				(IsMissing ? "not installed." : (IsOnWrongVersion ? "on the wrong version." : "ok."));
		}
	}
}