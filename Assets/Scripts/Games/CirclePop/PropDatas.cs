using System.Collections.Generic;
using UnityEngine;

namespace CirclePop {
    
    public class PropData {
        public PropShapes shape=PropShapes.Circle;
        public Vector2 pos=Vector2.zero;
        public Vector2 size=new Vector2(30,30);
        public float rotation=0;
        public float rotateSpeed=0;
        public Vector2 posB=Vector2.positiveInfinity; // default to unused.
		public bool doHideMovePath=false; // only affects MOVING Props. Set to TRUE if we DON'T want to show the path (useful if it's only partially on screen and looks weird if we do show it).
        public float moveSpeed=1;
        public float moveLocOffset=0;
    }

    public class GrowerData : PropData {
        public bool doMoveWhenSolid=false;
		public float growSpeed=1;
		public float growSpeedBounce=1; // for GRAVITY BOUNCE grow pattern.
		public float growSpeedGravity=0; // for GRAVITY BOUNCE grow pattern.
		public float growSpeedMin=0; // for special OSCILLATION grow pattern.
		public float growSpeedMax=0; // for special OSCILLATION grow pattern.
		public float growSpeedOscFreq=0; // for special OSCILLATION grow pattern.
		public List<GrowerCompositePartData> parts=new List<GrowerCompositePartData>();
    }

    public class WallData : PropData {
    }


	/** Only for GrowerComposites. */
	public class GrowerCompositePartData {
		public PropShapes shape;
		public Vector2 pos;
		public Vector2 size;

		/// For circles, pass in "20,0, 30"; for rects, pass in "20,0, 30,30".
		public GrowerCompositePartData(string propertiesString) {
			string[] vals = propertiesString.Split(',');
			if (vals.Length < 3) { Debug.LogError("GrowerComposite part properties string doesn't contain enough values."); return; } // Safety check.
			pos = new Vector2(TextUtils.ParseFloat(vals[0]), TextUtils.ParseFloat(vals[1]));
			// Circle?
			if (vals.Length == 3) {
				shape = PropShapes.Circle;
				size = new Vector2(TextUtils.ParseFloat(vals[2]), TextUtils.ParseFloat(vals[2])); // radius,radius.
			}
			else {
				shape = PropShapes.Rect;
				size = new Vector2(TextUtils.ParseFloat(vals[2]), TextUtils.ParseFloat(vals[3])); // width,height.
			}
		}
	}

}