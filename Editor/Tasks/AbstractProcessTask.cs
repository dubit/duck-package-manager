using System;
using System.Collections.Generic;
using System.Diagnostics;
using DUCK.Tasks;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DUCK.PackageManager.Editor.Tasks
{
	internal abstract class AbstractProcessTask : ITask<ProcessTaskResult>
	{
		private static readonly string projectDirectory;

		private readonly List<string> stdOut;
		private readonly List<string> stdErr;

		public string WorkingDirectory { get; set; }

		static AbstractProcessTask()
		{
			projectDirectory = Project.RootDirectory;
		}

		private Action<ProcessTaskResult> onCompleteCallback;
		private Process process;
		private int exitCode;

		protected AbstractProcessTask()
		{
			WorkingDirectory = projectDirectory;
			stdOut = new List<string>();
			stdErr = new List<string>();
		}

		public void Execute(Action<ProcessTaskResult> onComplete)
		{
			onCompleteCallback = onComplete;
			Run();
		}

		protected void RunProcessWithArgs(string fileName, string args)
		{
			Debug.Log("Executing: " + args + "\r\nFrom " + WorkingDirectory);

			process = new Process();

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
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				onCompleteCallback(new ProcessTaskResult(e));
				process.Dispose();
			}
		}

		private void HandleGitProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				stdErr.Add(e.Data);
			}
		}

		private void HandleGitProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				stdOut.Add(e.Data);
			}
		}

		private void HandleGitProcessExited(object sender, EventArgs eventArgs)
		{
			// Allow this to happen on next frame
			EditorApplication.update += NextFrame;
			exitCode = process.ExitCode;
			Debug.Log("Exit code: " + exitCode);
			process.Dispose();
		}

		private void NextFrame()
		{
			EditorApplication.update -= NextFrame;
			onCompleteCallback(new ProcessTaskResult(stdOut, stdErr, exitCode));
		}

		protected abstract void Run();
	}

	internal class ProcessTaskResult : OperationResult
	{
		public List<string> StdErr { get; private set; }
		public List<string> StdOut { get; private set; }
		public int ExitCode { get; private set; }

		public ProcessTaskResult(List<string> stdOut, List<string> stdErr, int exitCode)
		{
			StdOut = stdOut;
			StdErr = stdErr;
			ExitCode = exitCode;
			if (exitCode != 0)
			{
				Err(exitCode.ToString(),
					string.Format("Received exit code {0} from process", exitCode),
					exitCode);
			}
		}

		public ProcessTaskResult(Exception error)
		{
			StdOut = new List<string>();
			StdErr = new List<string>();
			Err(error);
		}
	}
}