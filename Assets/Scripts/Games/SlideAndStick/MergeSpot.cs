using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	public class MergeSpot {
		// Properties
		//		public List<Vector2Int> tileAFootGlobal { get; private set; }
		//		public List<Vector2Int> tileBFootGlobal { get; private set; }
		public Vector2Int dir { get; private set; }
		public Vector2Int pos { get; private set; }
		public MergeSpot(Tile tileA, Tile tileB, Vector2Int pos, Vector2Int dir) {
			//			tileAFootGlobal = new List<Vector2Int>(tileA.FootprintGlobal);
			//			tileBFootGlobal = new List<Vector2Int>(tileB.FootprintGlobal);
			this.pos = pos;
			this.dir = dir;
		}
	}
}