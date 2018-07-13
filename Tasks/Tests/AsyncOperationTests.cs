using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DUCK.PackageManager.Tasks.Tests
{
	public class AsyncOperationTests
	{
		class TestTask<T> : ITask<Result<T>>
		{
			public bool Fails { get; set; }

			public T Data { get; private set; }
			public bool IsExecuted { get; private set; }

			public TestTask(T data = default(T))
			{
				Data = data;
			}

			public void Execute(Action<Result<T>> onComplete)
			{
				var result = new Result<T>(Data);
				if (Fails)
				{
					result.Err("test-error", "There was an unexpected Error");
				}

				IsExecuted = true;
				onComplete(result);
			}
		}

		[Test]
		public void ExpectConstructorNotToThrow()
		{
			Assert.DoesNotThrow(() => { new AsyncOperation(); });
		}

		[Test]
		public void ExpectAddToThrowWithNullParameters()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var operation = new AsyncOperation();
				operation.Add<OperationResult>(null, (result, op) => {});
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				var operation = new AsyncOperation();
				operation.Add(new TestTask<int>(), null);
			});
		}

		[Test]
		public void ExpectAddedTaskToBeExecutedAndDataToBePassedToProcessResultCallback()
		{
			var isProcessResultExecuted = false;
			var resultValue = 0;

			var task = new TestTask<int>(42);

			var operation = new AsyncOperation();
			operation.Add(task, (r, op) =>
			{
				resultValue = r.Data;
				isProcessResultExecuted = true;
			});

			operation.Execute();

			Assert.IsTrue(task.IsExecuted);
			Assert.IsTrue(isProcessResultExecuted);
			Assert.AreEqual(42, resultValue);
		}

		[Test]
		public void ExpectAddedTasksToBeExecutedInOrder()
		{
			var results = new List<int>();

			var task1 = new TestTask<int>(1);
			var task2 = new TestTask<int>(2);
			var task3 = new TestTask<int>(3);

			Action<Result<int>, IOperationControls> processResult = (result, op) =>
			{
				results.Add(result.Data);
			};

			var operation = new AsyncOperation();
			operation.Add(task1, processResult);
			operation.Add(task2, processResult);
			operation.Add(task3, processResult);

			operation.Execute();

			Assert.AreEqual(1, results[0]);
			Assert.AreEqual(2, results[1]);
			Assert.AreEqual(3, results[2]);
		}

		[Test]
		public void ExpectFailingTaskToFailTheOperation()
		{
			var task1 = new TestTask<int>(1);
			var task2 = new TestTask<int>(2){ Fails = true };
			var task3 = new TestTask<int>(3);

			Action<Result<int>, IOperationControls> processResult = (result, op) =>
			{
			};

			var operation = new AsyncOperation();
			operation.Add(task1, processResult);
			operation.Add(task2, processResult);
			operation.Add(task3, processResult);

			OperationResult overallResult = null;

			operation.Execute(result =>
			{
				overallResult = result;
			});

			Assert.IsTrue(task1.IsExecuted);
			Assert.IsTrue(task2.IsExecuted);
			Assert.IsFalse(task3.IsExecuted);
			Assert.IsTrue(overallResult.IsError);
		}

		[Test]
		public void ExpectFailingOpInProcessResultToFailTheOperation()
		{
			var task1 = new TestTask<int>(1);
			var task2 = new TestTask<int>(2);
			var task3 = new TestTask<int>(3);

			Action<Result<int>, IOperationControls> processResult = (result, op) =>
			{
			};

			var operation = new AsyncOperation();
			operation.Add(task1, processResult);
			operation.Add(task2, (result, op) => { op.Fail(new Error("test-error", "Error"));});
			operation.Add(task3, processResult);

			OperationResult overallResult = null;

			operation.Execute(result =>
			{
				overallResult = result;
			});

			Assert.IsTrue(task1.IsExecuted);
			Assert.IsTrue(task2.IsExecuted);
			Assert.IsFalse(task3.IsExecuted);
			Assert.IsTrue(overallResult.IsError);
		}

		[Test]
		public void ExpectCompletingTheOpInProcessResultToSkipRemainingTasks()
		{
			var task1 = new TestTask<int>(1);
			var task2 = new TestTask<int>(2);
			var task3 = new TestTask<int>(3);

			Action<Result<int>, IOperationControls> processResult = (result, op) =>
			{
			};

			var operation = new AsyncOperation();
			operation.Add(task1, processResult);
			operation.Add(task2, (result, op) =>
			{
				var finalResult = new Result<string>("done");
				op.Complete(finalResult);
			});
			operation.Add(task3, processResult);

			Result<string> overallResult = null;

			operation.Execute(result =>
			{
				overallResult = (Result<string>)result;
			});

			Assert.IsTrue(task1.IsExecuted);
			Assert.IsTrue(task2.IsExecuted);
			Assert.IsFalse(task3.IsExecuted);
			Assert.IsFalse(overallResult.IsError);
			Assert.AreEqual("done", overallResult.Data);
		}

		[Test]
		public void ExpectAddToThrowIfTheOperationIsRunning()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var operation = new AsyncOperation();
				operation.Add(new TestTask<int>(1), (result, op) =>
				{
					operation.Add(new TestTask<int>(2), (result1, op1) => {} );
				});
				operation.Execute();
			});
		}
	}
}