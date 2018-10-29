using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	abstract public class BoardObject {
		// Properties
		private BoardPos boardPos; // col, row, and sideFacing!
		private bool isInPlay = true; // we set this to false when I'm removed from the Board!
		// References
		protected Board boardRef;

		// Getters
		public bool IsInPlay { get { return isInPlay; } }
		public BoardPos BoardPos { get { return boardPos; } }
		public int Col { get { return boardPos.col; } }
		public int Row { get { return boardPos.row; } }
		public int SideFacing { get { return boardPos.sideFacing; } }
		public Board BoardRef { get { return boardRef; } }
        protected BoardSpace GetSpace (Vector2Int _boardPos) { return GetSpace(_boardPos.x,_boardPos.y); }
        protected BoardSpace GetSpace (int _col,int _row) { return BoardUtils.GetSpace (boardRef, _col,_row); }
//		public BoardSpace MySpace { get { return GetSpace (Col,Row); } }

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		protected void InitializeAsBoardObject (Board _boardRef, BoardPos _boardPos) {//BoardObjectData _data
			boardRef = _boardRef;
			boardPos = _boardPos;

			// Call my "official" set-property functions so the accompanying stuff happens too. (Note: In SlideAndStick as is, there's no accompanying stuff.)
			SetColRow (boardPos.col,boardPos.row);
			SetSideFacing (boardPos.sideFacing);
			// Automatically add me to the board!
			AddMyFootprint ();
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		virtual public void SetColRow (int _col, int _row) {
			boardPos.col = _col;
			boardPos.row = _row;
		}
		public void SetSideFacing (int _sideFacing) {
			boardPos.sideFacing = _sideFacing;
		}

		/** This removes me from the Board completely and permanently. */
		public void RemoveFromPlay () {
			// Gemme outta here!
			isInPlay = false;
			RemoveMyFootprint();
			// Tell my boardRef I'm toast!
			boardRef.OnObjectRemovedFromPlay (this);
		}

		// Override these!
		virtual public void AddMyFootprint () {}//abstract 
		virtual public void RemoveMyFootprint () {}//abstract 


	}
}