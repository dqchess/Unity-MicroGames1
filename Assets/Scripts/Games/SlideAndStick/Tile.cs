using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class Tile : BoardOccupant {
        // Properties
        private int colorID;
        private int value;
        public bool WasUsedInSearchAlgorithm=false; // for finding congruent tiles.

        // Getters
        public int ColorID { get { return colorID; } }
        public int Value { get { return value; } }

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public Tile (Board _boardRef, TileData _data) {
			base.InitializeAsBoardOccupant (_boardRef, _data);
			colorID = _data.colorID;
            value = _data.value;
		}
		public TileData SerializeAsData() {
            TileData data = new TileData (BoardPos, colorID, value);
			return data;
		}

	}
}