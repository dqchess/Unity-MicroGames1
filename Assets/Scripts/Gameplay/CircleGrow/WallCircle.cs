using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class WallCircle : Wall {
        // Components
        [SerializeField] private CircleCollider2D myCollider;


        override public void Initialize(Transform tf_parent, Vector2 center, Vector2 size) {
            base.Initialize(tf_parent, center, size);
            myCollider.radius = size.x*0.5f;
        }
    }
}