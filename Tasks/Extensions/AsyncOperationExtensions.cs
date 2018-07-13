using System;

namespace DUCK.Tasks.Extensions
{
	public static class AsyncOperationExtensions
	{
		public static void Add<TResult>(this AsyncOperation operation, Action<Action<TResult>> task,
			Action<TResult, IOperationControls> processResult) where TResult : OperationResult
		{
			operation.Add(new BasicTask<TResult>(task), processResult);
		}
	}
}