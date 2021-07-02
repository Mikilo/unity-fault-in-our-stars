using FaultInOurStars;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace FaultInOurStarsEditor
{
	public class StarPresets : ScriptableObject
	{
		private static StarPresets	instance;
		public static StarPresets	Instance
		{
			get
			{
				if (StarPresets.instance == null)
				{
					StarPresets.instance = AssetDatabase.LoadAssetAtPath<StarPresets>(StarPresets.GetFilePath());

					if (StarPresets.instance == null)
					{
						StarPresets.instance = ScriptableObject.CreateInstance<StarPresets>();
						AssetDatabase.CreateAsset(StarPresets.instance, StarPresets.GetFilePath());
						AssetDatabase.SaveAssets();
					}
				}

				return StarPresets.instance;
			}
		}

		[NonSerialized]
		public Preset[]	presets;

		private static string	GetFilePath()
		{
			return FaultInOurStarsSettings.PresetsPath;
		}

		protected virtual void	OnEnable()
		{
			this.name = nameof(StarPresets);

			UnityEngine.Object[]	allAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			List<Preset>			list = new List<Preset>();

			for (int i = 0, max = allAssets.Length; i < max; ++i)
			{
				if (allAssets[i] is Preset p)
					list.Add(p);
			}

			this.presets = list.ToArray();
		}

		public void	AddPreset(Star star)
		{
			Array.Resize(ref this.presets, this.presets.Length + 1);
			this.presets[this.presets.Length - 1] = new Preset(star);
			this.presets[this.presets.Length - 1].name = "Preset " + this.presets.Length;

			AssetDatabase.AddObjectToAsset(this.presets[this.presets.Length - 1], StarPresets.GetFilePath());
			AssetDatabase.SaveAssets();
		}

		public void	DeletePreset(int i)
		{
			List<Preset>	list = new List<Preset>(this.presets);

			UnityEngine.Object.DestroyImmediate(this.presets[i], true);
			list.RemoveAt(i);

			for (i = list.Count - 1; i >= 0; --i)
				list[i].name = "Preset " + (i + 1);

			this.presets = list.ToArray();

			AssetDatabase.SaveAssets();
		}
	}
}