using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
    public enum RemovalTypes { Matched, ExtrudeSource } // TODO: Put this in its own class, goro

	public class Tile : BoardOccupant {
		// Properties
		private int colorID;
        public bool WasUsedInSearchAlgorithm=false; // for finding congruent tiles.

		// Getters
		public int ColorID { get { return colorID; } }

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public Tile (Board _boardRef, TileData _data) {
			base.InitializeAsBoardOccupant (_boardRef, _data);
			colorID = _data.colorID;
		}
		public TileData SerializeAsData() {
			TileData data = new TileData (BoardPos, colorID);
			return data;
		}

	}
}