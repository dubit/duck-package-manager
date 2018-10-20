using System.IO;
using System.Linq;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.UI.Flux;
using UnityEngine;

namespace DUCK.PackageManager.Editor.UI.Stores
{
	internal class PackageStore
	{
		public IState<bool> IsFetchingPackages { get; private set; }
		public IState<AvailablePackageList> AvailablePackages { get; set; }
		public IState<InstalledPackageList> InstalledPackages { get; set; }
		public IState<PackageListStatus> PackageListStatus { get; set; }
		public IState<string> Error { get; set; }
		public IState<string> SearchQuery { get; set; }

		public IState<bool> IsWorking { get; set; }
		public IState<string> Operation { get; set; }

		public PackageStore(Store store)
		{
			IsFetchingPackages = store.CreateState(false);
			AvailablePackages = store.CreateState<AvailablePackageList>(null);
			InstalledPackages = store.CreateState(new InstalledPackageList());
			PackageListStatus = store.CreateState<PackageListStatus>(null);
			Error = store.CreateState<string>(null);
			SearchQuery = store.CreateState<string>(null);
			IsWorking = store.CreateState(false);
			Operation = store.CreateState<string>(null);

			store.Subscribe(ActionTypes.REQUEST_PACKAGE_LIST_STARTED, HandleRequestPackageListStarted);
			store.Subscribe(ActionTypes.REQUEST_PACKAGE_LIST_COMPLETED, HandleRequestPackageListCompleted);
			store.Subscribe(ActionTypes.REQUEST_PACKAGE_LIST_FAILED, HandleRequestPackageListFailed);
			store.Subscribe(ActionTypes.PACKAGE_LIST_SEARCH_CHANGED, HandlePackageListSearchChanged);
			store.Subscribe(ActionTypes.PACKAGE_INSTALLATION_STARTED, HandlePackageInstallationStarted);
			store.Subscribe(ActionTypes.PACKAGE_INSTALLATION_COMPLETE, HandlePackageInstallationComplete);
			store.Subscribe(ActionTypes.REMOVE_PACKAGE_STARTED, HandleRemovePackageStarted);
			store.Subscribe(ActionTypes.REMOVE_PACKAGE_COMPLETE, HandleRemovePackageComplete);
			store.Subscribe(ActionTypes.READ_PACKAGES_JSON, HandleReadPackagesJson);
			store.Subscribe(ActionTypes.SWITCH_PACKAGE_VERSION_STARTED, HandleSwitchPackageVersionStarted);
			store.Subscribe(ActionTypes.SWITCH_PACKAGE_VERSION_COMPLETE, HandleSwitchPackageVersionComplete);
			store.Subscribe(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_STARTED, HandleCompilePackageListStatusStarted);
			store.Subscribe(ActionTypes.COMPILE_PACKAGE_LIST_STATUS_COMPLETE, HandleCompilePackageListStatusComplete);
		}

		private void HandleRequestPackageListStarted(Action action)
		{
			IsFetchingPackages.SetValue(true);
		}

		private void HandleRequestPackageListCompleted(Action action)
		{
			IsFetchingPackages.SetValue(false);
			AvailablePackages.SetValue((AvailablePackageList) action.Payload);
		}

		private void HandleRequestPackageListFailed(Action action)
		{
			IsFetchingPackages.SetValue(false);
			Error.SetValue((string) action.Payload);
		}

		private void HandlePackageListSearchChanged(Action action)
		{
			SearchQuery.SetValue((string) action.Payload);
		}

		private void HandlePackageInstallationStarted(Action action)
		{
			var args = (InstallPackageArgs) action.Payload;
			IsWorking.SetValue(true);
			Operation.SetValue("Installing " + args.PackageName + "...");
		}

		private void HandlePackageInstallationComplete(Action action)
		{
			var args = (InstallPackageArgs) action.Payload;

			var installedPackages = InstalledPackages.Value;
			installedPackages.Packages.Add(new InstalledPackage(args.PackageName, args.Version, args.GitUrl));

			InstalledPackages.SetValue(installedPackages);
			IsWorking.SetValue(false);
			Operation.SetValue(null);

			WritePackagesJson();
		}

		private void HandleReadPackagesJson(Action action)
		{
			InstalledPackages.SetValue((InstalledPackageList) action.Payload);
		}

		private void HandleRemovePackageStarted(Action action)
		{
			IsWorking.SetValue(true);
			Operation.SetValue("Removing " + ((AvailablePackage) action.Payload).Name + "...");
		}

		private void HandleRemovePackageComplete(Action action)
		{
			var package = (AvailablePackage) action.Payload;

			IsWorking.SetValue(false);
			Operation.SetValue(null);

			var installedPackages = InstalledPackages.Value;
			installedPackages.Packages.RemoveAll(p => p.Name == package.Name);
			InstalledPackages.SetValue(installedPackages);

			WritePackagesJson();
		}

		private void HandleSwitchPackageVersionStarted(Action action)
		{
			IsWorking.SetValue(true);
			Operation.SetValue("Switching package version");
		}

		private void HandleSwitchPackageVersionComplete(Action action)
		{
			var args = (SwitchPackageArgs) action.Payload;

			var installedPackages = InstalledPackages.Value;
			var package = installedPackages.Packages.FirstOrDefault(p => p.Name == args.Package.Name);
			package.Version = args.Version;

			InstalledPackages.SetValue(installedPackages);

			WritePackagesJson();

			IsWorking.SetValue(false);
			Operation.SetValue(null);
		}

		private void WritePackagesJson()
		{
			var jsonText = JsonUtility.ToJson(InstalledPackages.Value, true);
			File.WriteAllText(Settings.AbsolutePackagesJsonFilePath, jsonText);
		}

		private void HandleCompilePackageListStatusStarted(Action action)
		{
			PackageListStatus.SetValue(null);
			IsWorking.SetValue(true);
			Operation.SetValue("Compiling status of each package");
		}

		private void HandleCompilePackageListStatusComplete(Action action)
		{
			IsWorking.SetValue(false);
			Operation.SetValue(null);

			PackageListStatus.SetValue((PackageListStatus) action.Payload);
		}
	}
}