using System;

namespace DUCK.Tasks
{
	public interface ITask<TResult>
	{
		void Execute(Action<TResult> onComplete);
	}
}
