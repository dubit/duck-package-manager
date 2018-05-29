using UnityEditor;
using UnityEngine;

namespace DUCK.PackageManager.Editor.Preferences
{
	public class DuckPreferences
	{
		[PreferenceItem("Duck")]
		private static void CustomPreferencesGUI()
		{
			EditorGUILayout.LabelField("Package Manager", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Version: " + Settings.VERSION);
			EditorGUILayout.Space();

			var settings = Settings.AllSettings;

			foreach (var setting in settings.Values)
			{
				var key = setting.Key;
				var oldValue = setting.Value;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(key), GUILayout.Width(130));
				var newValue = EditorGUILayout.TextField(setting.Value);
				if (oldValue != newValue)
				{
					setting.Set(newValue);
				}

				EditorGUILayout.EndHorizontal();
			}
		}
	}
}