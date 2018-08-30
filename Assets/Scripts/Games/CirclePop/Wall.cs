using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CirclePop {
    abstract public class Wall : Prop {

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(Level _myLevel, Transform tf_parent, WallData data) {// Vector2 center, Vector2 size) {
			BaseInitialize(_myLevel, tf_parent, data);

            // Color me impressed!
			bodyColor = Grower.color_solid(myLevel.LevelIndex);
		}

		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		override public void OnIllegalOverlap() {
//			// Color me disappointed!
//			bodyColor = Grower.color_illegal;
		}
    }
}
