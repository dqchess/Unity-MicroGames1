using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CirclePop {
	public class GrowerRect : Grower {
		// Components
		[SerializeField] private BoxCollider2D myCollider=null;

        // Getters
        override public float Area() {
            return Size.x*Size.y;
        }


//		override public void Initialize(Level _myLevel, Transform tf_parent, Vector2 _pos) {
//			base.Initialize(_myLevel, tf_parent, _pos);
//		}
		//override protected void SetRadius(float _radius) {
			//base.SetRadius(_radius);
			//myCollider.size = new Vector2(Radius*2,Radius*2);
        //}
        override public Prop SetSize(Vector2 _size) {
            base.SetSize(_size);
            myCollider.size = _size;
            return this;
        }
	}
}