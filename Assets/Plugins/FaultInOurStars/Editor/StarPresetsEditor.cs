using FaultInOurStars;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace FaultInOurStarsEditor
{
	[CustomEditor(typeof(StarPresets))]
	public class StarPresetsEditor : Editor
	{
		[Serializable]
		private class StarJson
		{
			public string[]	presets;
		}

		public const string	JSONLastPath = nameof(StarEditor) + "_path";

		private GUIContent	createFromDragAndDropContent = new GUIContent("Create", "Drag & drop into the Scene to create one.");
		private GUIContent	createFromSelectionContent = new GUIContent("Create Preset from current selected GameObject");

		protected virtual void	OnEnable()
		{
			this.createFromSelectionContent.image = AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
		}

		public override void OnInspectorGUI()
		{
			if (this.target is StarPresets starPresets)
			{
				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Export as JSON"))
					{
						GameObject		go = new GameObject();
						Star			star = go.AddComponent<Star>();
						StarJson		json = new StarJson();

						json.presets = new string[starPresets.presets.Length];

						for (int i = 0, max = starPresets.presets.Length; i < max; ++i)
						{
							json.presets[i] = EditorJsonUtility.ToJson(starPresets.presets[i], false);
						}

						UnityEngine.Object.DestroyImmediate(go);

						string	path = EditorPrefs.GetString(StarPresetsEditor.JSONLastPath, nameof(StarPresets) + ".json");

						path = EditorUtility.SaveFilePanel(Constants.ReadableName, path, nameof(StarPresets) + ".json", "json");
					
						if (string.IsNullOrEmpty(path) == false)
						{
							try
							{
								File.WriteAllText(path, EditorJsonUtility.ToJson(json));
								EditorPrefs.SetString(StarPresetsEditor.JSONLastPath, path);
							}
							catch (Exception ex)
							{
								Debug.LogException(ex);
							}
						}
					}

					if (GUILayout.Button("Import from JSON"))
					{
						string path = EditorPrefs.GetString(StarPresetsEditor.JSONLastPath, nameof(StarPresets) + ".json");

						path = EditorUtility.OpenFilePanel(Constants.ReadableName, path, "json");

						if (string.IsNullOrEmpty(path) == false)
						{
							EditorPrefs.SetString(StarPresetsEditor.JSONLastPath, path);

							GameObject	go = new GameObject();

							try
							{
								string		rawJson = File.ReadAllText(path);
								StarJson	json = JsonUtility.FromJson<StarJson>(rawJson);
								Star		star = go.AddComponent<Star>();
								int			i = 0;

								AssetDatabase.StartAssetEditing();

								// Proceed to destroy all existing presets.
								while (i < starPresets.presets.Length)
								{
									UnityEngine.Object.DestroyImmediate(starPresets.presets[i], true);
									++i;
								}

								Array.Resize(ref starPresets.presets, 0);

								i = 0;

								// Then recreate the new ones. Because apparently EditorJsonUtility.FromJsonOverwrite() does not like overwriting on an existing Preset.
								for (int max = json.presets.Length; i < max; ++i)
								{
									string	element = json.presets[i];
									Preset	p = new Preset(star);

									EditorJsonUtility.FromJsonOverwrite(element, p);
									rawJson = EditorJsonUtility.ToJson(p, false);

									// Regenerate a Star from the Preset to inject into AddPreset() below.
									p.ApplyTo(star);

									starPresets.AddPreset(star);
								}

								AssetDatabase.StopAssetEditing();
								AssetDatabase.SaveAssets();
							}
							catch (Exception ex)
							{
								Debug.LogException(ex);
							}
							finally
							{
								UnityEngine.Object.DestroyImmediate(go);
							}
						}
					}
				}

				this.OnGUICreatePresetFromSelection();

				for (int i = 0, max = starPresets.presets.Length; i < max; ++i)
				{
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Preset " + (i + 1));

						if (GUILayout.Button("Edit"))
							Selection.activeObject = starPresets.presets[i];

						Event		currentEvent = Event.current;
						EventType	eventType = currentEvent.type;
						Rect		r = GUILayoutUtility.GetRect(this.createFromDragAndDropContent, GUI.skin.button);

						// Handle the drag & drop and overdraw with the button on top.
						if (eventType == EventType.MouseDown)
						{
							if (r.Contains(currentEvent.mousePosition))
							{
								DragAndDrop.PrepareStartDrag();
								DragAndDrop.objectReferences = new UnityEngine.Object[] { starPresets.presets[i] };
								DragAndDrop.paths = new string[] { AssetDatabase.GetAssetPath(starPresets) };
								currentEvent.Use();
							}
						}
						else if (eventType == EventType.MouseUp)
						{
							if (r.Contains(currentEvent.mousePosition))
							{
								EditorWindow window = EditorWindow.mouseOverWindow;
								if (window != null)
									window.ShowNotification(new GUIContent("Drag & drop into the Scene to create one."));
							}
						}
						else if (eventType == EventType.MouseDrag)
						{
							if (r.Contains(currentEvent.mousePosition))
							{
								DragAndDrop.StartDrag("Create");
								currentEvent.Use();
							}
						}
						else if (eventType == EventType.DragPerform)
						{
							DragAndDrop.AcceptDrag();
							currentEvent.Use();
						}

						if (eventType == EventType.Repaint)
							GUI.skin.button.Draw(r, this.createFromDragAndDropContent, r.Contains(currentEvent.mousePosition), false, true, false);

						if (GUILayout.Button("Delete"))
						{
							StarPresets.Instance.DeletePreset(i);
							break;
						}
					}
				}
			}
		}

		private void	OnGUICreatePresetFromSelection()
		{
			GameObject	selected = Selection.activeGameObject;

			if (selected != null)
			{
				Star	star = selected.GetComponent<Star>();

				if (star != null)
				{
					GUILayout.Space(10F);

					if (GUILayout.Button(this.createFromSelectionContent) == true)
						StarPresets.Instance.AddPreset(star);

					GUILayout.Space(10F);
				}
			}
		}
	}
}