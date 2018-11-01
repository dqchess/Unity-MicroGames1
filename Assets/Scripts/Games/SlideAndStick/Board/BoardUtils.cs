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


        public static BoardSpace GetSpace(Board b, BoardPos pos) { return GetSpace(b, pos.col,pos.row); }
        public static BoardSpace GetSpace(Board b, Vector2Int pos) { return GetSpace(b, pos.x,pos.y); }
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
        //public static Wall GetWall(Board b, int col,int row) {
        //    BoardSpace space = GetSpace (b, col,row);
        //    if (space==null) { return null; }
        //    return space.WallOnMe;
        //}

		public static bool IsSpaceOpen(Board b, BoardPos pos) { return IsSpaceOpen(b, pos.col,pos.row); }
		public static bool IsSpaceOpen(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsOpen();
		}
		public static bool IsSpacePlayable(Board b, Vector2Int pos) { return IsSpacePlayable(b, pos.x,pos.y); }
		public static bool IsSpacePlayable(Board b, int col,int row) {
			BoardSpace bs = GetSpace (b, col,row);
			return bs!=null && bs.IsPlayable;
		}
		public static BoardOccupant GetOccupantInClonedBoard(BoardOccupant sourceBO, Board newBoard) {
			return GetOccupant(newBoard, sourceBO.Col,sourceBO.Row);
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
                if (CanAddTile(b, new BoardPos(originPos.col+dir.x, originPos.row+dir.y))) {
                    return dir;
                }
            }
            return Vector2Int.zero;
        }
        public static bool CanAddTile(Board b, BoardPos pos) {
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



		public static bool CanExecuteMove(Board originalBoard, BoardOccupant boToMove, Vector2Int dir) {
			if (boToMove == null) { return false; } // No BoardOccupant to move? Can't execute.
			// Clone the Board, execute the exact move, and return if it worked.
			Board testBoard = originalBoard.Clone();
			MoveResults moveResult = testBoard.ExecuteMove(boToMove.BoardPos, dir);
			return moveResult != MoveResults.Fail;
		}
		public static bool CanOccupantMoveInDir(Board originalBoard, BoardOccupant originalBO, Vector2Int moveDir) {
			Board testBoard = originalBoard.Clone();
			BoardOccupant testBO = GetOccupant(testBoard, originalBO.BoardPos.col,originalBO.BoardPos.row);
			MoveResults moveResult = MoveOccupant(testBoard, testBO, moveDir);
			return moveResult == MoveResults.Success;
		}

        private static bool AreSpacesPlayable(Board b, List<Vector2Int> footprintGlobal) {
            return AreSpacesPlayable(b, footprintGlobal, Vector2Int.zero);
        }
        private static bool AreSpacesPlayable(Board b, List<Vector2Int> footprintGlobal, Vector2Int posOffset) {
            for (int i=0; i<footprintGlobal.Count; i++) {
                if (!IsSpacePlayable(b, footprintGlobal[i]+posOffset)) { return false; }
            }
            return true;
        }
        private static bool CanPassThroughSpaces(Board b, List<Vector2Int> fpGlobal, Vector2Int dir, Vector2Int posOffset) {
            for (int i=0; i<fpGlobal.Count; i++) {
                if (!CanOccupantEverExit(b, fpGlobal[i]+posOffset, dir)) { return false; }
            }
            return true;
        }
        public static bool CanOccupantEverExit(Board b, Vector2Int pos, Vector2Int dir) {
            BoardSpace spaceFrom = GetSpace(b, pos);
            BoardSpace spaceTo   = GetSpace(b, pos+dir);
            if (spaceFrom==null || spaceTo==null) { return false; }
            if (!spaceFrom.CanOccupantEverExit(MathUtils.GetSide(dir))) { return false; }
            if (!spaceTo.CanOccupantEverEnterMe(MathUtils.GetOppositeSide(dir))) { return false; }
            return true;
        }

		public static MoveResults MoveOccupant (Board b, BoardOccupant bo, Vector2Int dir) {
			if (bo == null) {
				Debug.LogError ("Oops! We're trying to move a null Occupant! dir: " + dir.ToString());
				return MoveResults.Fail;
			}

			//// Is this outside the board? Oops! Return Fail.
			//if (!AreSpacesPlayable(b, bo.FootprintGlobal,dir)) {
			//	return MoveResults.Fail;
			//}

			// Always remove its footprint first. We're about to move it!
			bo.RemoveMyFootprint ();

			bo.SetColRow(bo.Col+dir.x, bo.Row+dir.y);

			// What Occupants shall we be displacing?
			for (int i=0; i<bo.FootprintGlobal.Count; i++) {
				Vector2Int footGlobal = bo.FootprintGlobal[i];
				BoardOccupant nextBO_touching = GetOccupant(b, footGlobal.x,footGlobal.y);
				if (nextBO_touching != null) {
					// PUSH the next guy! Interrupt this function with a recursive call! (And we'll add everyone back to the board at the end.)
					MoveResults nextResult = MoveOccupant(b, nextBO_touching, dir);
					// Whoa, if the recursive move we just made DIDN'T work, then stop here and return that info.
					if (nextResult != MoveResults.Success) {
						return nextResult;
					}
				}
			}

			// Add its footprint back now.
			bo.AddMyFootprint ();

            // Is this outside the board? Oops! Return Fail.
            if (!AreSpacesPlayable(b, bo.FootprintGlobal)) {
                return MoveResults.Fail;
            }
            // Did we pass through a Wall? Return Fail.
            if (!CanPassThroughSpaces(b, bo.FootprintGlobal, dir, MathUtils.GetOppositeDir(dir))) {
                return MoveResults.Fail;
            }

			// Success!
			return MoveResults.Success;
		}



		public static bool IsInHardcodedFailState(Board b) {
			if (b==null) { return false; }
            string fueID = b.FUEID;
			if (fueID == "introToUndo") {
				if (b.tiles.Count==3 && (!IsTileColor(b,0,0, 1) || !IsTileColor(b,2,0, 1))) {
					return true;
				}
			}
			else if (fueID == "failHelp0") {
				if (b.tiles.Count==3 && IsTileHorz(b.tiles[0]) && IsTileHorz(b.tiles[1]) && IsTileHorz(b.tiles[2])) {
					return true;
				}
			}
//			else if (levelIndex == 7) {
//				if (b.tiles.Count==4) { // 4 tiles?
//					if (IsTileVert(GetFirstTileOfColor(b,0))) { // blue is vertical?
//						Vector2Int[] shape = new Vector2Int[]{new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(1,0)};
//						if (IsTileShape(GetFirstTileOfColor(b,1), shape)) { // green is tromino?
//							return true;
//						}
//					}
//				}
//			}
            else if (fueID == "failHelp1") {
				if (b.tiles.Count==3) { // 3 tiles?
					if (IsTileTall(GetFirstTileOfColor(b,0), 3)) { // blue is at least 3 tall?
						return true;
					}
				}
			}
            else if (fueID == "failHelp2") {
				Tile topLeftTile = b.GetTile(0,0);
				if (b.tiles.Count==3 && topLeftTile!=null && topLeftTile.FootprintLocal.Count==1 && GetOccupant(b,0,1)!=null) {
					return true;
				}
			}
            else if (fueID == "failHelp3") { // NOTE: This one hardly captures most of the fail states for this lvl.
				Tile topLeftTile = b.GetTile(0,0);
				if (topLeftTile!=null && topLeftTile.FootprintLocal.Count==1) {
					if (IsTileColor(b,0,1, 3) && IsTileColor(b,1,0, 3) && IsTileColor(b,1,1, 3)) {
						return true;
					}
				}
			}
			// Nah, I have no opinions on this state.
			return false;
		}
		private static Tile GetFirstTileOfColor(Board b, int colorID) {
			for (int i=0; i<b.tiles.Count; i++) {
				if (b.tiles[i].ColorID == colorID) { return b.tiles[i]; }
			}
			return null;
		}
		private static bool IsTileColor(Board b, int col,int row, int colorID) {
			Tile t = b.GetTile(col,row);
			return t!=null && t.ColorID==colorID;
		}
		private static bool IsTileVert(Tile t) {
			if (t==null) { return false; }
			if (t.FootprintLocal.Count <= 1) { return false; } // Only 1 space? Nah.
			for (int i=0; i<t.FootprintLocal.Count; i++) {
				if (t.FootprintLocal[i].x != 0) { return false; }
			}
			return true;
		}
		private static bool IsTileHorz(Tile t) {
			if (t==null) { return false; }
			if (t.FootprintLocal.Count <= 1) { return false; } // Only 1 space? Nah.
			for (int i=0; i<t.FootprintLocal.Count; i++) {
				if (t.FootprintLocal[i].y != 0) { return false; }
			}
			return true;
		}
//		private static bool IsTileShape(Tile t, Vector2Int[] shape) { NOTE: Doesn't work yet... footprints can be offset from what we pass in. :P
//			if (t.FootprintLocal.Count != shape.Length) { return false; } // # footprints don't match? Nah.
//			for (int i=0; i<shape.Length; i++) {
//				if (!t.FootprintLocal.Contains(shape[i])) { return false; }
//			}
//			return true;
//		}
		private static bool IsTileTall(Tile t, int numTall) {
			if (t.FootprintLocal.Count < numTall) { return false; } // Quick check: Not even enough footprints? Nah.
			int yMin= 999;
			int yMax=-999;
			for (int i=0; i<t.FootprintLocal.Count; i++) {
				int y = t.FootprintLocal[i].y;
				yMin = Mathf.Min(y, yMin);
				yMax = Mathf.Max(y, yMax);
			}
			int tileHeight = (yMax - yMin) + 1;
			return tileHeight >= numTall;
		}


	}
}
