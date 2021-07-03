using FaultInOurStars;
using System.Collections.Generic;
using UnityEditor;
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
			// TODO Improve opening security by using OpenWindow().
			EditorWindow.GetWindow<PresetsWindow>(PresetsWindow.Title);
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

		/*
		// As you required me to not use my own tool and code, I did not use it. But for the sake of good share & will, here is my normal way to open an EditorWindow.

		private static bool			initializeOpenWindowMetadata;
		private static FieldInfo	m_Parent;

		public static void	OpenWindow(Type type, bool defaultIsUtility, string title, bool focus = true, Type nextTo = null, Action<EditorWindow> callback = null)
		{
			if (Utility.initializeOpenWindowMetadata == false)
			{
				Utility.initializeOpenWindowMetadata = true;
				Utility.LazyInitializeOpenWindowMetadata();
			}

			// Destroy orphan windows.
			if (Utility.m_Parent != null && typeof(EditorWindow).IsAssignableFrom(type) == true)
			{
				EditorWindow[]	windows = Resources.FindObjectsOfTypeAll(type) as EditorWindow[];

				for (int i = 0, max = windows.Length; i < max; ++i)
				{
					EditorWindow	window = windows[i];

					if (Utility.m_Parent.GetValue(window) == null)
						Object.DestroyImmediate(window);
				}
			}

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				EditorWindow	focusedWindow = EditorWindow.focusedWindow;

				GUICallbackWindow.Open(() =>
				{
					Event	currentEvent = Event.current;

					// Bug in 2020.1, GUICallbackWindow.Close() will destroy the utility window. Need to delay the creation after the call to Close().
					EditorApplication.delayCall += () =>
					{
						EditorWindow	instance;

						if (currentEvent.control == true)
						{
							instance = EditorWindow.CreateInstance(type) as EditorWindow;
							instance.titleContent.text = currentEvent.shift == defaultIsUtility && title.StartsWith("NG ") == true ? title.Substring(3) : title;

							if (nextTo != null)
								Utility.AddWindowNextTo(instance, nextTo);
							else
							{
								if (currentEvent.shift != defaultIsUtility)
									instance.ShowUtility();
								else
									instance.Show();
							}
						}
						else
						{
							if (currentEvent.shift != defaultIsUtility)
								instance = EditorWindow.GetWindow(type, true, title, focus);
							else
							{
								MethodInfo	getWindowMethod = typeof(EditorWindow).GetMethod("GetWindow", new Type[] { typeof(string), typeof(bool), typeof(Type[]) });
								MethodInfo	generic = getWindowMethod.MakeGenericMethod(type);

								instance = generic.Invoke(null, new object[] { null, focus, new Type[] { nextTo } }) as EditorWindow;
							}

							instance.titleContent.text = title.StartsWith("NG ") == true ? title.Substring(3) : title;
							if (instance.titleContent.image == null)
								instance.titleContent.image = UtilityResources.NGIcon;
						}

						if (focus == true)
							instance.Focus();
						else if (focusedWindow != null)
							focusedWindow.Focus();

						if (callback != null)
							callback(instance);
					};
				});
			}
			else
			{
				EditorWindow	instance = EditorWindow.GetWindow(type, false, title.StartsWith("NG ") == true ? title.Substring(3) : title, focus);

				if (instance.titleContent.image == null)
					instance.titleContent.image = UtilityResources.NGIcon;

				if (callback != null)
					callback(instance);
			}
		}

		// AssemblyVerifier is a utility class in NG Tools to automatically handles multiversions Reflection calls and warns the user if a call is missing.
		// Which, as a surprise, brought a great user experience, as I display a little warning window to let the user know that an API is missing and a report should be sent to the author via a single click.
		// Easy one step thing, users love to get in touch and feel they contributed to the development. Win/win.
		[AssemblyVerifier]
		private static void	LazyInitializeOpenWindowMetadata()
		{
			Utility.m_Parent = AssemblyVerifier.TryGetField(typeof(EditorWindow), "m_Parent", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		*/
	}
}