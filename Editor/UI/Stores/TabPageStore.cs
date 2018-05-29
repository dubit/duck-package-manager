using DUCK.PackageManager.Editor.UI.Flux;

namespace DUCK.PackageManager.Editor.UI.Stores
{
	public class TabPageStore
	{
		public IState<string> CurrentPage { get; private set; }

		public TabPageStore(Store store)
		{
			CurrentPage = store.CreateState(Tabs.AVAILABLE_PACKAGES);

			store.Subscribe(ActionTypes.CHANGE_TAB_PAGE, HandleTabPageChanged);
		}

		private void HandleTabPageChanged(Action obj)
		{
			CurrentPage.SetValue((string)obj.Payload);
		}
	}
}