using System;
using System.Collections.Generic;
using System.IO;
using DUCK.PackageManager.Editor;
using DUCK.PackageManager.Editor.Git;
using DUCK.Tasks.Extensions;
using UnityEditor;

namespace DUCK.Tasks.Epics
{
	internal static partial class ActionEpics
	{
		public static AsyncOperation RemoveSubmodule(string localRelativeSubmodulePath)
		{
			if (localRelativeSubmodulePath == null) throw new ArgumentNullException("localRelativeSubmodulePath");

			var projectRoot = Project.RootDirectory;

			var operation = new AsyncOperation();

			// Remove from gitmodules
			operation.AddSync(() =>
			{
				RemoveFromGitConfig(projectRoot + "/.gitmodules", localRelativeSubmodulePath);
			});

			// stage that change
			operation.Add(new GitTask("add .gitmodules"));

			// remove lines in .git/config
			operation.AddSync(() =>
			{
				RemoveFromGitConfig(projectRoot + "/.git/config", localRelativeSubmodulePath);
			});

			// Delete the actual files
			operation.Add(new GitTask("rm --cached -f " + localRelativeSubmodulePath));

			// Run rm -rf .git/modules/path_to_submodule (no trailing slash).
			operation.AddSync(() =>
			{
				var directory = projectRoot + "/.git/modules/" + localRelativeSubmodulePath;
				FileUtil.DeleteFileOrDirectory(directory);
			});

			// Delete the now untracked submodule files rm -rf path_to_submodule
			operation.AddSync(() =>
			{
				var directory = projectRoot + "/" + localRelativeSubmodulePath;
				FileUtil.DeleteFileOrDirectory(directory);
			});

			return operation;
		}

		private static void RemoveFromGitConfig(string file, string localRelativeSubmodulePath)
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