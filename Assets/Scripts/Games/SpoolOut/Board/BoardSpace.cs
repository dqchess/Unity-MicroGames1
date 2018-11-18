using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
	public class BoardSpace {
        // Properties
        public BoardPos BoardPos { get; private set; }
        private bool isPlayable = true;
        // References
        public Spool MySpool { get; private set; } // Can be any part of a Spool.
        private Wall[] myWalls; // references to the walls around me! Index = side (T, R, B, L).

        // Getters
        public bool IsPlayable { get { return isPlayable; } }
        public bool IsPos(Vector2Int _pos) { return _pos.x == Col && _pos.y == Row; }
        public int Col { get { return BoardPos.col; } }
        public int Row { get { return BoardPos.row; } }
        public bool IsOpen() {
			return isPlayable && MySpool==null;
		}
        public bool CanOccupantEverEnterMe (Vector2Int dir) { return CanOccupantEverEnterMe (MathUtils.GetSide(dir)); }
        public bool CanOccupantEverEnterMe (int side) {
            if (!IsPlayable) { return false; } // Unplayable? Return false.
            if (HasWall(side)) { return false; } // Wall in the way? Return false!
            return true; // Looks good!
        }
        public bool CanOccupantEverExit (int side) {
            // As long as there's no Wall here, we're good!
			return !HasWall (side);
        }
		public bool HasWall (int side) {
            return myWalls[side] != null;
        }
    

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public BoardSpace (Board _boardRef, BoardSpaceData _data) {
//			myBoard = _boardRef;
			BoardPos = _data.boardPos;
			isPlayable = _data.isPlayable;
            myWalls = new Wall[4];
		}
		public BoardSpaceData SerializeAsData () {
			BoardSpaceData data = new BoardSpaceData(Col,Row);
			data.isPlayable = isPlayable;
			return data;
		}


		public void SetMySpool(Spool _spool) {
			if (MySpool != null) {
				//throw new UnityException NOTE: Disabled throwing exception! To Sherlock this: Check if my Board is same as Occupant's Board on phone.
                Debug.LogError("Oops! Trying to set a Space's MySpool, but that Space already has an MySpool! original: " + Col + ", " + Row);
			}
			MySpool = _spool;
		}
		public void RemoveMySpool(Spool _spool) {
			if (MySpool != _spool) {
				//throw new UnityException
                Debug.LogError("Oops! We're trying to remove a Spool from a space that doesn't own it! " + Col + " " + Row + ".");
			}
			MySpool = null;
		}
        
        public void SetWallOnMe (Wall _wall, int side) {
            myWalls[side] = _wall;
        }


	}
}
