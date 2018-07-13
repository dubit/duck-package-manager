using System;

namespace DUCK.PackageManager.Tasks
{
	public interface ITask<TResult> where TResult : OperationResult
	{
		void Execute(Action<TResult> onComplete);
	}
}
