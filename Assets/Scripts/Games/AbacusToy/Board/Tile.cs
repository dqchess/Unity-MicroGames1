using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
	public class Tile : BoardOccupant {
        // Properties
        public int ColorID { get; private set; }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public Tile (Board _boardRef, TileData _data) {
			base.InitializeAsBoardOccupant(_boardRef, _data);
			ColorID = _data.colorID;
		}
		public TileData SerializeAsData() {
			TileData data = new TileData(BoardPos, ColorID, new List<Vector2Int>(FootprintLocal)); // note: COPY the Vector2Int list. DEF don't want a reference.
			return data;
		}

	}
}