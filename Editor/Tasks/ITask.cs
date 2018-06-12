using System;

namespace DUCK.PackageManager.Editor.Tasks
{
	internal interface ITask
	{
		void Execute(Action onComplete = null);
	}
}