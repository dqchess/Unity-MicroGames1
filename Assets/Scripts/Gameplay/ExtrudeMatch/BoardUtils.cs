using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
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


		public static Vector2Int GetDir (int side) {
			switch (side) {
			case Sides.L: return Vector2Int.L;
			case Sides.R: return Vector2Int.R;
			case Sides.B: return Vector2Int.B;
			case Sides.T: return Vector2Int.T;
			case Sides.TL: return Vector2Int.TL;
			case Sides.TR: return Vector2Int.TR;
			case Sides.BL: return Vector2Int.BL;
			case Sides.BR: return Vector2Int.BR;
			default: throw new UnityException ("Whoa, " + side + " is not a valid side. Try 0 through 7.");
			}
		}
		public static int GetSide (Vector2Int dir) {
			if (dir == Vector2Int.L) { return Sides.L; }
			if (dir == Vector2Int.R) { return Sides.R; }
			if (dir == Vector2Int.B) { return Sides.B; }
			if (dir == Vector2Int.T) { return Sides.T; }
			if (dir == Vector2Int.TL) { return Sides.TL; }
			if (dir == Vector2Int.TR) { return Sides.TR; }
			if (dir == Vector2Int.BL) { return Sides.BL; }
			if (dir == Vector2Int.BR) { return Sides.BR; }
			return Sides.Undefined; // Whoops.
		}
		public static int GetOppositeSide (Vector2Int dir) { return GetOppositeSide(GetSide(dir)); }
		public static int GetOppositeSide (int side) {
			return Sides.GetOpposite(side);
		}
		public static Vector2Int GetOppositeDir (int side) { return GetDir(GetOppositeSide(side)); }
		/** Useful for flipping dirEntering to dirExiting, for example. Just returns the original value * -1. */
		public static Vector2Int GetOppositeDir (Vector2Int dir) { return new Vector2Int(-dir.x, -dir.y); }


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


	}
}
