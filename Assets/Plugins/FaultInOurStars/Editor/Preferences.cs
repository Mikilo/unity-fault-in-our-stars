using FaultInOurStars;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FaultInOurStarsEditor
{
    class FaultInOurStarsSettings : SettingsProvider
	{
		public const string	PresetsPathKeyPref = nameof(FaultInOurStarsSettings) + "_" + nameof(PresetsPath);
		public const string	LastSaveFilePathKeyPref = nameof(FaultInOurStarsSettings) + "_LastSaveFilePath";
		public const string	LastOpenFolderPathKeyPref = nameof(FaultInOurStarsSettings) + "_LastOpenFolderPath";

		public static string	PresetsPath
		{
			get
			{
				return EditorPrefs.GetString(FaultInOurStarsSettings.PresetsPathKeyPref, "Assets/Plugins/" + Constants.PackageName + "/" + nameof(StarPresets) + ".asset");
			}
			set
			{
				EditorPrefs.SetString(FaultInOurStarsSettings.PresetsPathKeyPref, value);
			}
		}

		[SettingsProvider]
		private static SettingsProvider	CreateProvider()
		{
			return new FaultInOurStarsSettings();
		}

		protected	FaultInOurStarsSettings() : base("Preferences/" + Constants.ReadableName, SettingsScope.User)
		{
			this.label = Constants.ReadableName;
		}

		public override void	OnTitleBarGUI()
		{
			GUILayout.Label(Constants.Version);
		}

		public override void	OnActivate(string searchContext, VisualElement rootElement)
		{
			base.OnActivate(searchContext, rootElement);
		}

		public override void	OnGUI(string searchContext)
		{
			EditorGUI.BeginChangeCheck();
			string	path = FaultInOurStarsSettings.PresetsPath;
			string	newPath = this.SaveFileField("Presets Path", path);
			if (EditorGUI.EndChangeCheck() && path != newPath)
			{
				if (AssetDatabase.LoadAssetAtPath<Object>(newPath) != null)
				{
					if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
					{
						if (EditorUtility.DisplayDialog(Constants.ReadableName, $"Asset at \"{newPath}\" already exist. Do you want to overwrite with the current?", "Yes", "No") == true)
						{
							AssetDatabase.DeleteAsset(newPath);
							AssetDatabase.MoveAsset(path, newPath);
						}
					}
				}
				else
				{
					if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
					{
						if (EditorUtility.DisplayDialog(Constants.ReadableName, $"Do you want to move the current to the new path?", "Yes", "No") == true)
							AssetDatabase.MoveAsset(path, newPath);
					}
				}

				FaultInOurStarsSettings.PresetsPath = newPath;
				StarPresets.instance = null;
			}
		}

		private string	SaveFileField(string label, string path)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.DelayedTextField(label, path);

				GUIContent content = new GUIContent();
				content.text = "Browse";

				Rect	r = GUILayoutUtility.GetRect(content, "ButtonLeft", GUILayout.ExpandWidth(false));
				r.y -= 2F;
				if (GUI.Button(r, content, "ButtonLeft") == true)
				{
					string	directory = string.IsNullOrEmpty(path) == false ? Path.GetDirectoryName(path) : EditorPrefs.GetString(FaultInOurStarsSettings.LastSaveFilePathKeyPref);
					string	projectPath = EditorUtility.SaveFilePanelInProject(label, Path.GetFileName(path), "asset", string.Empty, directory);

					if (string.IsNullOrEmpty(projectPath) == false)
					{
						EditorPrefs.SetString(FaultInOurStarsSettings.LastSaveFilePathKeyPref, Path.GetDirectoryName(projectPath));
						path = projectPath;
						GUI.FocusControl(null);
					}
				}

				using (new EditorGUI.DisabledScope(false))
				{
					GUI.enabled = true;
					content.text = "Open";

					r = GUILayoutUtility.GetRect(content, "ButtonRight", GUILayout.ExpandWidth(false));
					r.y -= 2F;
					if (GUI.Button(r, content, "ButtonRight") == true)
						EditorUtility.RevealInFinder(path);
				}
			}

			return path;
		}
	}
}