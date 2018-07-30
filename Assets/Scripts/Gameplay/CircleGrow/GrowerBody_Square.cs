using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class GrowerBody_Square : GrowerBody {
        // Components
        [SerializeField] private BoxCollider2D myCollider=null;


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        override public void SetRadius(float radius) {
            myCollider.size = new Vector2(radius*2, radius*2);
        }


    }
}
