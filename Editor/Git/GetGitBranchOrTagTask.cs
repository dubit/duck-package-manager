using System;
using DUCK.PackageManager.Editor.Data;
using DUCK.Tasks;

namespace DUCK.PackageManager.Editor.Git
{
	internal class GetGitBranchOrTagTask : ITask<Result<string>>
	{
		private readonly InstalledPackage package;

		public GetGitBranchOrTagTask(InstalledPackage package)
		{
			this.package = package;
		}

		public void Execute(Action<Result<string>> onComplete)
		{
			var workingDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;

			var symbolic = new GitTask("symbolic-ref -q --short HEAD");
			symbolic.WorkingDirectory = workingDirectory;
			symbolic.Execute(symbolicResult =>
			{
				if (symbolicResult.IsError || symbolicResult.StdOut.Count == 0)
				{
					var tags = new GitTask("describe --tags --exact-match");
					tags.WorkingDirectory = workingDirectory;
					tags.Execute(tagsResult =>
					{
						onComplete(new Result<string>(tagsResult.StdOut[0]));
					});
					return;
				}

				onComplete(new Result<string>(symbolicResult.StdOut[0]));
			});
		}
	}
}