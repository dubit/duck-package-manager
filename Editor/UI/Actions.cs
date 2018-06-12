using System;
using System.IO;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.Tasks;
using DUCK.PackageManager.Editor.Tasks.Web;
using DUCK.PackageManager.Editor.UI.Flux;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using Action = DUCK.PackageManager.Editor.UI.Flux.Action;

namespace DUCK.PackageManager.Editor.UI
{
	public static class Actions
	{
		public static void FetchPackages()
		{
			Dispatcher.Dispatch(
				new Action(ActionTypes.REQUEST_PACKAGE_LIST_STARTED));

			var task = new HttpGetRequestTask("https://raw.githubusercontent.com/dubit/duck-packages/master/packages.json");
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
			// verify this version exists
			// verify not already installed

			var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + package.Name;
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;
			var args = new InstallPackageArgs(package, version);

			Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_STARTED, args);

			var taskChain = new TaskChain();

			taskChain.Add(new AddSubmoduleTask(package.GitUrl, relativeInstallDirectory));

			taskChain.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));

			taskChain.Execute(() =>
			{
				Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, args);
			});

			// TODO: handle errors
		}

		public static void InstallCustomPackage(string name, string url, string version = null)
		{
			// verify this version exists
			// verify not already installed

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
	}

	public class InstallPackageArgs
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