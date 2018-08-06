using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
	public class LevelTile : BaseLevelTile {
		// Properties
		float colorOscLoc;


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            if (IsLocked) { return; }

            // Oscillate my color, bro!
            float oscSpeed = 1f;//MathUtils.Sin01(Time.time);
            colorOscLoc += (Time.deltaTime * oscSpeed) * 0.04f;
            //float posScaleX = 1f + MathUtils.Sin01(Time.time*0.3f+2);
            //float posScaleY = 1f + MathUtils.Sin01(Time.time*0.9f+3);
            //float h = (MathUtils.Sin01(colorOscLoc) + Mathf.Abs(pos.x*posScaleX+pos.y*posScaleY)*0.001f) % 1f;
            float posY = pos.y + levelSelectController.ScrollY*0.8f;
            float h = MathUtils.Sin01(colorOscLoc + posY*0.002f);
            h = (h + Mathf.Max(0, Mathf.PerlinNoise(pos.x*2+colorOscLoc, pos.y+colorOscLoc))*0.4f) % 1f;
            i_backing.color = new ColorHSB(h, 0.6f, 1f).ToColor();
        }


    }
}