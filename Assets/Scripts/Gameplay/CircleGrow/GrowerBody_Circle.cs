using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class GrowerBody_Circle : GrowerBody {
        // Components
        [SerializeField] private CircleCollider2D myCollider=null;


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        override public void SetRadius(float radius) {
            myCollider.radius = radius;
        }


    }
}
