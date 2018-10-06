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
        public TileData (BoardPos _boardPos, int _colorID) {
    		boardPos = _boardPos;
    		colorID = _colorID;
    	}
    }
}