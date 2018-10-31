using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
	[System.Serializable]
	public class Board {
        // Constants
        private const int MaxColorID = 9; // TODO: Set this per level, yo!
		// Properties
        public bool DoTilesTow { get; private set; } // if FALSE, then we just have basic 16-sliding puzzle mechanic. If TRUE, we have our special towing mechanic!
        public bool AreGoalsSatisfied { get; private set; }
        public int NumCols { get; private set; }
        public int NumRows { get; private set; }
        private int randGroupSize;
        public int ParMoves { get; private set; }
        public int NumFootprintsDown { get; set; } // For in-progress moves. We don't wanna do groupfinding/islandtugging unless all footprints are down.
        private int[] numGroupsOfColorID; // index is colorID. Value is how many groups there are of this colorID.
        // Objects
        public BoardSpace[,] spaces;
        public List<Tile> tiles;
        // Reference Lists
        public List<BoardObject> objectsAddedThisMove;
        private List<List<Tile>> tileGroups;

		// Getters (Private)
		private bool GetAreGoalsSatisfied() {
            // If ANY colorIDs have more than 1 group, return false.
            for (int i=0; i<numGroupsOfColorID.Length; i++) {
                if (numGroupsOfColorID[i] > 1) { return false; }
            }
            return true;
		}

        // Getters (Public)
        public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
		public BoardSpace[,] Spaces { get { return spaces; } }
        public Tile GetTile(BoardPos pos) { return GetTile(pos.col,pos.row); }
        public Tile GetTile(Vector2Int pos) { return GetTile(pos.x,pos.y); }
        public Tile GetTile(int col,int row) { return BoardUtils.GetOccupant(this, col,row) as Tile; }

		public Board Clone () {
			BoardData data = SerializeAsData();
			return new Board(data);
		}
		public BoardData SerializeAsData() {
			BoardData bd = new BoardData(NumCols,NumRows);
            bd.parMoves = ParMoves;
            bd.doTilesTow = DoTilesTow;
            bd.randGroupSize = randGroupSize;
			foreach (Tile p in tiles) { bd.tileDatas.Add (p.SerializeAsData()); }
			for (int col=0; col<NumCols; col++) {
				for (int row=0; row<NumRows; row++) {
					bd.spaceDatas[col,row] = GetSpace(col,row).SerializeAsData();
				}
			}
			return bd;
		}



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public Board (BoardData bd) {
			NumCols = bd.numCols;
			NumRows = bd.numRows;
            ParMoves = bd.parMoves;
            DoTilesTow = bd.doTilesTow;
            randGroupSize = bd.randGroupSize;
            NumFootprintsDown = 0;

			// Add all gameplay objects!
			MakeEmptyPropLists ();
			MakeBoardSpaces (bd);
			AddPropsFromBoardData (bd);
            OnMoveComplete();
		}

		private void MakeBoardSpaces (BoardData bd) {
			spaces = new BoardSpace[NumCols,NumRows];
			for (int i=0; i<NumCols; i++) {
				for (int j=0; j<NumRows; j++) {
					spaces[i,j] = new BoardSpace (this, bd.spaceDatas[i,j]);
				}
			}
		}
		private void MakeEmptyPropLists() {
			tiles = new List<Tile>();
			objectsAddedThisMove = new List<BoardObject>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
			foreach (TileData data in bd.tileDatas) { AddTile (data); }
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
			if (bo is Tile) { tiles.Remove(bo as Tile); }
			else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType().ToString() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		/** Moves requested Tile, and the Occupants it'll also push.
			Returns TRUE if we made a successful, legal move, and false if we couldn't move anything. */
		public MoveResults ExecuteMove(BoardPos boToMovePos, Vector2Int dir) {
			// Clear out the Objects-added list just before the move.
			objectsAddedThisMove.Clear ();

            BoardOccupant boToMove = BoardUtils.GetOccupant(this, boToMovePos);
            MoveResults result = BoardUtils.ExecuteMove(this, boToMove, dir);
            
            // ONLY if this move was a success, do the OnMoveComplete paperwork!
            if (result == MoveResults.Success) {
			    OnMoveComplete ();
            }
			return result;
		}
		private void OnMoveComplete () {
            CalculateTileGroups();
			AreGoalsSatisfied = GetAreGoalsSatisfied();
		}
        
        // ----------------------------------------------------------------
        //  Tile Group-Finding
        // ----------------------------------------------------------------
        private void CalculateTileGroups() {
            // Reset all tiles' WasUsedInSearchAlgorithm.
            for (int i=0; i<tiles.Count; i++) {
                tiles[i].WasUsedInSearchAlgorithm = false;
            }
            // Find da groups!
            tileGroups = new List<List<Tile>>();
            for (int i=tiles.Count-1; i>=0; --i) {
                if (!tiles[i].WasUsedInSearchAlgorithm) {
                    tileGroups.Add(new List<Tile>());
                    RecursiveTileFinding(tiles[i].Col,tiles[i].Row, tiles[i].ColorID);
                }
            }
            // Update how many of each group there are!
            numGroupsOfColorID = new int[MaxColorID];
            for (int i=0; i<tileGroups.Count; i++) {
                int colorID = tileGroups[i][0].ColorID; // use the first Tile's colorID (they're all the same).
                if (colorID == -1) { continue; } // Safety check for undefined Tiles.
                numGroupsOfColorID[colorID] ++;
            }
            // Finally, tell the Tiles what's up!
            for (int i=0; i<tiles.Count; i++) {
                int colorID = tiles[i].ColorID;
                if (colorID == -1) { continue; } // Safety check for undefined Tiles.
                int numGroups = numGroupsOfColorID[colorID];
                tiles[i].UpdateIsInOnlyGroup(numGroups);
            }
        }
        private void RecursiveTileFinding(int col,int row, int colorID) {
            Tile tileHere = GetTile(col,row);
            if (tileHere==null || tileHere.WasUsedInSearchAlgorithm) { return; } // No unused tile here? Stop.
            if (tileHere.ColorID != colorID) { return; } // Not a match? Stop.
            tileHere.WasUsedInSearchAlgorithm = true;
            tileGroups[tileGroups.Count-1].Add(tileHere);
            if (col>0) { RecursiveTileFinding(col-1,row, colorID); }
            if (row>0) { RecursiveTileFinding(col,row-1, colorID); }
            if (col<NumCols-1) { RecursiveTileFinding(col+1,row, colorID); }
            if (row<NumRows-1) { RecursiveTileFinding(col,row+1, colorID); }
        }


        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        /** In xml layout, use "x" to add an undefined colorID Tile. This function will assign them colors randomly. */
        public void Debug_RandomizeUndefinedTileColorIDs() {
            // Prep a list of all the colorIDs we're gonna assign the undefined Tiles, and shuffle it!.
            List<int> colorIDs = new List<int>();
            for (int i=0; i<tiles.Count; i++) {
                if (tiles[i].ColorID==-1) {
                    int colorID = Mathf.FloorToInt(colorIDs.Count/(float)randGroupSize);
                    colorIDs.Add(colorID);
                }
            }
            colorIDs = MathUtils.GetShuffledIntArray(colorIDs);
            
            int numAssigned = 0; // this is incremented.
            for (int i=0; i<tiles.Count; i++) {
                if (tiles[i].ColorID == -1) { // This one's undefined! Let's assign it a color.
                    tiles[i].Debug_SetColorID(colorIDs[numAssigned++]);
                }
            }
            OnMoveComplete();
        }
        public void Debug_PrintLayout(bool isCompact) {
            string boardString = Debug_GetLayout(isCompact);
            Debug.Log (boardString);
        }
        public void Debug_CopyLayoutToClipboard() {
            string str = "";//"\n";
            str += Debug_GetLayout(true);
            GameUtils.CopyToClipboard(str);
        }
        public string Debug_GetLayout(bool isCompact) {
            string str = "";
            for (int row=0; row<NumRows; row++) {
                str += isCompact ? " " : "        "; // put it on my tab!
                for (int col=0; col<NumCols; col++) {
                    BoardSpace space = GetSpace(col,row);
                    Tile tile = GetTile(col,row);
                    if (tile!=null) { str += tile.ColorID.ToString(); }
                    else if (!space.IsPlayable) { str += "#"; }
                    else { str += "."; }
                }
                str += ",";
                if (!isCompact && row<NumRows-1) { str += "\n"; }
            }
            return str;
        }



	}
    
}

/*

        //public void Debug_AddTilesIfNone(GameController gameController) {
        //    if (tiles.Count > 0) { return; } // Nah, we've got some.
        //    int numToAdd = Mathf.FloorToInt(NumCols*NumRows * gameController.PercentTiles);
        //    int numColors = gameController.NumColors;
        //    Debug_AddRandomTiles(numToAdd);
        //    OnMoveComplete();
        //}
        //private void Debug_AddRandomTiles(int numToAdd) {
        //  for (int i=0; i<numToAdd; i++) {
        //      BoardPos randPos = BoardUtils.GetRandOpenPos(this, 2);
  //              if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
  //              int colorID = Mathf.FloorToInt(i/(float)randGroupSize);
        //      AddTile(randPos, colorID);
        //  }
        //}
*/