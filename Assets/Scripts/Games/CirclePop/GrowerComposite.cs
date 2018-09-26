using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CirclePop {
	public class GrowerComposite : Grower {
		// Components
//		private Collider2D myCollider=null;
		// References
		[SerializeField] private Sprite s_circle;
		[SerializeField] private Sprite s_square;
		// Properties
//		private Vector2[] pointsNeutral; // the points that define my shape in its smallest, ungrown form.

		// Getters
		override public float Area() {
			return Size.x*Size.y;
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		override public void Initialize(Level _myLevel, Transform tf_parent, GrowerData data) {
			base.Initialize(_myLevel, tf_parent, data);

			// Add my components!
			for (int i=0; i<data.parts.Count; i++) {
				AddPart(data.parts[i]);
			}
		}
		private void AddPart(GrowerCompositePartData partData) {
			GameObject partGO = new GameObject();
			GameUtils.ParentAndReset(partGO, this.transform);
			partGO.transform.localPosition = partData.pos;
			partGO.name = "Part";

			Image img = partGO.AddComponent<Image>();

			switch (partData.shape) {
			case PropShapes.Circle:
				CircleCollider2D circleCol = partGO.AddComponent<CircleCollider2D>();
				circleCol.radius = partData.size.x*0.5f;
				img.sprite = s_circle;
				break;
			case PropShapes.Rect:
				BoxCollider2D boxCol = partGO.AddComponent<BoxCollider2D>();
				boxCol.size = partData.size;
				img.sprite = s_square;
				break;
			default:
				Debug.LogError("Oops! Part shape not recognized: " + partData.shape);
				break;
			}

			GameUtils.SizeUIGraphic(img, partData.size.x,partData.size.y);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		override public Prop SetSize(Vector2 _size) {
			base.SetSize(_size);
			// TODO: THIS
			this.transform.localScale = _size*0.033f; // TEST
			return this;
		}

		override protected void ApplyBodyColor() {
			base.ApplyBodyColor();

			// Just loop through all my children manually. :p
			Image[] allImages = GetComponentsInChildren<Image>();
			for (int i=0; i<allImages.Length; i++) {
				allImages[i].color = BodyColor;
			}
		}



	}
}