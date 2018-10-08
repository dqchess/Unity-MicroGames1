using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SlideAndStick {
    public class PropData {
    }

    public class BoardObjectData : PropData {
    	public BoardPos boardPos;
    }
    public class BoardOccupantData : BoardObjectData {
		public List<Vector2Int> footprintLocal=new List<Vector2Int>{Vector2Int.zero}; // by default, start with one space.
    }
    public class BoardSpaceData : BoardObjectData {
    	public bool isPlayable = true;
    	public BoardSpaceData (int _col,int _row) {
    		boardPos.col = _col;
    		boardPos.row = _row;
    	}
    }

    public class TileData : BoardOccupantData {
		public int colorID;
		public TileData (BoardPos _boardPos, int _colorID) { // TEMP FOR debugging
			boardPos = _boardPos;
			colorID = _colorID;
		}
		public TileData (BoardPos _boardPos, int _colorID, List<Vector2Int> _footprintLocal) {
    		boardPos = _boardPos;
    		colorID = _colorID;
			footprintLocal = _footprintLocal;
    	}
    }
}