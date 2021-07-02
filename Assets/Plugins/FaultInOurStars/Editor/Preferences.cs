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

		public enum FieldButtons
		{
			None = 0,
			All = Browse | Open,
			Browse = 1,
			Open = 2
		}

		public static string	PresetsPath
		{
			get
			{
				return EditorPrefs.GetString(FaultInOurStarsSettings.PresetsPathKeyPref, "Assets/Plugins/" + Constants.PackageName + "/StarPresets.asset");
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
			string newPath = FaultInOurStarsSettings.SaveFileField("Presets Path", FaultInOurStarsSettings.PresetsPath);
			if (EditorGUI.EndChangeCheck())
				FaultInOurStarsSettings.PresetsPath = newPath;
		}

		public static string	SaveFileField(string label, string path, string defaultName = "", string extension = "", FieldButtons buttons = FieldButtons.All)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (label != null)
					path = EditorGUILayout.TextField(label, path);
				else
					path = EditorGUILayout.TextField(path);

				if ((buttons & FieldButtons.Browse) != 0)
				{
					GUIContent content = new GUIContent();
					content.text = "Browse";

					Rect	r = GUILayoutUtility.GetRect(content, buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button, GUILayout.ExpandWidth(false));
					r.y -= 2F;
					if (GUI.Button(r, content, buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button) == true)
					{
						string	directory = string.IsNullOrEmpty(path) == false ? Path.GetDirectoryName(path) : EditorPrefs.GetString(FaultInOurStarsSettings.LastSaveFilePathKeyPref);
						string	projectPath = EditorUtility.SaveFilePanelInProject(label, directory, defaultName, extension);

						if (string.IsNullOrEmpty(projectPath) == false)
						{
							EditorPrefs.SetString(FaultInOurStarsSettings.LastSaveFilePathKeyPref, Path.GetDirectoryName(projectPath));
							path = projectPath;
							GUI.FocusControl(null);
						}
					}
				}

				if ((buttons & FieldButtons.Open) != 0)
				{
					using (new EditorGUI.DisabledScope(false))
					{
						GUI.enabled = true;
						GUIContent content = new GUIContent();
						content.text = "Open";

						Rect	r = GUILayoutUtility.GetRect(content, buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button, GUILayout.ExpandWidth(false));
						r.y -= 2F;
						if (GUI.Button(r, content, buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button) == true)
							EditorUtility.RevealInFinder(path);
					}
				}
			}

			return path;
		}

		public static string	OpenFolderField(string label, string path, FieldButtons buttons = FieldButtons.All)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (label != null)
					path = EditorGUILayout.TextField(label, path);
				else
					path = EditorGUILayout.TextField(path);

				if ((buttons & FieldButtons.Browse) != 0)
				{
					GUIContent content = new GUIContent();
					content.text = "Browse";

					Rect r = GUILayoutUtility.GetRect(content, buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button, GUILayout.ExpandWidth(false));
					r.y -= 2F;

					if (GUI.Button(r, "Browse", buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button) == true)
					{
						path = string.IsNullOrEmpty(path) == false ? path : EditorPrefs.GetString(FaultInOurStarsSettings.LastOpenFolderPathKeyPref);
						string projectPath = EditorUtility.OpenFolderPanel(label, path, string.Empty);

						if (string.IsNullOrEmpty(projectPath) == false)
						{
							EditorPrefs.SetString(FaultInOurStarsSettings.LastOpenFolderPathKeyPref, Path.GetDirectoryName(projectPath));
							path = projectPath;
							GUI.FocusControl(null);
						}
					}
				}

				if ((buttons & FieldButtons.Open) != 0)
				{
					using (new EditorGUI.DisabledScope(false))
					{
						GUIContent content = new GUIContent();
						content.text = "Open";

						GUI.enabled = true;
						Rect r = GUILayoutUtility.GetRect(content, buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button, GUILayout.ExpandWidth(false));
						r.y -= 2F;
						if (GUI.Button(r, content, buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button) == true)
							EditorUtility.RevealInFinder(path);
					}
				}
			}

			return path;
		}
	}
}