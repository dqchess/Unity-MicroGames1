using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	[System.Serializable]
	public class Board {
        // Constants
        private const int MIN_GROUP_SIZE = 3;
		// Properties
		private int numCols,numRows;
        private int numMovesMade;
		// Objects
		public BoardSpace[,] spaces;
        public List<Tile> tiles;
        // Reference Lists
        public List<Tile> tilesRecentlyAdded; // so the view knows what TileViews to add.
        private List<List<Tile>> tileGroups; // for finding congruent groups of tiles

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

        public bool AreAllSpacesFilled() {
            for (int col=0; col<numCols; col++) {
                for (int row=0; row<numRows; row++) {
                    if (spaces[col,row].IsOpen()) { return false; }
                }
            }
            return true;
        }

		private int RandomTileColorID() {
            if (numMovesMade <= 3) { return Random.Range(0, 3); }
            else if (numMovesMade <= 8) { return Random.Range(0, 4); }
            else if (numMovesMade <= 18) { return Random.Range(0, 5); }
            //// Now, *usually* prefer one of 5 colors. (Aka red will appear as an infrequent obstacle.)
            //if (Random.Range(0f,1f) < 0.5f) {
            //    return Random.Range(0, 5);
            //}
            return Random.Range(0, 6);
		}
		public bool CanExtrudeTile(Tile tile) {
			return CanExtrudeTileInDir(tile, Vector2Int.T)
				|| CanExtrudeTileInDir(tile, Vector2Int.R)
				|| CanExtrudeTileInDir(tile, Vector2Int.B)
				|| CanExtrudeTileInDir(tile, Vector2Int.L);
		}
		private bool CanExtrudeTileInDir(Tile tile, Vector2Int dir) {
			BoardPos newTilePos = new BoardPos(tile.Col+dir.x,tile.Row+dir.y);
			return BoardUtils.IsSpaceOpen(this, newTilePos);
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
		}

		private void MakeBoardSpaces (BoardData bd) {
			spaces = new BoardSpace[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaces[i,j] = new BoardSpace (this, bd.spaceDatas[i,j]);
				}
			}
		}
		private void MakeEmptyPropLists () {
			tilesRecentlyAdded = new List<Tile>();

			tiles = new List<Tile>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
			// Add Props to the lists!
			foreach (TileData data in bd.tileDatas) { AddTile (data); }
		}


		// ----------------------------------------------------------------
		//  Adding / Removing
		// ----------------------------------------------------------------
		private Tile AddTile(BoardPos pos, int colorID, int value) {
            return AddTile(new TileData(pos, colorID, value));
		}
		private Tile AddTile (TileData data) {
			Tile prop = new Tile (this, data);
			tiles.Add (prop);
			tilesRecentlyAdded.Add(prop);
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
		//  Doers
		// ----------------------------------------------------------------
		public void AddRandomTiles(int numToAdd) {
			for (int i=0; i<numToAdd; i++) {
				BoardPos randPos = BoardUtils.GetRandOpenPos(this);
				if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
				int colorID = RandomTileColorID();
				AddTile(randPos, colorID, 1);
			}
		}
		public void ExtrudeTile(Tile tile) {
			// Try to add 4 Tiles around the source Tile!
			AddTileInDirAttempt(tile, Vector2Int.T);
			AddTileInDirAttempt(tile, Vector2Int.R);
			AddTileInDirAttempt(tile, Vector2Int.B);
			AddTileInDirAttempt(tile, Vector2Int.L);
			// Remove the source Tile.
			tile.RemoveFromPlay();
            // We made a move!
            numMovesMade ++;
		}
		private void AddTileInDirAttempt(Tile tile, Vector2Int dir) {
			if (CanExtrudeTileInDir(tile, dir)) {
				BoardPos newTilePos = new BoardPos(tile.Col+dir.x,tile.Row+dir.y);
                AddTile(newTilePos, tile.ColorID, tile.Value+1);
			}
        }
        public int ClearCongruentTiles() {
            CalculateTileGroups();
            int numTilesCleared = 0;
            for (int i=0; i<tileGroups.Count; i++) {
                if (tileGroups[i].Count >= MIN_GROUP_SIZE) {
                    ClearTilesInGroup(tileGroups[i]);
                    numTilesCleared += tileGroups[i].Count;
                }
            }
            return numTilesCleared;
        }


        // ----------------------------------------------------------------
        //  Tile Group-Finding
        // ----------------------------------------------------------------
        private void CalculateTileGroups() {
            // FIRST, tell all tiles they're not used in the search algorithm
            for (int i=0; i<tiles.Count; i++) {
                tiles[i].WasUsedInSearchAlgorithm = false;
            }
            tileGroups = new List<List<Tile>>();
            for (int i=tiles.Count-1; i>=0; --i) {
                tileGroups.Add(new List<Tile>());
                RecursiveTileFinding(tiles[i].Col,tiles[i].Row, tiles[i].ColorID);
            }
        }
        private void RecursiveTileFinding(int col,int row, int colorID) {
            Tile tileHere = GetTile(col,row);
            if (tileHere==null || tileHere.WasUsedInSearchAlgorithm) { return; } // No unused tile here? Stop.
            if (tileHere.ColorID != colorID) { return; } // Not a match? Stop.
            //if (tileGroups[tileGroups.Count-1].Count >= 2) return; // if this group is BIGGER THAN 2, then return. This is a shoehorn to change the mechanic without changing this algorithm.
            tileHere.WasUsedInSearchAlgorithm = true;
            tileGroups[tileGroups.Count-1].Add(tileHere);
            if (col>0) { RecursiveTileFinding(col-1,row, colorID); }
            if (row>0) { RecursiveTileFinding(col,row-1, colorID); }
            if (col<numCols-1) { RecursiveTileFinding(col+1,row, colorID); }
            if (row<numRows-1) { RecursiveTileFinding(col,row+1, colorID); }
        }
        private void ClearTilesInGroup(List<Tile> tilesInGroup) {
            for (int i=0; i<tilesInGroup.Count; i++) {
                tilesInGroup[i].RemoveFromPlay();
            }
        }




	}
}