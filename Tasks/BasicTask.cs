using System;

namespace DUCK.Tasks
{
	public class BasicTask<TResult> : ITask<TResult> where TResult : OperationResult
	{
		private readonly Action<Action<TResult>> taskFunction;

		public BasicTask(Action<Action<TResult>> taskFunction)
		{
			if (taskFunction == null) throw new ArgumentNullException("taskFunction");
			this.taskFunction = taskFunction;
		}

		public void Execute(Action<TResult> onComplete)
		{
			taskFunction(onComplete);
		}
	}
}