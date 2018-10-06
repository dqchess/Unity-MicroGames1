using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	public class Board {
		// Properties
		private int numCols,numRows;
        private int numMovesMade;
		// Objects
		public BoardSpace[,] spaces;
        public List<Tile> tiles;
        // Reference Lists
		private List<List<Tile>> tileGroups; // for finding congruent groups of tiles
		public List<BoardObject> objectsAddedThisMove;

		// Getters
		public int NumCols { get { return numCols; } }
		public int NumRows { get { return numRows; } }

		public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
		public BoardSpace[,] Spaces { get { return spaces; } }
        public Tile GetTile(Vector2Int pos) { return GetTile(pos.x,pos.y); }
        public Tile GetTile(int col,int row) { return BoardUtils.GetOccupant(this, col,row) as Tile; }

		public Board Clone () {
			BoardData data = SerializeAsData();
			return new Board(data);
		}
		public BoardData SerializeAsData() {
			BoardData bd = new BoardData(numCols,numRows);
			foreach (Tile p in tiles) { bd.tileDatas.Add (p.SerializeAsData()); }
			for (int col=0; col<numCols; col++) {
				for (int row=0; row<numRows; row++) {
					bd.spaceDatas[col,row] = GetSpace(col,row).SerializeAsData();
				}
			}
			return bd;
		}



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public Board (BoardData bd) {
			numCols = bd.numCols;
			numRows = bd.numRows;
            numMovesMade = 0;

			// Add all gameplay objects!
			MakeEmptyPropLists ();
			MakeBoardSpaces (bd);
			AddPropsFromBoardData (bd);
			// TEMP TEST
			Debug_AddRandomTiles(8);

			// Start our groups out, goro!
			CalculateTileGroups();
		}

		private void MakeBoardSpaces (BoardData bd) {
			spaces = new BoardSpace[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaces[i,j] = new BoardSpace (this, bd.spaceDatas[i,j]);
				}
			}
		}
		private void MakeEmptyPropLists() {
			tiles = new List<Tile>();
			objectsAddedThisMove = new List<BoardObject>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
			// Add Props to the lists!
			foreach (TileData data in bd.tileDatas) { AddTile (data); }
		}
		private void Debug_AddRandomTiles(int numToAdd) {
			const int numColors = 4;
			for (int i=0; i<numToAdd; i++) {
				BoardPos randPos = BoardUtils.GetRandOpenPos(this);
				if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
				int colorID = Random.Range(0, numColors);
				AddTile(randPos, colorID);
			}
		}


		// ----------------------------------------------------------------
		//  Adding / Removing
		// ----------------------------------------------------------------
		private Tile AddTile(BoardPos pos, int colorID) {
            return AddTile(new TileData(pos, colorID));
		}
		private Tile AddTile (TileData data) {
			Tile prop = new Tile (this, data);
			tiles.Add (prop);
			return prop;
		}

		public void OnObjectRemovedFromPlay (BoardObject bo) {
			// Remove it from its lists!
			//		if (bo is BoardOccupant) { // Is it an Occupant? Remove it from allOccupants list!
			//			allOccupants.Remove (bo as BoardOccupant);
			//		}
			if (bo is Tile) { tiles.Remove (bo as Tile); }
			else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType().ToString() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
		}



        // ----------------------------------------------------------------
        //  Tile Group-Finding
        // ----------------------------------------------------------------
        private void CalculateTileGroups() {
            // FIRST, tell all Tiles they're not used in the search algorithm
            for (int i=0; i<tiles.Count; i++) {
                tiles[i].GroupID = -1;
            }
            tileGroups = new List<List<Tile>>();
			for (int i=tiles.Count-1; i>=0; --i) {
				if (tiles[i].GroupID != -1) { continue; }
                tileGroups.Add(new List<Tile>());
                RecursiveTileFinding(tiles[i].Col,tiles[i].Row, tiles[i].ColorID);
            }
        }
        private void RecursiveTileFinding(int col,int row, int colorID) {
            Tile tileHere = GetTile(col,row);
            if (tileHere==null || tileHere.GroupID!=-1) { return; } // No unused tile here? Stop.
            if (tileHere.ColorID != colorID) { return; } // Not a match? Stop.
			// Add it to the group!
			int groupID = tileGroups.Count-1;
			tileHere.GroupID = groupID;
			tileGroups[groupID].Add(tileHere);
			// Keep lookin'!
            RecursiveTileFinding(col-1,row, colorID);
            RecursiveTileFinding(col,row-1, colorID);
            RecursiveTileFinding(col+1,row, colorID);
            RecursiveTileFinding(col,row+1, colorID);
        }


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		/** Moves requested Tile, and the Occupants it'll also push.
			Returns TRUE if we made a successful, legal move, and false if we couldn't move anything. */
		public MoveResults ExecuteMove (BoardPos boToMovePos, Vector2Int dir) {
			// Clear out the Objects-added list just before the move.
			objectsAddedThisMove.Clear ();

			BoardOccupant boToMove = BoardUtils.GetOccupant(this, boToMovePos);
			MoveResults result = BoardUtils.MoveOccupant (this, boToMove, dir);
			OnMoveComplete ();
			return result;
		}
		private void OnMoveComplete () {
//			areGoalsSatisfied = CheckAreGoalsSatisfied ();
		}

		public void MoveGroupAttempt(int groupID, Vector2Int dir) {
			if (CanMoveGroup(groupID, dir)) {
				MoveGroup(groupID, dir);
			}
		}
		private void MoveGroup(int groupID, Vector2Int dir) {
			// VERTICAL
			if (dir.y != 0) {
				int startingRow = dir.y<0 ? 0 : NumRows-1;
				for (int col=0; col<NumCols; col++) {
					for (int row=startingRow; row>=0 && row<NumRows; row-=dir.y) {
						Tile tileHere = GetTile(col,row);
						if (tileHere!=null && tileHere.GroupID==groupID) {
							MoveTile(tileHere, dir);
						}
					}
				}
			}
			// HORIZONTAL
			else {
				int startingCol = dir.x<0 ? 0 : NumCols-1;
				for (int row=0; row<NumRows; row++) {
					for (int col=startingCol; col>=0 && col<NumCols; col-=dir.x) {
						Tile tileHere = GetTile(col,row);
						if (tileHere!=null && tileHere.GroupID==groupID) {
							MoveTile(tileHere, dir);
						}
					}
				}
			}
		}
		private void MoveTile(Tile tile, Vector2Int dir) {
			BoardSpace space = tile.MySpace;
			tile.RemoveMyFootprint();
			tile.SetColRow(tile.BoardPos.col+dir.x, tile.BoardPos.row+dir.y);
			tile.AddMyFootprint();
		}

		private bool CanMoveGroup(int groupID, Vector2Int dir) {
			if (groupID == -1) { return false; } // No group? No move.

			// TODO: Make boardData, execute move, and see if it works
			return true;
		}




	}
}