using System;
using System.IO;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.Git;
using DUCK.PackageManager.Editor.Tasks.Web;
using DUCK.PackageManager.Editor.UI.Flux;
using DUCK.Tasks;
using DUCK.Tasks.Epics;
using DUCK.Tasks.Extensions;
using UnityEngine;
using Action = DUCK.PackageManager.Editor.UI.Flux.Action;
using AsyncOperation = DUCK.Tasks.AsyncOperation;

namespace DUCK.PackageManager.Editor.UI
{
	internal static class Actions
	{
		public static void FetchPackages()
		{
			Dispatcher.Dispatch(
				new Action(ActionTypes.REQUEST_PACKAGE_LIST_STARTED));

			var task = new HttpGetRequestTask(Settings.DuckPackagesUrl);
			task.Execute(result =>
			{
				string error = null;
				if (!result.IsError)
				{
					try
					{
						var packageList = JsonUtility.FromJson<AvailablePackageList>(result.Text);

						Dispatcher.Dispatch(
							new Action(ActionTypes.REQUEST_PACKAGE_LIST_COMPLETED, packageList));

						return;
					}
					catch (Exception e)
					{
						error = "Error parsing the package list";
						Debug.LogError(e.Message);
					}
				}
				else
				{
					error = result.Error.Message;
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

			var operation = new AsyncOperation();

			operation.AddSync(() => { Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_STARTED, args); });

			operation.Add(new AddSubmoduleTask(package.GitUrl, relativeInstallDirectory));

			operation.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));

			operation.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Error installing package");
					Debug.LogError(result.Error.Message);

					Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_FAILED);
				}
				else
				{
					Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, args);
				}
			});
		}

		public static void InstallCustomPackage(string name, string url, string version = null)
		{
			// TODO: verify this version exists
			// TODO: verify not already installed

			var relativeInstallDirectory = Settings.RelativePackagesDirectoryPath + name;
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + name;
			var args = new InstallPackageArgs(name, url, version);

			Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_STARTED, args);

			var operation = new AsyncOperation();

			operation.Add(new AddSubmoduleTask(url, relativeInstallDirectory));

			if (!string.IsNullOrEmpty(version))
			{
				operation.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
			}

			operation.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Error installing package");
					Debug.LogError(result.Error.Message);

					Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_FAILED);
				}
				else
				{
					Dispatcher.Dispatch(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, args);
				}
			});
		}

		public static void RemovePackage(AvailablePackage package)
		{
			var installDirectory = Settings.RelativePackagesDirectoryPath + package.Name;

			Dispatcher.Dispatch(ActionTypes.REMOVE_PACKAGE_STARTED, package);

			var task = ActionEpics.RemoveSubmodule(installDirectory);
			task.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Failed to removed submodule: " + installDirectory);
					Debug.LogError(result.Error.Message);
					Dispatcher.Dispatch(ActionTypes.REMOVE_PACKAGE_FAILED);
				}
				else
				{
					Debug.Log("Successfully removed submodule: " + installDirectory);
					Dispatcher.Dispatch(ActionTypes.REMOVE_PACKAGE_COMPLETE, package);
				}
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
			// TODO: Verify that this package is actually installed
			
			var absoluteInstallDirectory = Settings.AbsolutePackagesDirectoryPath + package.Name;

			Dispatcher.Dispatch(ActionTypes.SWITCH_PACKAGE_VERSION_STARTED);

			var operation = new AsyncOperation();
			
			operation.Add(new GitTask("fetch"){ WorkingDirectory = absoluteInstallDirectory });
			
			operation.Add(new CheckoutSubmoduleTask(absoluteInstallDirectory, version));
			
			operation.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Failed to switch package version: " + package.Name + "@" + version);
					Debug.LogError(result.Error.Message);
					Dispatcher.Dispatch(ActionTypes.SWITCH_PACKAGE_VERSION_FAILED);
				}
				else
				{
					Debug.Log("Switched package: " + package.Name + " to version: " + version);
					Dispatcher.Dispatch(ActionTypes.SWITCH_PACKAGE_VERSION_COMPLETE, 
						new SwitchPackageArgs(package, version));
				}
			});
		}

		public static void SyncProject()
		{
			var tasks = ActionEpics.SyncProject();
			tasks.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Failed to sync project");
					Debug.LogError(result.Error.Message);
				}
				else
				{
					Debug.Log("Project sync completed, the project is now up to date.");
				}
			});
		}

		private static void CompilePackageListStatus()
		{
			Dispatcher.Dispatch(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_STARTED);

			var tasks = ActionEpics.CompilePackageListStatus();
			tasks.Execute(result =>
			{
				if (result.IsError)
				{
					Debug.LogError("Failed to compile package status");
					Debug.LogError(result.Error.Message);
				}
				else
				{
					var packageListStatus = ((Result<PackageListStatus>) result).Data;
					Dispatcher.Dispatch(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_COMPLETE,
						packageListStatus);
				}
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

	internal class SwitchPackageArgs
	{
		public AvailablePackage Package { get; set; }
		public string Version { get; set; }

		public SwitchPackageArgs(AvailablePackage package, string version)
		{
			Package = package;
			Version = version;
		}
	}
}