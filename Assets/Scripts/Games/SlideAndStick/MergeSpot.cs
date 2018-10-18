using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	public class MergeSpot {
		// Properties
		public Vector2Int dir { get; private set; }
		public Vector2Int pos { get; private set; }
        
        
		public MergeSpot(Tile tileA, Tile tileB, Vector2Int pos, Vector2Int dir) {
			this.pos = pos;
			this.dir = dir;
            if (tileA.DidJustMove) {
                ReverseDir();
            }
		}
        
        private void ReverseDir() {
            pos += dir;
            dir = new Vector2Int(-dir.x, -dir.y);
        }
	}
}