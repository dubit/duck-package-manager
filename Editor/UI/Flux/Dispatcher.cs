namespace DUCK.PackageManager.Editor.UI.Flux
{
	public static class Dispatcher
	{
		public static event System.Action<Action> OnActionDispatched;

		public static void Dispatch(Action action)
		{
			if (OnActionDispatched != null)
			{
				OnActionDispatched.Invoke(action);
			}
		}

		public static void Dispatch(string actionType, object payload = null)
		{
			if (OnActionDispatched != null)
			{
				OnActionDispatched.Invoke(new Action(actionType, payload));
			}
		}
	}
}