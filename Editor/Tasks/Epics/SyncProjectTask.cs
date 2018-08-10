using System;
using DUCK.PackageManager.Editor;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using UnityEngine;

namespace DUCK.Tasks.Epics
{
	internal static partial class ActionEpics
	{
		public static AsyncOperation SyncProject()
		{
			var tasks = new AsyncOperation();

			PackageListStatus packageListStatus = null;

			// Compile package list status
			tasks.Add(new BasicTask<OperationResult>(CompileProjectPackageListStatus), (result, op) =>
			{
				if (result.IsError)
				{
					Debug.LogError("Failed to compile package status");
					Debug.LogError(result.Error.Message);
					return;
				}

				packageListStatus = ((Result<PackageListStatus>) result).Data;
			});

			// Sync project
			tasks.Add(new BasicTask<OperationResult>(onComplete => SyncProjectInternal(packageListStatus, onComplete)));

			return tasks;
		}

		private static void CompileProjectPackageListStatus(Action<OperationResult> onComplete)
		{
			var compilePackageListStatus = CompilePackageListStatus();
			compilePackageListStatus.Execute(onComplete);
		}

		private static void SyncProjectInternal(PackageListStatus packageListStatus, Action<OperationResult> onComplete)
		{
			if (packageListStatus.IsProjectUpToDate)
			{
				onComplete(new OperationResult());
				return;
			}

			var operation = new AsyncOperation();

			foreach (var packageStatus in packageListStatus.Packages)
			{
				var version = packageStatus.RequiredVersion;
				var packageName = packageStatus.PackageName;
				var gitUrl = packageStatus.GitUrl;
				var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + packageName;
				var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + packageName;

				if (packageStatus.IsMissing)
				{
					operation.Add(new AddSubmoduleTask(gitUrl, relativeInstallDirectory));
					operation.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
				}
				else if (packageStatus.IsOnWrongVersion)
				{
					operation.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
				}
			}

			operation.Execute(onComplete);
		}
	}
}