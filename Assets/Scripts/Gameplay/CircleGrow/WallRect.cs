using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class WallRect : Wall {
        // Components
        [SerializeField] private BoxCollider2D myCollider;


		override public void Initialize(Level _myLevel, Transform tf_parent, Vector2 center, Vector2 size) {
            base.Initialize(_myLevel, tf_parent, center, size);
            myCollider.size = size;
        }
    }
}