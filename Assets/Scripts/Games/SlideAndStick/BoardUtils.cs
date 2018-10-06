using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardUtils {
		
		// ----------------------------------------------------------------
		//  Basic Getters
		// ----------------------------------------------------------------
		public static bool IsSpaceEven (int col,int row) {
			bool isColEven = col%2 == 0;
			if (row%2 == 0) { // If it's an EVEN row, return if it's an even col!
				return isColEven;
			}
			return !isColEven; // If it's an ODD row, return if it's NOT an even col!
		}


		public static BoardSpace GetSpace(Board b, int col, int row) {
			if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) return null;
			return b.Spaces[col,row];
		}
		public static BoardOccupant GetOccupant(Board b, BoardPos boardPos) { return GetOccupant(b, boardPos.col,boardPos.row); }
		public static BoardOccupant GetOccupant(Board b, int col,int row) {
			BoardSpace space = GetSpace (b, col,row);
			if (space==null) { return null; }
			return space.MyOccupant;
		}

		public static bool IsSpaceOpen(Board b, BoardPos pos) { return IsSpaceOpen(b, pos.col,pos.row); }
		public static bool IsSpaceOpen(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsOpen();
		}
		public static bool IsSpacePlayable(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsPlayable;
		}
		public static BoardOccupant GetOccupantInClonedBoard(BoardOccupant sourceBO, Board newBoard) {
			return GetOccupant(newBoard, sourceBO.Col,sourceBO.Row);
		}



		public static BoardPos GetRandOpenPos(Board b) {
			int safetyCount = 0;
			while(safetyCount++ < 99) {
				int randCol = Random.Range(0, b.NumCols);
				int randRow = Random.Range(0, b.NumRows);
				if (b.GetSpace(randCol,randRow).IsOpen()) {
					return new BoardPos(randCol, randRow);
				}
			}
			return BoardPos.undefined;
		}

		public static int GetNumPlayableSpaces(Board b) {
			int total = 0;
			for (int col=0; col<b.NumCols; col++) {
				for (int row=0; row<b.NumRows; row++) {
					if (IsSpacePlayable(b, col,row)) { total ++; }
				}
			}
			return total;
		}



		public static bool CanExecuteMove(Board originalBoard, BoardPos boToMovePos, Vector2Int dir) {
			// Clone the Board, execute the exact move, and return if it worked.
			Board testBoard = originalBoard.Clone();
			MoveResults moveResult = testBoard.ExecuteMove(boToMovePos, dir);
			return moveResult != MoveResults.Fail;
		}
		public static bool CanOccupantMoveInDir(Board originalBoard, BoardOccupant originalBO, Vector2Int moveDir) {
			Board testBoard = originalBoard.Clone();
			BoardOccupant testBO = GetOccupant(testBoard, originalBO.BoardPos.col,originalBO.BoardPos.row);
			MoveResults moveResult = MoveOccupant(testBoard, testBO, moveDir);
			return moveResult == MoveResults.Success;
		}


		public static MoveResults MoveOccupant (Board b, BoardOccupant bo, Vector2Int dir) {
			if (bo == null) {
				Debug.LogError ("Oops! We're trying to move a null Occupant! dir: " + dir.ToString());
				return MoveResults.Fail;
			}

			int newCol = bo.Col+dir.x;
			int newRow = bo.Row+dir.y;

			// Is this outside the board? Oops! Return Fail.
			if (!IsSpacePlayable(b, newCol,newRow)) {
				return MoveResults.Fail;
			}
			// Are we trying to pass through a Wall? Return Fail.
//			if (bo.MySpace!=null && !bo.MySpace.CanOccupantEverExit(GetSide(dir))) {
//				return MoveResults.Fail;
//			}

			// Always remove its footprint first. We're about to move it!
			bo.RemoveMyFootprint ();

			bo.SetColRow (newCol,newRow);

			// Is there another Occupant in this next space that we might displace??
			BoardOccupant nextBO_touching = GetOccupant(b, newCol,newRow);
			if (nextBO_touching != null) {
				// PUSH the next guy! Interrupt this function with a recursive call! (And we'll add everyone back to the board at the end.)
				MoveResults nextResult = MoveOccupant(b, nextBO_touching, dir);
				// Whoa, if the recursive move we just made DIDN'T work, then stop here and return that info.
				if (nextResult != MoveResults.Success) {
					return nextResult;
				}
			}

			// Add its footprint back now.
			bo.AddMyFootprint ();

			// Success!
			return MoveResults.Success;
		}


	}
}
