using System;
using DUCK.PackageManager.Editor.Tasks;

namespace DUCK.PackageManager.Editor.Git
{
	internal class GitTask : AbstractProcessTask
	{
		public static GitTask Run(string args)
		{
			var task = new GitTask(args);
			task.Execute(r => {});
			return task;
		}

		private readonly string arguments;

		public GitTask(string args)
		{
			if (args == null) throw new ArgumentNullException("args");
			arguments = args;
		}

		protected override void Run()
		{
			RunProcessWithArgs(
				Settings.GitExecutablePath,
				arguments);
		}
	}
}