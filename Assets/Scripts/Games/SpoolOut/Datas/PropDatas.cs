using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpoolOut {
    public class PropData {
    }

    public class BoardObjectData : PropData {
    	public BoardPos boardPos;
    }
    public class BoardSpaceData : BoardObjectData {
    	public bool isPlayable = true;
    	public BoardSpaceData (int _col,int _row) {
    		boardPos.col = _col;
    		boardPos.row = _row;
    	}
    }

    public class SpoolData : BoardObjectData {
        public int numSpacesToFill;
        public int colorID;
        public List<Vector2Int> pathSpaces;//=new List<Vector2Int>{Vector2Int.zero}; // by default, start with one space.
        //public SpoolData (BoardPos _boardPos, int _colorID) { // TEMP FOR debugging
        //    boardPos = _boardPos;
        //    colorID = _colorID;
        //}
        public SpoolData (BoardPos _boardPos, int _colorID, List<Vector2Int> _pathSpaces) {
            boardPos = _boardPos;
            colorID = _colorID;
            if (_pathSpaces == null) { // Convenience check! Default our pathSpaces, mmkay?
                _pathSpaces = new List<Vector2Int> { new Vector2Int(boardPos.col,boardPos.row) };
            }
            pathSpaces = new List<Vector2Int>(_pathSpaces);
        }
    }

    public class WallData : BoardObjectData {
        public bool IsVertical() { return boardPos.sideFacing==1 || boardPos.sideFacing==3; } // note that 3 should never be passed in. 0 and 1 only (so we can never have two walls in the same space accidentally).
        public WallData(BoardPos _boardPos) {
            boardPos = _boardPos;
        }
    }
}