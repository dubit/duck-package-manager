using System;
using System.IO;
using System.Linq;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.Tasks;
using DUCK.PackageManager.Editor.Tasks.Web;
using DUCK.PackageManager.Editor.UI.Flux;
using DUCK.PackageManager.Editor.UI.Stores;
using UnityEngine;
using Action = DUCK.PackageManager.Editor.UI.Flux.Action;

namespace DUCK.PackageManager.Editor.UI
{
	internal static class Actions
	{
		public static void FetchPackages()
		{
			Dispatcher.Dispatch(
				new Action(ActionTypes.REQUEST_PACKAGE_LIST_STARTED));

			var task = new HttpGetRequestTask(Settings.DuckPackagesUrl);
			task.Execute(() =>
			{
				string error = null;
				if (!task.IsError)
				{
					try
					{
						var packageList = JsonUtility.FromJson<AvailablePackageList>(task.Text);

						Dispatcher.Dispatch(
							new Action(ActionTypes.REQUEST_PACKAGE_LIST_COMPLETED, packageList));
					}
					catch (Exception e)
					{
						error = "Error parsing the package list";
						Debug.LogError(e.Message);
					}
				}
				else
				{
					error = task.Error;
				}

				Dispatcher.Dispatch(
					new Action(ActionTypes.REQUEST_PACKAGE_LIST_FAILED, error));
			});
		}

		public static void InstallPackage(AvailablePackage package, string version)
		{
			// TODO: verify this version exists
			// TODO: verify not already installed

			var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + package.Name;
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;
			var args = new InstallPackageArgs(package, version);

			var taskChain = new TaskChain();

			taskChain.Add(() => { Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_STARTED, args); });
			taskChain.Add(new AddSubmoduleTask(package.GitUrl, relativeInstallDirectory));
			taskChain.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
			taskChain.Add(() => { Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, args); });
			taskChain.Execute();

			// TODO: handle errors
		}

		public static void InstallCustomPackage(string name, string url, string version = null)
		{
			// TODO: verify this version exists
			// TODO: verify not already installed

			var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + name;
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + name;
			var args = new InstallPackageArgs(name, url, version);

			Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_STARTED, args);

			var taskChain = new TaskChain();

			taskChain.Add(new AddSubmoduleTask(url, relativeInstallDirectory));

			if (!string.IsNullOrEmpty(version))
			{
				taskChain.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
			}

			taskChain.Execute(() => { Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, args); });

			// TODO: handle errors
		}

		public static void RemovePackage(AvailablePackage package)
		{
			var installDirectory = Settings.RelativePackagesDirectoryPath + package.Name;

			Dispatcher.Dispatch(ActionTypes.REMOVE_PACKAGE_STARTED, package);

			var task = new RemoveSubmoduleTask(installDirectory);
			task.Execute(() =>
			{
				Dispatcher.Dispatch(ActionTypes.REMOVE_PACKAGE_COMPLETE, package);
			});
		}

		public static void ReadPackagesJson()
		{
			InstalledPackageList installedPackageList = null;

			if (File.Exists(Settings.AbsolutePackagesJsonFilePath))
			{
				var jsonText = File.ReadAllText(Settings.AbsolutePackagesJsonFilePath);
				installedPackageList = JsonUtility.FromJson<InstalledPackageList>(jsonText);
			}

			Dispatcher.Dispatch(ActionTypes.READ_PACKAGES_JSON, installedPackageList ?? new InstalledPackageList());
		}

		public static void SwitchVersion(AvailablePackage package, string version)
		{
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;

			Dispatcher.Dispatch(ActionTypes.SWITCH_PACKAGE_VERSION_STARTED);

			var task = new CheckoutSubmoduleTask(absoluteInstallDirectory, version);
			task.Execute(() =>
			{
				Dispatcher.Dispatch(ActionTypes.SWITCH_PACKAGE_VERSION_COMPLETE);
			});
		}

		public static void SyncProjectEpic()
		{
			CompilePackageListStatus((packageListStatus) =>
			{
				if (!packageListStatus.IsProjectUpToDate)
				{
					var taskChain = new TaskChain();

					foreach (var packageStatus in packageListStatus.Packages)
					{
						var version = packageStatus.RequiredVersion;
						var packageName = packageStatus.PackageName;
						var gitUrl = packageStatus.GitUrl;
						var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + packageName;
						var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + packageName;

						if (packageStatus.IsMissing)
						{
							taskChain.Add(
								new AddSubmoduleTask(gitUrl, relativeInstallDirectory));
							taskChain.Add(
								new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
						}
						else if (packageStatus.IsOnWrongVersion)
						{
							taskChain.Add(
								new CheckoutSubmoduleTask(
									absoluteInstallDirectory,
									version));
						}
					}

					taskChain.Execute(() =>
					{
						Debug.Log("Synced Project.");
					});
				}
				else
				{
					Debug.Log("Project is already up to date.");
				}
			});
		}

		private static void CompilePackageListStatus(Action<PackageListStatus> onComplete)
		{
			Dispatcher.Dispatch(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_STARTED);

			var tasks = new TaskChain();
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

				var task = new GetGitBranchOrTagTask(package);
				var packageVersion = package.Version;
				tasks.Add(task);
				tasks.Add(() =>
				{
					packageStatus.IsOnWrongVersion = task.Result != packageVersion;
				});
			}

			tasks.Execute(() =>
			{
				Debug.Log("Project package status: ");

				foreach (var package in packageListStatus.Packages)
				{
					Debug.Log(package);
				}

				Debug.Log("The project is " +
					(packageListStatus.IsProjectUpToDate ? "" : "not ")+ "up to date");

				Dispatcher.Dispatch(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_COMPLETE,
					packageListStatus);

				onComplete(packageListStatus);
			});
		}
	}

	internal class InstallPackageArgs
	{
		public string PackageName { get; set; }
		public string GitUrl { get; set; }
		public string Version { get; set; }

		public InstallPackageArgs(AvailablePackage package, string version)
		{
			PackageName = package.Name;
			GitUrl = package.GitUrl;
			Version = version;
		}

		public InstallPackageArgs(string packageName, string gitUrl, string version)
		{
			PackageName = packageName;
			GitUrl = gitUrl;
			Version = version;
		}
	}
}