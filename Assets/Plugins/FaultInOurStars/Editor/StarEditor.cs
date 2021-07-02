using FaultInOurStars;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FaultInOurStarsEditor
{
	[CustomEditor(typeof(Star))]
	public class StarEditor : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement	root = new VisualElement();

			if (this.target is Star star)
			{
				VisualElement	horizontalName = new VisualElement();
				horizontalName.style.flexDirection = FlexDirection.Row;
				root.Add(horizontalName);

				TextField	starName = new TextField(ObjectNames.NicifyVariableName(nameof(Star.starName)));
				starName.bindingPath = nameof(Star.starName);
				horizontalName.Add(starName);

				ColorField	color = new ColorField();
				color.bindingPath = nameof(Star.color);
				color.style.flexShrink = 1F;
				horizontalName.Add(color);

				FloatField	starRadius = new FloatField(ObjectNames.NicifyVariableName(nameof(Star.starRadius)));
				starRadius.bindingPath = nameof(Star.starRadius);
				starRadius.RegisterValueChangedCallback(e => { if (e.newValue < 0F) starRadius.value = 0F; });
				root.Add(starRadius);

				FloatField	gravityWellRadius = new FloatField(ObjectNames.NicifyVariableName(nameof(Star.gravityWellRadius)));
				gravityWellRadius.bindingPath = nameof(Star.gravityWellRadius);
				gravityWellRadius.RegisterValueChangedCallback(e => { if (e.newValue < 0F) gravityWellRadius.value = 0F; });
				root.Add(gravityWellRadius);
			}

			return root;
		}

		protected virtual void OnSceneGUI()
		{
			Star		t = target as Star;
			Transform	tr = t.transform;
			Vector3		pos = tr.position;

			Handles.color = t.color;
			Handles.Label(pos, t.starName + " (R=" + t.starRadius.ToString("F2") + ")");

			EditorGUI.BeginChangeCheck();
			float	r = Handles.RadiusHandle(tr.localRotation, pos, t.starRadius);
			if (EditorGUI.EndChangeCheck())
			{
				if (Event.current.shift) // Hold Shift to round radius.
					r = Mathf.Round(r);
				t.starRadius = r;
			}

			// TODO Switch handle for gravity well on Alt/Control?
		}
	}
}