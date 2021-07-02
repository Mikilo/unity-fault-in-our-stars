using FaultInOurStars;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace FaultInOurStarsEditor
{
	public class PresetsWindow : EditorWindow
	{
		public const string	Title = "Star Presets";

		private Editor			starPresetsEditor;
		private List<Editor>	subEditors = new List<Editor>();
		private Vector2			scrollPosition;

		[MenuItem(Constants.ReadableName + "/" + PresetsWindow.Title)]
		public static void	Open()
		{
			// TODO Improve opening security.
			EditorWindow.GetWindow<PresetsWindow>(PresetsWindow.Title);
		}

		[Shortcut(Constants.ReadableName + "/Create Preset", KeyCode.F4, ShortcutModifiers.Control)]
		public static void	CreatePresetFromShortcut()
		{
			GameObject	selected = Selection.activeGameObject;

			if (selected != null)
			{
				Star	star = selected.GetComponent<Star>();

				if (star != null)
				{
					StarPresets.Instance.AddPreset(star);
					return;
				}
			}

			EditorUtility.DisplayDialog(Constants.ReadableName, $"You need to select a {nameof(GameObject)} with a {nameof(Component)} {nameof(Star)}.", "OK");
		}

		[MenuItem("CONTEXT/" + nameof(Star) + "/Create Preset")]
		public static void	AddPreset(MenuCommand command)
		{
			StarPresets.Instance.AddPreset((Star)command.context);
		}

		protected virtual void	OnEnable()
		{
			Selection.selectionChanged += this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			using (var scroll = new GUILayout.ScrollViewScope(this.scrollPosition))
			{
				this.scrollPosition = scroll.scrollPosition;

				StarPresets	starPresets = StarPresets.Instance;

				if (starPresets == null)
					EditorGUILayout.HelpBox("Preset asset not created yet. Generate a preset by pressing F4 to make one automatically.", MessageType.Warning);
				else
				{
					if (this.starPresetsEditor == null || this.starPresetsEditor.target != starPresets)
						this.starPresetsEditor = Editor.CreateEditor(starPresets);

					if (this.starPresetsEditor != null)
						this.starPresetsEditor.OnInspectorGUI();
				}
			}
		}
	}
}