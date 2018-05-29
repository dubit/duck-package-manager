using System;

namespace DUCK.PackageManager.Editor.Tasks
{
	public interface ITask
	{
		void Execute(Action onComplete = null);
	}
}