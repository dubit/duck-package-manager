using System.IO;
using DUCK.PackageManager.Editor.Data;
using DUCK.PackageManager.Editor.UI.Flux;
using UnityEngine;

namespace DUCK.PackageManager.Editor.UI.Stores
{
	public class PackageStore
	{
		public IState<bool> IsFetchingPackages { get; private set; }
		public IState<AvailablePackageList> AvailablePackages { get; set; }
		public IState<InstalledPackageList> InstalledPackages { get; set; }
		public IState<string> Error { get; set; }
		public IState<string> SearchQuery { get; set; }

		public IState<bool> IsWorking { get; set; }
		public IState<string> Operation { get; set; }

		public PackageStore(Store store)
		{
			IsFetchingPackages = store.CreateState(false);
			AvailablePackages = store.CreateState<AvailablePackageList>(null);
			InstalledPackages = store.CreateState(new InstalledPackageList());
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
		}

		private void HandleRequestPackageListStarted(Action obj)
		{
			IsFetchingPackages.SetValue(true);
		}

		private void HandleRequestPackageListCompleted(Action obj)
		{
			IsFetchingPackages.SetValue(false);
			AvailablePackages.SetValue((AvailablePackageList)obj.Payload);
		}

		private void HandleRequestPackageListFailed(Action obj)
		{
			IsFetchingPackages.SetValue(false);
			Error.SetValue((string)obj.Payload);
		}

		private void HandlePackageListSearchChanged(Action obj)
		{
			SearchQuery.SetValue((string)obj.Payload);
		}

		private void HandlePackageInstallationStarted(Action obj)
		{
			var args = (InstallPackageArgs) obj.Payload;
			IsWorking.SetValue(true);
			Operation.SetValue("Installing " + args.PackageName + "...");
		}

		private void HandlePackageInstallationComplete(Action obj)
		{
			var args = (InstallPackageArgs) obj.Payload;

			var installedPackages = InstalledPackages.Value;
			installedPackages.Packages.Add(new InstalledPackage(args.PackageName, args.Version, args.GitUrl));

			InstalledPackages.SetValue(installedPackages);
			IsWorking.SetValue(false);
			Operation.SetValue(null);

			WritePackagesJson();
		}

		private void HandleReadPackagesJson(Action obj)
		{
			InstalledPackages.SetValue((InstalledPackageList)obj.Payload);
		}

		private void HandleRemovePackageStarted(Action obj)
		{
			IsWorking.SetValue(true);
			Operation.SetValue("Removing " + ((AvailablePackage)obj.Payload).Name + "...");
		}

		private void HandleRemovePackageComplete(Action obj)
		{
			var package = (AvailablePackage) obj.Payload;

			IsWorking.SetValue(false);
			Operation.SetValue(null);

			var installedPackages = InstalledPackages.Value;
			installedPackages.Packages.RemoveAll(p => p.Name == package.Name);
			InstalledPackages.SetValue(installedPackages);

			WritePackagesJson();
		}

		private void HandleSwitchPackageVersionStarted(Action obj)
		{
			IsWorking.SetValue(true);
			Operation.SetValue("Switching package version");
		}

		private void HandleSwitchPackageVersionComplete(Action obj)
		{
			IsWorking.SetValue(false);
			Operation.SetValue(null);
		}

		private void WritePackagesJson()
		{
			var jsonText = JsonUtility.ToJson(InstalledPackages.Value, true);
			File.WriteAllText(Settings.AbsolutePackagesJsonFilePath, jsonText);
		}
	}
}