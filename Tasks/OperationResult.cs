using System;

namespace DUCK.PackageManager.Tasks
{
	public class OperationResult
	{
		public bool IsError { get; set; }
		public Error Error { get; set; }

		public void Err(Error error)
		{
			IsError = true;
			Error = error;
		}

		public void Err(string id, string message, object data = null)
		{
			IsError = true;
			Error = new Error(id, message, data);
		}
	}

	public class Result<T> : OperationResult
	{
		public T Data { get; private set; }

		public Result(T data)
		{
			Data = data;
		}
	}

	public class Error
	{
		public string ID { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }

		public Error(string id, string message, object data = null)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (message == null) throw new ArgumentNullException("message");

			ID = id;
			Message = message;
			Data = data;
		}
	}
}