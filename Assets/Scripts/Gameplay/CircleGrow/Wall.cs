using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    abstract public class Wall : Prop {

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
		virtual public void Initialize(Level _myLevel, Transform tf_parent, Vector2 center, Vector2 size) {
			BaseInitialize(_myLevel, tf_parent, center, size);

            // Color me impressed!
            i_body.color = Grower.color_solid;
        }

    }
}
