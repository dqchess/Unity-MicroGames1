using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class WallCircle : Wall {
        // Components
        [SerializeField] private CircleCollider2D myCollider;


        override public void Initialize(Level _myLevel, Transform tf_parent, Vector2 center, Vector2 size) {
            base.Initialize(_myLevel, tf_parent, center, size);
            myCollider.radius = size.x*0.5f;
        }
    }
}