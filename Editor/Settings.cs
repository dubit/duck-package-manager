using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DUCK.PackageManager.Editor
{
	internal static class Settings
	{
		public const string VERSION = "0.1.0 (preview)";

		public static string GitExecutablePath
		{
			get { return AllSettings[Keys.GIT_EXECUTABLE_PATH].Value; }
		}

		public static string RelativePackagesDirectoryPath
		{
			get { return AllSettings[Keys.PACKAGES_DIRECTORY_PATH].Value; }
		}

		public static string AbsolutePackagesDirectoryPath
		{
			get { return Project.RootDirectory + "/" + RelativePackagesDirectoryPath; }
		}

		public static string RelativePackagesJsonFilePath
		{
			get { return AllSettings[Keys.PACKAGES_JSON_PATH].Value; }
		}

		public static string AbsolutePackagesJsonFilePath
		{
			get { return Application.dataPath + RelativePackagesJsonFilePath; }
		}

		public static Dictionary<string, Setting> AllSettings { get; private set; }

		static Settings()
		{
			AllSettings = new Dictionary<string, Setting>();

			AddSetting(Keys.GIT_EXECUTABLE_PATH, "C:/Program Files/Git/bin/git.exe");
			AddSetting(Keys.PACKAGES_DIRECTORY_PATH, "Assets/Duck/Packages/");
			AddSetting(Keys.PACKAGES_JSON_PATH, "/Duck/packages.json");
		}

		private static void AddSetting(string key, string defaultValue)
		{
			AllSettings.Add(key, new Setting(key, defaultValue));
		}

		private static class Keys
		{
			public const string GIT_EXECUTABLE_PATH = "gitExecutablePath";
			public const string PACKAGES_DIRECTORY_PATH = "packagesDirPath";
			public const string PACKAGES_JSON_PATH = "packagesJsonPath";
		}

		public class Setting
		{
			public string Key { get; private set; }
			public string Value { get; private set; }

			public Setting(string key, string value)
			{
				Key = key;
				Value = value;

				if (EditorPrefs.HasKey(Key))
				{
					Value = EditorPrefs.GetString(Key);
				}
			}

			public void Set(string value)
			{
				Value = value;

				EditorPrefs.SetString(Key, value);
			}
		}
	}
}