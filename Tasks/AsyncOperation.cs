using System;
using System.Collections.Generic;

namespace DUCK.Tasks
{
	public interface IOperationControls
	{
		void Fail(Error err);
		void Complete(OperationResult result);
	}

	public class AsyncOperation : IOperationControls
	{
		private readonly List<ITaskRunner> tasks = new List<ITaskRunner>();
		private Action onCompleteCallback;
		private Action<OperationResult> onCompleteCallbackWithData;
		private bool isRunning;

		public void Add<TResult>(ITask<TResult> task, Action<TResult, IOperationControls> processResult) where TResult : OperationResult
		{
			if (isRunning) throw new InvalidOperationException("Cannot call Add during execution");
			if (task == null) throw new ArgumentNullException("task");
			if (processResult == null) throw new ArgumentNullException("processResult");
			tasks.Add(new TaskRunner<TResult>(task, processResult));
		}

		public void Execute(Action onComplete = null)
		{
			isRunning = true;
			onCompleteCallback = onComplete;
			ExecuteNextTask();
		}

		public void Execute(Action<OperationResult> onComplete)
		{
			isRunning = true;
			onCompleteCallbackWithData = onComplete;
			ExecuteNextTask();
		}

		private void NotifyComplete(OperationResult result)
		{
			if (onCompleteCallback != null)
			{
				onCompleteCallback();
			}
			if (onCompleteCallbackWithData != null)
			{
				onCompleteCallbackWithData(result);
			}
		}

		private void ExecuteNextTask()
		{
			if (tasks.Count == 0)
			{
				isRunning = false;
				NotifyComplete(new OperationResult());
				return;
			}

			tasks[0].Run(this, () =>
			{
				if (isRunning)
				{
					tasks.RemoveAt(0);
					ExecuteNextTask();
				}
			});
		}

		void IOperationControls.Fail(Error err)
		{
			isRunning = false;
			var result = new OperationResult();
			result.Err(err);
			NotifyComplete(result);
		}

		void IOperationControls.Complete(OperationResult result)
		{
			isRunning = false;
			NotifyComplete(result);
		}
	}

	interface ITaskRunner
	{
		void Run(IOperationControls op, Action onComplete);
	}

	/// <summary>
	/// Faciliates running a single task
	/// </summary>
	/// <typeparam name="TResult">The result type of the task</typeparam>
	class TaskRunner<TResult> : ITaskRunner where TResult : OperationResult
	{
		private ITask<TResult> task;
		private Action<TResult, IOperationControls> processResult;

		public TaskRunner(ITask<TResult> task, Action<TResult, IOperationControls> processResult)
		{
			this.task = task;
			this.processResult = processResult;
		}

		public void Run(IOperationControls op, Action onComplete)
		{
			task.Execute(result =>
			{
				if (result.IsError)
				{
					op.Fail(result.Error);
				}

				processResult(result, op);

				if (!result.IsError)
				{
					onComplete();
				}
			});
		}
	}
}