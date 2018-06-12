using System;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Tasks;

namespace DUCK.PackageManager.Editor.Git
{
	internal class GetGitBranchOrTagTask : ITask
	{
		public string Result { get; private set; }

		private readonly InstalledPackage package;

		public GetGitBranchOrTagTask(InstalledPackage package)
		{
			this.package = package;
		}

		public void Execute(Action onComplete = null)
		{
			var workingDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;

			var symbolic = new GitTask("symbolic-ref -q --short HEAD");
			symbolic.WorkingDirectory = workingDirectory;
			symbolic.Execute(() =>
			{
				if (symbolic.ReceivedError || symbolic.StdOut.Count == 0)
				{
					var tags = new GitTask("describe --tags --exact-match");
					tags.WorkingDirectory = workingDirectory;
					tags.Execute(() =>
					{
						Result = tags.StdOut[0];
						if (onComplete != null) onComplete();
					});
					return;
				}

				Result = symbolic.StdOut[0];
				if (onComplete != null) onComplete();
			});
		}
	}
}