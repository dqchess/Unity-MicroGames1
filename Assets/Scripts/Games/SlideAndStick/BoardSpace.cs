using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardSpace {
		// Properties
		private BoardPos boardPos;
		private bool isPlayable = true;
        // References
        public BoardOccupant MyOccupant { get; private set; } // Occupants sit on my face. Only one Occupant occupies each space.
        private Wall[] myWalls; // references to the walls around me! Index = side (T, R, B, L).

		// Getters
		public bool IsPlayable { get { return isPlayable; } }
		public bool IsPos(Vector2Int _pos) { return _pos.x==Col && _pos.y==Row; }
		public int Col { get { return boardPos.col; } }
		public int Row { get { return boardPos.row; } }
        public bool IsOpen() {
			return isPlayable && MyOccupant==null;
		}
        public bool CanOccupantEverEnterMe (Vector2Int dir) { return CanOccupantEverEnterMe (MathUtils.GetSide(dir)); }
        public bool CanOccupantEverEnterMe (int side) {
            if (!IsPlayable) { return false; } // Unplayable? Return false.
            if (IsWallAtSide (side)) { return false; } // Wall in the way? Return false!
            return true; // Looks good!
        }
        public bool CanOccupantEverExit (int side) {
            // As long as there's no Wall here, we're good!
            return !IsWallAtSide (side);
        }
        private bool IsWallAtSide (int side) {
            return myWalls[side] != null;
        }
    

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public BoardSpace (Board _boardRef, BoardSpaceData _data) {
//			myBoard = _boardRef;
			boardPos = _data.boardPos;
			isPlayable = _data.isPlayable;
            myWalls = new Wall[4];
		}
		public BoardSpaceData SerializeAsData () {
			BoardSpaceData data = new BoardSpaceData(Col,Row);
			data.isPlayable = isPlayable;
			return data;
		}


		public void SetMyOccupant (BoardOccupant _bo) {
			if (MyOccupant != null) {
				throw new UnityException ("Oops! Trying to set a Space's Occupant, but that Space already has an Occupant! original: " + MyOccupant.GetType().ToString() + ", new: " + _bo.GetType().ToString() + ". " + Col + ", " + Row);
			}
			MyOccupant = _bo;
		}
		public void RemoveMyOccupant (BoardOccupant _bo) {
			if (MyOccupant != _bo) {
				throw new UnityException ("Oops! We're trying to remove a " + _bo.GetType().ToString() + " from a space that doesn't own it! " + Col + " " + Row + ".");
			}
			MyOccupant = null;
		}
        
        public void SetWallOnMe (Wall _wall, int side) {
            myWalls[side] = _wall;
        }


	}
}
