using System;

namespace DUCK.PackageManager.Editor.Git
{
	internal class AddSubmoduleTask : GitTask
	{
		private const string COMMAND = "submodule add {0} {1}";

		public AddSubmoduleTask(string gitUrl, string installDirectory)
			:base(string.Format(COMMAND, gitUrl, installDirectory))
		{
			if (string.IsNullOrEmpty(gitUrl)) throw new ArgumentException("Canont be null or empty", "gitUrl");
			if (string.IsNullOrEmpty(installDirectory)) throw new ArgumentException("Canont be null or empty", "installDirectory");
		}
	}
}