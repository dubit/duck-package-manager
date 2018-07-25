using System.IO;
using System.Linq;
using DUCK.PackageManager.Editor.Tasks;

namespace DUCK.PackageManager.Editor.Git
{
	internal class CheckoutSubmoduleTask : GitTask
	{
		private readonly string version;
		private const string COMMAND = "checkout {0}";

		public CheckoutSubmoduleTask(string installDirectory, string version) :
			base(string.Format(COMMAND, version))
		{
			this.version = version;
			WorkingDirectory = installDirectory;
		}

		protected override ProcessTaskResult HandleResult(ProcessTaskResult result)
		{
			if (result.ExitCode != 0)
			{
				if (result.StdErr.Count > 0)
				{
					var folderName = WorkingDirectory.Split(Path.AltDirectorySeparatorChar).Last();
					var errMessage = result.StdErr[0];
					var message = string.Format(
						"Failed to checkout submodule: {0}:{1}\r\n{2}",
						folderName,
						version,
						errMessage);
					result.Err(message);
				}
				else
				{
					result.Err();
				}
			}

			return result;
		}
	}
}