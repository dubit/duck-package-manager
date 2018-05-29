namespace DUCK.PackageManager.Editor.Git
{
	public class CheckoutSubmoduleTask : GitTask
	{
		private const string COMMAND = "checkout {0}";

		public CheckoutSubmoduleTask(string installDirectory, string version) :
			base(string.Format(COMMAND, version))
		{
			workingDirectory = installDirectory;
		}
	}
}