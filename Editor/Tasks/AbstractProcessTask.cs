using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DUCK.PackageManager.Editor.Tasks
{
	internal abstract class AbstractProcessTask : ITask
	{
		private static readonly string projectDirectory;

		public bool ReceivedError
		{
			get { return StdError.Count > 0; }
		}

		public List<string> StdOut { get; private set; }
		public List<string> StdError { get; private set; }
		public string WorkingDirectory { get; set; }

		static AbstractProcessTask()
		{
			projectDirectory = Project.RootDirectory;
		}

		private Action onCompleteCallback;

		protected AbstractProcessTask()
		{
			WorkingDirectory = projectDirectory;
			StdOut = new List<string>();
			StdError = new List<string>();
		}

		public void Execute(Action onComplete = null)
		{
			onCompleteCallback = onComplete;
			Run();
		}

		protected void RunProcessWithArgs(string fileName, string args)
		{
			Debug.Log("Executing:: " + args + "\r\nFrom " + WorkingDirectory);

			using (var process = new Process())
			{
				try
				{
					var gitInfo = new ProcessStartInfo
					{
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true,
						FileName = fileName,
						UseShellExecute = false,
						Arguments = args,
						WorkingDirectory = WorkingDirectory,
					};

					process.StartInfo = gitInfo;
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.EnableRaisingEvents = true;
					process.Exited += HandleGitProcessExited;
					process.OutputDataReceived += HandleGitProcessOutputDataReceived;
					process.ErrorDataReceived += HandleGitProcessErrorDataReceived;
					process.WaitForExit();
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					throw;
				}
				finally
				{
					process.Dispose();
				}
			}
		}

		private void HandleGitProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Debug.LogError(e.Data);
				StdError.Add(e.Data);
			}
		}

		private void HandleGitProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Debug.Log(e.Data);
				StdOut.Add(e.Data);
			}
		}

		private void HandleGitProcessExited(object sender, EventArgs eventArgs)
		{
			EditorApplication.update += EditorUpdate;
		}

		private void EditorUpdate()
		{
			EditorApplication.update -= EditorUpdate;
			if (onCompleteCallback != null) onCompleteCallback();
		}

		protected abstract void Run();
	}
}