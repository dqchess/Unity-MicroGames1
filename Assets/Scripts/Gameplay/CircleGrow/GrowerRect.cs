using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
	public class GrowerRect : Grower {
		// Components
		[SerializeField] private BoxCollider2D myCollider;


//		override public void Initialize(Level _myLevel, Transform tf_parent, Vector2 _pos) {
//			base.Initialize(_myLevel, tf_parent, _pos);
//		}
		override protected void SetRadius(float _radius) {
			base.SetRadius(_radius);
			myCollider.size = new Vector2(Radius*2,Radius*2);
		}
	}
}