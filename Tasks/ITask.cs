using System;

namespace DUCK.Tasks
{
	public interface ITask<TResult> where TResult : OperationResult
	{
		void Execute(Action<TResult> onComplete);
	}
}
