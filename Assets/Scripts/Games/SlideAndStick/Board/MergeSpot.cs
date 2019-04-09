using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	public class MergeSpot {
		// Properties
        public int colorID { get; private set; }
        public Vector2Int dir { get; private set; }
        public Vector2 pos { get; private set; }
        
        // Getters
        public Vector2Int PosPlusDir() {
            //return pos + dir;
            return new Vector2Int(pos.x+dir.x, pos.y+dir.y);
        }
        
        
        public MergeSpot(Vector2 pos, Vector2Int dir, int colorID) {
            this.colorID = colorID;
            this.pos = pos;
            this.dir = dir;
        }
        public MergeSpot(Tile tileA, Vector2Int pos, Vector2Int dir) {
            this.colorID = tileA.ColorID;
            this.pos = pos.ToVector2();
            this.dir = dir;
            if (tileA.DidJustMove) {
                ReverseDir();
            }
        }
        
        private void ReverseDir() {
            pos += dir.ToVector2();
            dir = new Vector2Int(-dir.x, -dir.y);
        }
	}
}