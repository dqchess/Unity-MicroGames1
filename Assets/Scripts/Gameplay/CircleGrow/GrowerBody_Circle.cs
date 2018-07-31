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
            radius = radius - 1; // ooh, have a TIIIny amount of grace. :)
            myCollider.radius = radius;
        }


    }
}
