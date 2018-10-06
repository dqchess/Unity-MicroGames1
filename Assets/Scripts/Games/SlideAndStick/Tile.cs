using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class Tile : BoardOccupant {
        // Properties
        private int colorID;
		public int GroupID=-1; // which cluster I'm a part of! (CURRENTLY, code is simple-- all tiles are still 1x1, and just in clusters.)

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