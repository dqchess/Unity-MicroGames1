using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class Tile : BoardOccupant {
        // Properties
        private int colorID;

        // Getters
        public int ColorID { get { return colorID; } }

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public Tile (Board _boardRef, TileData _data) {
			base.InitializeAsBoardOccupant(_boardRef, _data);
			colorID = _data.colorID;
		}
		public TileData SerializeAsData() {
			TileData data = new TileData(BoardPos, colorID, new List<Vector2Int>(FootprintLocal)); // note: COPY the Vector2Int list. DEF don't want a reference.
			return data;
		}

	}
}