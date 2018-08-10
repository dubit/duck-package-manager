using System;

namespace DUCK.Tasks.Extensions
{
	public static class AsyncOperationExtensions
	{
		public static void AddSync<TResult>(this AsyncOperation operation, Func<TResult> task, Action<TResult, IOperationControls> processResult = null)
			where TResult : OperationResult
		{
			operation.Add(new BasicTask<TResult>(onComplete =>
			{
				onComplete(task());
			}), processResult);
		}

		public static void AddSync(this AsyncOperation operation, Action task)
		{
			operation.Add(new BasicTask<OperationResult>(onComplete =>
			{
				var result = new OperationResult();

				try
				{
					task();
				}
				catch (Exception e)
				{
					result.Err(new Error(e));
				}

				onComplete(result);
			}));
		}
	}
}