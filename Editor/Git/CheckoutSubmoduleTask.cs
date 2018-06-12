namespace DUCK.PackageManager.Editor.Git
{
	internal class CheckoutSubmoduleTask : GitTask
	{
		private const string COMMAND = "checkout {0}";

		public CheckoutSubmoduleTask(string installDirectory, string version) :
			base(string.Format(COMMAND, version))
		{
			WorkingDirectory = installDirectory;
		}
	}
}