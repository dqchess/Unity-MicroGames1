using UnityEngine;

namespace CircleGrow {
    
    public class PropData {
        public PropShapes shape=PropShapes.Circle;
        public Vector2 pos=Vector2.zero;
        public Vector2 size=new Vector2(30,30);
        public float rotation=0;
        public float rotateSpeed=0;
        public Vector2 posB=Vector2.positiveInfinity; // default to unused.
        public float moveSpeed=1;
        public float moveLocOffset=0;
    }

    public class GrowerData : PropData {
        public bool doMoveWhenSolid=false;
        public float growSpeed=1;
    }

    public class WallData : PropData {
    }

}