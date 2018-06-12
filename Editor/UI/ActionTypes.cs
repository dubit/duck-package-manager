namespace DUCK.PackageManager.Editor.UI
{
	internal static class ActionTypes
	{
		public const string READ_PACKAGES_JSON = "read-packages-json";
		public const string REQUEST_PACKAGE_LIST_STARTED = "request-package-list-start";
		public const string REQUEST_PACKAGE_LIST_COMPLETED = "request-package-list-completed";
		public const string REQUEST_PACKAGE_LIST_FAILED = "request-package-list-failed";

		public const string PACKAGE_LIST_SEARCH_CHANGED = "package-list-search-changed";

		public const string PACKAGE_INSTALLATION_STARTED = "package-installation-started";
		public const string PACKAGE_INSTALLATION_COMPLETE = "package-installation-complete";
		public const string PACKAGE_INSTALLATION_FAILED = "package-installation-failed";

		public const string REMOVE_PACKAGE_STARTED = "remove-package-started";
		public const string REMOVE_PACKAGE_COMPLETE = "remove-package-complete";
		public const string REMOVE_PACKAGE_FAILED = "remove-package-failed";

		public const string SWITCH_PACKAGE_VERSION_STARTED = "switch-package-version-started";
		public const string SWITCH_PACKAGE_VERSION_COMPLETE = "switch-package-version-complete";
		public const string SWITCH_PACKAGE_VERSION_FAILED = "switch-package-version-failed";

		public const string CHANGE_TAB_PAGE = "change-tab-page";
	}
}