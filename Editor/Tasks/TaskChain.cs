using System;
using System.Collections.Generic;
using UnityEditor;

namespace DUCK.PackageManager.Editor.Tasks
{
	public class TaskChain : ITask
	{
		private List<ITask> tasks = new List<ITask>();
		private Action onCompleteCallback;

		public TaskChain Add(ITask task)
		{
			tasks.Add(task);
			return this;
		}

		public TaskChain Add(Action action)
		{
			tasks.Add(new FunctionCallTask(action));
			return this;
		}

		public void Execute(Action onComplete = null)
		{
			onCompleteCallback = onComplete;
			ExecuteNextTask();
		}

		private void NotifyComplete()
		{
			if (onCompleteCallback != null)
			{
				onCompleteCallback();
			}
		}

		private void ExecuteNextTask()
		{
			if (tasks.Count == 0)
			{
				NotifyComplete();
				return;
			}

			tasks[0].Execute(() =>
			{
				tasks.RemoveAt(0);
				ExecuteNextTask();
			});
		}

		private class FunctionCallTask : ITask
		{
			private Action onCompleteCallback;
			private readonly Action function;

			public FunctionCallTask(Action func)
			{
				if (func == null) throw new ArgumentNullException("func");
				function = func;
			}

			public void Execute(Action onComplete = null)
			{
				onCompleteCallback = onComplete;
				EditorApplication.update += NextFrame;
			}

			private void NextFrame()
			{
				EditorApplication.update -= NextFrame;
				function();
				onCompleteCallback();
			}
		}
	}
}