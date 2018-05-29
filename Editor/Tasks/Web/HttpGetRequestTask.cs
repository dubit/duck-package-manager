using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.PackageManager.Editor.Tasks.Web
{
	public class HttpGetRequestTask : ITask
	{
		public bool IsError { get { return www.isHttpError || www.isNetworkError; } }
		public string Text { get { return www.downloadHandler.text; } }
		public string Error { get { return www.error; } }

		private UnityWebRequest www;
		private Action onCompleteCallback;

		public HttpGetRequestTask(string url)
		{
			www = UnityWebRequest.Get(url);
		}

		public void Execute(Action onComplete = null)
		{
			onCompleteCallback = onComplete;
			www.Send();
			EditorApplication.update += EditorUpdate;
		}

		private void EditorUpdate()
		{
			while (!www.isDone)
			{
				return;
			}

			if (onCompleteCallback != null)
			{
				onCompleteCallback();
			}

			EditorApplication.update -= EditorUpdate;
		}
	}
}