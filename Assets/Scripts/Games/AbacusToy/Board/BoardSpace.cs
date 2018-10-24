using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
	public class BoardSpace {
		// Properties
		private BoardPos boardPos;
		private bool isPlayable = true;
		// References
//		private Board myBoard;
		private BoardOccupant myOccupant; // occupants sit on my face. Only one Occupant occupies each space.

		// Getters
		public bool IsPlayable { get { return isPlayable; } }
		public bool IsPos(Vector2Int _pos) { return _pos.x==Col && _pos.y==Row; }
		public int Col { get { return boardPos.col; } }
		public int Row { get { return boardPos.row; } }
		public BoardOccupant MyOccupant { get { return myOccupant; } }
		public bool IsOpen() {
			return isPlayable && myOccupant==null;
		}

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public BoardSpace (Board _boardRef, BoardSpaceData _data) {
//			myBoard = _boardRef;
			boardPos = _data.boardPos;
			isPlayable = _data.isPlayable;
		}
		public BoardSpaceData SerializeAsData () {
			BoardSpaceData data = new BoardSpaceData(Col,Row);
			data.isPlayable = isPlayable;
			return data;
		}


		public void SetMyOccupant (BoardOccupant _bo) {
			if (myOccupant != null) {
				throw new UnityException ("Oops! Trying to set a Space's Occupant, but that Space already has an Occupant! original: " + myOccupant.GetType().ToString() + ", new: " + _bo.GetType().ToString() + ". " + Col + ", " + Row);
			}
			myOccupant = _bo;
		}
		public void RemoveMyOccupant (BoardOccupant _bo) {
			if (myOccupant != _bo) {
				throw new UnityException ("Oops! We're trying to remove a " + _bo.GetType().ToString() + " from a space that doesn't own it! " + Col + " " + Row + ".");
			}
			myOccupant = null;
		}


	}
}