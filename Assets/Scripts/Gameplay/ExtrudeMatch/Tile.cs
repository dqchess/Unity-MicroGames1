using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	public class Tile : BoardOccupant {
		// Properties
		private int colorID;

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