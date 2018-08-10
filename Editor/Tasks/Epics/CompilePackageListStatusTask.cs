using System.IO;
using DUCK.PackageManager.Editor;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.UI.Stores;
using DUCK.Tasks.Extensions;
using UnityEngine;

namespace DUCK.Tasks.Epics
{
	internal static partial class ActionEpics
	{
		public static AsyncOperation CompilePackageListStatus()
		{
			var operation = new AsyncOperation();
			var packageListStatus = new PackageListStatus();

			foreach (var package in RootStore.Instance.Packages.InstalledPackages.Value.Packages)
			{
				var packageStatus = new PackageStatus
				{
					RequiredVersion = package.Version,
					PackageName = package.Name,
					GitUrl = package.GitUrl
				};
				packageListStatus.Packages.Add(packageStatus);
				var packageDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;
				if (!Directory.Exists(packageDirectory))
				{
					packageStatus.IsMissing = true;
					continue;
				}

				var getGitBranchOrTagTask = new GetGitBranchOrTagTask(package);
				var packageVersion = package.Version;
				operation.Add(getGitBranchOrTagTask, (result, op) =>
				{
					packageStatus.IsOnWrongVersion = result.Data != packageVersion;
				});
			}

			operation.AddSync(() =>
			{
				Debug.Log("Project package status: ");

				foreach (var package in packageListStatus.Packages)
				{
					Debug.Log(package);
				}

				Debug.Log("The project is " +
					(packageListStatus.IsProjectUpToDate ? "" : "not ")+ "up to date");

				return new Result<PackageListStatus>(packageListStatus);
			}, (result, op) =>
			{
				op.Complete(result);
			});

			return operation;
		}
	}
}