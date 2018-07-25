using System;
using System.Text;

namespace DUCK.Tasks
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

		public void Err(Exception exception)
		{
			IsError = true;
			Error = new Error(exception);
		}

		public void Err(string id, string message, object data = null)
		{
			IsError = true;
			Error = new Error(id, message, data);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append(GetType().Name);
			builder.Append(" | ");
			builder.Append(IsError ? "Error: " : "Success");
			if (IsError)
			{
				builder.Append(Error.Message);
				builder.AppendLine();
				builder.Append(Error.Data);
			}

			return builder.ToString();
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

		public Error(Exception exception)
		{
			ID = "exception-occurred";
			Message = exception.Message;
			Data = exception;
		}

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