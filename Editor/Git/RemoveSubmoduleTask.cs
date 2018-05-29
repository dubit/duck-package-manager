using System;
using System.Collections.Generic;
using System.IO;
using DUCK.PackageManager.Editor.Tasks;
using UnityEditor;
using UnityEngine;

namespace DUCK.PackageManager.Editor.Git
{
	public class RemoveSubmoduleTask : ITask
	{
		private readonly string localRelativeSubmodulePath;

		public RemoveSubmoduleTask(string localRelativeSubmodulePath)
		{
			if (localRelativeSubmodulePath == null) throw new ArgumentNullException("localRelativeSubmodulePath");
			this.localRelativeSubmodulePath = localRelativeSubmodulePath;
		}

		public void Execute(Action onComplete = null)
		{
			var projectRoot = Project.RootDirectory;

			var taskChain = new TaskChain()
				// Remove from gitmodules
				.Add(() => { RemoveFromGitConfig(projectRoot + "/.gitmodules", localRelativeSubmodulePath); })
				// stage that change
				.Add(new GitTask("add .gitmodules"))
				// remove lines in .git/config
				.Add(() => { RemoveFromGitConfig(projectRoot + "/.git/config", localRelativeSubmodulePath); })
				// Delete the actual files
				.Add(new GitTask("rm --cached -f " + localRelativeSubmodulePath))
				// Run rm -rf .git/modules/path_to_submodule (no trailing slash).
				.Add(() =>
				{
					var directory = projectRoot + "/.git/modules/" + localRelativeSubmodulePath;
					FileUtil.DeleteFileOrDirectory(directory);
				})
				// Delete the now untracked submodule files rm -rf path_to_submodule
				.Add(() =>
				{
					var directory = projectRoot + "/" + localRelativeSubmodulePath;
					FileUtil.DeleteFileOrDirectory(directory);
				})
				.Add(() =>
				{
					Debug.Log("Successfully removed submodule: " + localRelativeSubmodulePath);
				});

			taskChain.Execute(onComplete);
		}

		private void RemoveFromGitConfig(string file, string localRelativeSubmodulePath)
		{
			var header = string.Format("[submodule \"{0}\"]", localRelativeSubmodulePath);

			var outputLines = new List<string>();
			var lines = File.ReadAllLines(file);

			var remove = false;

			for (var i = 0; i < lines.Length; i++)
			{
				if (lines[i] == header)
				{
					remove = true;
				}
				else if (remove && !lines[i].StartsWith("\t"))
				{
					remove = false;
				}

				if (!remove)
				{
					outputLines.Add(lines[i]);
				}
			}

			File.WriteAllLines(file, outputLines.ToArray());
		}
	}
}