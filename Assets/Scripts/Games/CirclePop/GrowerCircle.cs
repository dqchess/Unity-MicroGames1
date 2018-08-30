using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CirclePop {
	public class GrowerCircle : Grower {
		// Components
        [SerializeField] private CircleCollider2D myCollider=null;
        // Properties
        private float radius; // updated when we call SetSize.

        // Getters
        override public float Area() { return Mathf.PI * radius*radius; }


//		override public void Initialize(Level _myLevel, Transform tf_parent, Vector2 center, Vector2 size) {
//			base.Initialize(_myLevel, tf_parent, center, size);
//			myCollider.radius = Radius;
//		}

		//override protected void SetRadius(float _radius) {
			//base.SetRadius(_radius);
			//myCollider.radius = Radius;
        //}
        override public Prop SetSize(Vector2 _size) {
            base.SetSize(_size);
            radius = _size.x*0.5f; // just use x. No ellipses in this game.
            myCollider.radius = radius;
            return this;
        }
	}
}