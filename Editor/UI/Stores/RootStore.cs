namespace DUCK.PackageManager.Editor.UI.Stores
{
	public class RootStore : Store
	{
		private static RootStore instance;
		public static RootStore Instance { get { return instance ?? (instance = new RootStore()); }}

		public PackageStore Packages { get; private set; }
		public TabPageStore TabPages { get; private set; }

		private RootStore()
		{
			Packages = new PackageStore(this);
			TabPages = new TabPageStore(this);
		}
	}
}