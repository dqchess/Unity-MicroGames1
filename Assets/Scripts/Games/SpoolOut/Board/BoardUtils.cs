using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
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


        public static BoardSpace GetSpace(Board b, BoardPos pos) { return GetSpace(b, pos.col,pos.row); }
        public static BoardSpace GetSpace(Board b, Vector2Int pos) { return GetSpace(b, pos.x,pos.y); }
        public static BoardSpace GetSpace(Board b, int col, int row) {
			if (col<0 || row<0  ||  col>=b.NumCols || row>=b.NumRows) return null;
			return b.Spaces[col,row];
		}
        public static Spool GetSpool(Board b, int col,int row) {
            BoardSpace space = GetSpace(b, col,row);
            return space==null ? null : space.MySpool;
        }
        //public static Wall GetWall(Board b, int col,int row) {
        //    BoardSpace space = GetSpace (b, col,row);
        //    if (space==null) { return null; }
        //    return space.WallOnMe;
        //}

        public static bool IsSpaceOpen(Board b, BoardPos pos) { return IsSpaceOpen(b, pos.col,pos.row); }
        public static bool IsSpaceOpen(Board b, Vector2Int pos) { return IsSpaceOpen(b, pos.x,pos.y); }
		public static bool IsSpaceOpen(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsOpen();
		}
		public static bool IsSpacePlayable(Board b, Vector2Int pos) { return IsSpacePlayable(b, pos.x,pos.y); }
		public static bool IsSpacePlayable(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsPlayable;
		}

        
        public static bool CanSpoolPathEnterSpace(Board b, Spool spool, Vector2Int spacePos) {
            if (spool.numSpacesLeft <= 0) { return false; } // Whoa, no spaces left in it? Naaah.
            return IsSpaceOpen(b, spacePos);
        }


		public static BoardPos GetRandPosWithoutWall(Board b) {
            if (b.NumCols*b.NumRows > 1) { // Safety check (if only one space, we can't fit a wall anywhere).
    			int safetyCount = 0;
    			while(safetyCount++ < 99) {
    				int side = MathUtils.RandBool() ? Sides.T : Sides.L;
    				int col = Random.Range(side==Sides.T?0:1, b.NumCols); // note: don't put a wall on the edge of the Board (a-doy)
    				int row = Random.Range(side==Sides.T?1:0, b.NumRows); // note: don't put a wall on the edge of the Board (a-doy)
    				if (!b.GetSpace(col,row).HasWall(side)) {
    					return new BoardPos(col,row, side);
    				}
    			}
            }
			return BoardPos.undefined;
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
        public static Vector2Int GetRandOpenDir(Board b, BoardPos originPos) {
            int[] randSides = MathUtils.GetShuffledIntArray(4);
            for (int i=0; i<randSides.Length; i++) {
                Vector2Int dir = MathUtils.GetDir(i);
                if (CanAddSpool(b, new BoardPos(originPos.col+dir.x, originPos.row+dir.y))) {
                    return dir;
                }
            }
            return Vector2Int.zero;
        }
        public static bool CanAddSpool(Board b, BoardPos pos) {
            BoardSpace space = GetSpace(b, pos);
            return space!=null && space.IsOpen();
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



        public static bool CanOccupantEverExit(Board b, Vector2Int pos, Vector2Int dir) {
            BoardSpace spaceFrom = GetSpace(b, pos);
            BoardSpace spaceTo   = GetSpace(b, pos+dir);
            if (spaceFrom==null || spaceTo==null) { return false; }
            if (!spaceFrom.CanOccupantEverExit(MathUtils.GetSide(dir))) { return false; }
            if (!spaceTo.CanOccupantEverEnterMe(MathUtils.GetOppositeSide(dir))) { return false; }
            return true;
        }



	}
}
