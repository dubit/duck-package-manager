namespace DUCK.PackageManager.Editor.UI.Stores
{
	internal interface IState
	{
		object GetValue();
	}

	internal interface IState<T> : IState
	{
		T Value { get; }
		void SetValue(T value);
	}

	internal partial class Store
	{
		private interface IStoreState : IState
		{
			bool IsDirty { get; }
			void Clean();
		}

		private class State<T> : IState<T>, IStoreState
		{
			public bool IsDirty { get; protected set; }
			public T Value { get; private set; }

			public State(T value)
			{
				Value = value;
			}

			public void SetValue(T value)
			{
				if (Equals(value, Value)) return;
				IsDirty = true;
				Value = value;
			}

			public void Clean()
			{
				IsDirty = true;
			}

			object IState.GetValue()
			{
				return Value;
			}
		}
	}
}