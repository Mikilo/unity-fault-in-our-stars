using UnityEditor;
using UnityEngine;

namespace FaultInOurStars
{
	public class Star : MonoBehaviour
	{
		public Color	color;
		public float	starRadius;
		public float	gravityWellRadius;
		public string	starName;

		public virtual void	OnDrawGizmos()
		{
			Gizmos.color = this.color;
			Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.localRotation, this.transform.localScale);
			Gizmos.DrawWireSphere(Vector3.zero, this.starRadius);

			//Gizmos.(Vector3.zero, this.starRadius);
			//Handles.DrawWireDisc(this.transform.position, this.transform.up, this.starRadius * this.gravityWellRadius);

			int		n = 0;
			float	f = 0f;

			while (n < 4)
			{
				float	a = Mathf.Exp(this.gravityWellRadius * f);

				Handles.DrawWireDisc(this.transform.position + this.transform.up * a, this.transform.up, this.starRadius + a);
				f += .25F;
				++n;
			}
		}
	}
}