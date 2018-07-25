using System;
using DUCK.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace DUCK.PackageManager.Editor.Tasks.Web
{
	internal class HttpGetRequestTask : ITask<HttpGetRequestResult>
	{

		private UnityWebRequest www;
		private Action<HttpGetRequestResult> onCompleteCallback;

		public HttpGetRequestTask(string url)
		{
			www = UnityWebRequest.Get(url);
		}

		public void Execute(Action<HttpGetRequestResult> onComplete)
		{
			onCompleteCallback = onComplete;
			www.SendWebRequest();
			EditorApplication.update += EditorUpdate;
		}

		private void EditorUpdate()
		{
			while (!www.isDone)
			{
				return;
			}

			onCompleteCallback(new HttpGetRequestResult(www));

			EditorApplication.update -= EditorUpdate;
		}
	}

	internal class HttpGetRequestResult : OperationResult
	{
		public string Text { get { return www.downloadHandler.text; } }

		private readonly UnityWebRequest www;

		public HttpGetRequestResult(UnityWebRequest www)
		{
			this.www = www;

			IsError = www.isHttpError || www.isNetworkError;
			if (IsError)
			{
				Error = new Error(www.responseCode.ToString(), www.error);
			}
		}
	}
}