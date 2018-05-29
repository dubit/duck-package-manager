using System;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DUCK.PackageManager.Editor.Tasks
{
	public abstract class AbstractProcessTask : ITask
	{
		private static readonly string projectDirectory;

		static AbstractProcessTask()
		{
			projectDirectory = Project.RootDirectory;
		}

		protected string workingDirectory;
		private Action onCompleteCallback;

		protected AbstractProcessTask()
		{
			workingDirectory = projectDirectory;
		}

		public void Execute(Action onComplete = null)
		{
			onCompleteCallback = onComplete;
			Run();
		}

		protected void RunProcessWithArgs(string fileName, string args)
		{
			Debug.Log("Executing:: " + args);

			var process = new Process();

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
					WorkingDirectory = workingDirectory,
				};

				process.StartInfo = gitInfo;
				process.Start();
				process.EnableRaisingEvents = true;
				process.Exited += HandleGitProcessExited;
				process.OutputDataReceived += HandleGitProcessOutputDataReceived;
				process.ErrorDataReceived += HandleGitProcessErrorDataReceived;
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
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

		private void HandleGitProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Debug.LogError(e.Data);
			}
		}

		private void HandleGitProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Debug.Log(e.Data);
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