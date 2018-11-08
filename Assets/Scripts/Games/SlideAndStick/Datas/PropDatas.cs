using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SlideAndStick {
    [System.Serializable]
    public class PropData {
    }

    [System.Serializable]
    public class BoardObjectData : PropData {
    	public BoardPos boardPos;
    }
    [System.Serializable]
    public class BoardOccupantData : BoardObjectData {
		public List<Vector2Int> footprintLocal=new List<Vector2Int>{Vector2Int.zero}; // by default, start with one space.
    }
    [System.Serializable]
    public class BoardSpaceData : BoardObjectData {
    	public bool isPlayable = true;
    	public BoardSpaceData (int _col,int _row) {
    		boardPos.col = _col;
    		boardPos.row = _row;
    	}
    }

    [System.Serializable]
    public class TileData : BoardOccupantData {
        public int colorID;
        public TileData (BoardPos _boardPos, int _colorID) { // TEMP FOR debugging
            boardPos = _boardPos;
            colorID = _colorID;
        }
        public TileData (BoardPos _boardPos, int _colorID, List<Vector2Int> _footprintLocal) {
            boardPos = _boardPos;
            colorID = _colorID;
            footprintLocal = new List<Vector2Int>(_footprintLocal);
        }
    }

    [System.Serializable]
    public class WallData : BoardObjectData {
        public bool IsVertical() { return boardPos.sideFacing==1 || boardPos.sideFacing==3; } // note that 3 should never be passed in. 0 and 1 only (so we can never have two walls in the same space accidentally).
        public WallData(BoardPos _boardPos) {
            boardPos = _boardPos;
        }
    }
}