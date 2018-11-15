using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	[System.Serializable]
	public class Board {
        // Properties
        public bool Debug_noWin { get; private set; }
        public int DevRating { get; private set; }
        public int Difficulty { get; private set; }
        public int NumCols { get; private set; }
        public int NumRows { get; private set; }
        private string Description;
        public string FUEID { get; private set; }
        // Properties (Variable)
        public bool AreGoalsSatisfied { get; private set; }
        public bool DidAnyTilesMergeLastMove { get; private set; }
        public bool DidRandGen { get; private set; } // TRUE when any Tiles are added randomly.
        public bool IsInKnownFailState { get; private set; }
        private int numTilesToWin; // set when we're made. Goal: One of each colored Tile!
		private List<MergeSpot> lastMergeSpots; // remade when we call MergeAllTiles.
		// Objects
		public BoardSpace[,] spaces;
        public List<Tile> tiles;
        public List<Wall> walls;
        // Reference Lists
		public List<BoardObject> objectsAddedThisMove;

		// Getters (Private)
		private bool GetAreGoalsSatisfied() {
            if (Debug_noWin) { return false; } // debug_noWin? Test level. We're never satisfied.
			return tiles.Count <= numTilesToWin;
		}

        // Getters (Public)
        public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
		public BoardSpace[,] Spaces { get { return spaces; } }
        public Tile GetTile(BoardPos pos) { return GetTile(pos.col,pos.row); }
        public Tile GetTile(Vector2Int pos) { return GetTile(pos.x,pos.y); }
        public Tile GetTile(int col,int row) { return BoardUtils.GetOccupant(this, col,row) as Tile; }
        public List<MergeSpot> LastMergeSpots { get { return lastMergeSpots; } }
		public int GetNumTiles(int colorID) {
			int total = 0;
			foreach (Tile t in tiles) { if (t.ColorID==colorID) { total ++; } }
			return total;
		}
        public bool AreAnyTileColorsSatisfied() {
            int[] numTilesOfColor = new int[9]; // we won't have more than 9 colorIDs.
            for (int i=0; i<numTilesOfColor.Length; i++) { numTilesOfColor[i] = 0; }
            foreach (Tile t in tiles) {
                if (t.ColorID<0 || t.ColorID>=numTilesOfColor.Length) { Debug.LogError("ColorID outta bounds."); continue; } // Safety check.
                numTilesOfColor[t.ColorID] ++;
            }
            for (int i=0; i<numTilesOfColor.Length; i++) {
                if (numTilesOfColor[i] == 1) { return true; }
            }
            return false;
        }
        /** Returns how many unique colors are in the board. */
        public int NumColors() {
            int numColors = 0; // will add to.
            bool[] isColor = new bool[9];
            for (int i=0; i<tiles.Count; i++) {
                int colorID = tiles[i].ColorID;
                if (!isColor[colorID]) { 
                    isColor[colorID] = true;
                    numColors ++;
                }
            }
            return numColors;
        }

		public Board Clone() {
			BoardData data = SerializeAsData();
			return new Board(data);
		}
		public BoardData SerializeAsData() {
			BoardData bd = new BoardData(NumCols,NumRows);
            bd.debug_noWin = Debug_noWin;
            bd.description = Description;
            bd.devRating = DevRating;
            bd.difficulty = Difficulty;
            bd.fueID = FUEID;
            foreach (Tile p in tiles) { bd.tileDatas.Add (p.SerializeAsData()); }
            foreach (Wall p in walls) { bd.wallDatas.Add (p.SerializeAsData()); }
			for (int col=0; col<NumCols; col++) {
				for (int row=0; row<NumRows; row++) {
                    BoardSpaceData bsd = GetSpace(col,row).SerializeAsData();
					bd.SetSpaceData(col,row, bsd);
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
            Debug_noWin = bd.debug_noWin;
            Description = bd.description;
            Difficulty = bd.difficulty;
            DevRating = bd.devRating;
            FUEID = bd.fueID;
            DidRandGen = false; // We'll say otherwise eventually.
            DidAnyTilesMergeLastMove = false;

			// Add all gameplay objects!
			MakeEmptyPropLists ();
			MakeBoardSpaces (bd);
			AddPropsFromBoardData (bd);

			// Start our solo bubbas out merged, goro!
			OnMoveComplete();

			CalculateNumTilesToWin();
		}

		private void MakeBoardSpaces (BoardData bd) {
			spaces = new BoardSpace[NumCols,NumRows];
			for (int i=0; i<NumCols; i++) {
				for (int j=0; j<NumRows; j++) {
					spaces[i,j] = new BoardSpace(this, bd.GetSpaceData(i,j));
				}
			}
		}
		private void MakeEmptyPropLists() {
			tiles = new List<Tile>();
            walls = new List<Wall>();
			objectsAddedThisMove = new List<BoardObject>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
            foreach (TileData data in bd.tileDatas) { AddTile (data); }
            foreach (WallData data in bd.wallDatas) { AddWall (data); }
		}
		private void CalculateNumTilesToWin() {
			numTilesToWin = 0; // We increment next.
			HashSet<int> colorIDsInBoard = new HashSet<int>();
			for (int i=0; i<tiles.Count; i++) {
				if (!colorIDsInBoard.Contains(tiles[i].ColorID)) {
					colorIDsInBoard.Add(tiles[i].ColorID);
					numTilesToWin ++;
				}
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
            objectsAddedThisMove.Add(prop);
            return prop;
        }
        private Wall AddWall(WallData data) {
            Wall prop = new Wall(this, data);
            walls.Add (prop);
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
        //  Tile Merging
        // ----------------------------------------------------------------
		private void MergeAdjacentTiles() {
			lastMergeSpots = new List<MergeSpot>();

			for (int i=tiles.Count-1; i>=0; --i) {
				if (i >= tiles.Count) { continue; } // Oh, if this tile was removed, skip it.
				Tile t = tiles[i];
                for (int j=0; j<t.FootprintGlobal.Count; j++) {
                    Vector2Int fpPos = t.FootprintGlobal[j];
                    MergeTilesAttempt(t, fpPos, Vector2Int.L);
                    MergeTilesAttempt(t, fpPos, Vector2Int.R);
                    MergeTilesAttempt(t, fpPos, Vector2Int.B);
                    MergeTilesAttempt(t, fpPos, Vector2Int.T);
                }
            }
		}

		private bool CanMergeTiles(Tile tileA, Tile tileB) {
			if (tileA==null || tileB==null) { return false; } // Check the obvious.
			if (tileA.ColorID != tileB.ColorID) { return false; } // Different colors? Nah.
			if (!tileA.IsInPlay || !tileB.IsInPlay) { return false; } // One's not in play anymore ('cause it was just merged)? Nah.
			if (tileA == tileB) { return false; } // Oh, we just merged and now it's looking at itself? Nah.
			return true; // Sure!
		}
        private void MergeTilesAttempt(Tile tileA, Vector2Int tileAFPPos, Vector2Int tileBDir) {
            if (!BoardUtils.CanOccupantEverExit(this, tileAFPPos, tileBDir)) { return; } // Can't travel between these spaces (there's a wall)? No merge.
            Tile tileB = GetTile(tileAFPPos+tileBDir);
            if (CanMergeTiles(tileA,tileB)) {
                MergeTiles(tileA,tileB);
            }
		}
		private void MergeTiles(Tile tileA, Tile tileB) {
            // For animations! If tileA just moved, swap the two.
            if (tileA.DidJustMove) {
                Tile tempTile = tileA;
                tileA = tileB;
                tileB = tempTile;
            }
			AddMergeSpots(tileA, tileB);
			//List<Vector2Int> tileBFootGlobal = new List<Vector2Int>(tileB.FootprintGlobal); // note: copy it so we don't modify the original.
			// Remove tileB from the board!
			tileB.RemoveFromPlay();
			// Append tileA's footprint, yo.
			tileA.AppendMyFootprint(tileB);
            // Yes, they did!
            DidAnyTilesMergeLastMove = true;
		}
		/** For each global footprint of tileA, looks around to see if it's mergin' with tileB. If so, we add a MergeLocation there. */
		private void AddMergeSpots(Tile tileA, Tile tileB) {
			for (int i=0; i<tileA.FootprintGlobal.Count; i++) {
				Vector2Int footA = tileA.FootprintGlobal[i];
				MaybeAddMergeLocation(tileA,tileB, footA, Vector2Int.B);
				MaybeAddMergeLocation(tileA,tileB, footA, Vector2Int.L);
				MaybeAddMergeLocation(tileA,tileB, footA, Vector2Int.R);
				MaybeAddMergeLocation(tileA,tileB, footA, Vector2Int.T);
			}
		}
		private void MaybeAddMergeLocation(Tile tileA,Tile tileB, Vector2Int footAPos, Vector2Int dir) {
			// Yes, tileB has a footprint in this dir!
			if (tileB.FootprintGlobal.Contains(footAPos+dir)) {
				lastMergeSpots.Add(new MergeSpot(tileA, footAPos,dir));
			}
		}
        
        private void UpdateTilesMergePoses() {
            for (int i=0; i<tiles.Count; i++) {
                tiles[i].RemakeMergePoses();
            }
        }
        
        
        // ----------------------------------------------------------------
        //  Tile Splitting
        // ----------------------------------------------------------------
        private void SeparateSplitTiles() {
            for (int i=tiles.Count-1; i>=0; --i) {
                MaybeSeparateSplitTile(tiles[i]);
            }
        }
        private void MaybeSeparateSplitTile(Tile tile) {
            tile.RemakeInnerClusters();
            if (tile.InnerClusters.Count > 1) {
                SeparateTile(tile);
            }
        }
        private void SeparateTile(Tile tile) {
            // What properties do we want?
            List<List<Vector2Int>> fpsGlobals = GetFootprintsGlobalsFromInnerClusters(tile);
            int colorID = tile.ColorID;
            // Destroy the original Tile.
            tile.RemoveFromPlay();
            // Add the new Tiles!
            for (int i=0; i<fpsGlobals.Count; i++) {
                Vector2Int bpV2 = fpsGlobals[i][0]; // make the BoardPos be the first footprint pos.
                List<Vector2Int> fpsLocal = OffsetFootprints(fpsGlobals[i], bpV2*-1);
                TileData tileData = new TileData(new BoardPos(bpV2), colorID, fpsLocal);
                AddTile(tileData);
            }
        }
        private List<List<Vector2Int>> GetFootprintsGlobalsFromInnerClusters(Tile tile) {
            List<List<Vector2Int>> ics = tile.InnerClusters;
            List<List<Vector2Int>> fpsGlobal = new List<List<Vector2Int>>();
            for (int i=0; i<ics.Count; i++) {
                fpsGlobal.Add(new List<Vector2Int>());
                for (int j=0; j<ics[i].Count; j++) {
                    fpsGlobal[i].Add(ics[i][j] + tile.BoardPos.ToVector2Int());
                }
            }
            return fpsGlobal;
        }
        private List<Vector2Int> OffsetFootprints(List<Vector2Int> fpsGlobal, Vector2Int offset) {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int i=0; i<fpsGlobal.Count; i++) {
                list.Add(fpsGlobal[i] + offset);
            }
            return list;
        }


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		/** Moves requested Tile, and the Occupants it'll also push.
			Returns TRUE if we made a successful, legal move, and false if we couldn't move anything. */
		public MoveResults ExecuteMove (BoardPos boToMovePos, Vector2Int dir) {
			// Clear out the Objects-added list just before the move.
			objectsAddedThisMove.Clear();
            foreach (Tile t in tiles) { t.DidJustMove = false; } // Reset DidJustMove for all!

			BoardOccupant boToMove = BoardUtils.GetOccupant(this, boToMovePos);
			MoveResults result = BoardUtils.MoveOccupant (this, boToMove, dir);
            // ONLY if this move was a success, do the OnMoveComplete paperwork!
            if (result == MoveResults.Success) {
			    OnMoveComplete ();
            }
			return result;
		}
		private void OnMoveComplete () {
            DidAnyTilesMergeLastMove = false; // will say otherwise next!
            MergeAdjacentTiles();
            UpdateTilesMergePoses();
            SeparateSplitTiles();
			AreGoalsSatisfied = GetAreGoalsSatisfied();
            // Update IsInKnownFailState!
            IsInKnownFailState = BoardUtils.IsInHardcodedFailState(this);
		}

		///// Weird, but MUCH easier to program: This is for the merging animation. If tileGrabbing is always at the start of the list, we can count on it always being taken out of play.
		//public void OnSetTileGrabbing(Tile _tile) {
		//	// If there's a tileGrabbing, move it to the beginning of the list (so that it'll be merged last).
		//	if (_tile != null) {
		//		tiles.Remove(_tile);
		//		tiles.Insert(0, _tile);
		//	}
		//}



		// ----------------------------------------------------------------
		//  Debug
		// ----------------------------------------------------------------
        public void Debug_SetDifficulty(int _difficulty) {
            Difficulty = _difficulty;
        }
		public void Debug_AddTilesIfNone(RandGenParams rgp) {
			if (tiles.Count > 0) { return; } // Nah, we've got some.
			int numToAdd = Mathf.FloorToInt(NumCols*NumRows * rgp.PercentTiles);
			int numColors = rgp.NumColors;
            int stickinessMin = rgp.StickinessMin;
            int stickinessMax = rgp.StickinessMax;
			//			if (tiles.Count == 0) { Debug_AddRandomTiles(Mathf.FloorToInt(NumCols*NumRows*Random.Range(0.5f,0.85f)), numColors); }
			Debug_AddRandomWalls(rgp.NumWalls);
			Debug_AddRandomTiles(numToAdd, numColors, stickinessMin,stickinessMax);
            // Yes, we did!
            DidRandGen = true;
			OnMoveComplete();
		}
		private void Debug_AddRandomWalls(int numToAdd) {
			int safetyCount=0;
			while (numToAdd > 0 && safetyCount++<99) {
				BoardPos randPos = BoardUtils.GetRandPosWithoutWall(this);
				if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
				AddWall(new WallData(randPos));
				numToAdd --;
				if (numToAdd <= 0) { break; }
			}
		}
		private void Debug_AddRandomTiles(int numToAdd, int numColors, int stickinessMin,int stickinessMax) {
            //for (int i=0; i<numToAdd; i++) {TEST TEMP
            //    BoardPos randPos = BoardUtils.GetRandOpenPos(this);
            //    if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
            //    int colorID = Random.Range(0, numColors);
            //    AddTile(randPos, colorID);
            //}
            int safetyCount=0;
            while (numToAdd > 0 && safetyCount++<99) {
                BoardPos randPos = BoardUtils.GetRandOpenPos(this);
                if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
                int colorID = Random.Range(0, numColors);
                int clusterAttemptSize = Random.Range(stickinessMin, stickinessMax+1);
                for (int j=0; j<clusterAttemptSize; j++) { // For every ONE tile, add more of the same color next to it!
                    if (!BoardUtils.CanAddTile(this,randPos)) { continue; }
                    AddTile(randPos, colorID);
                    numToAdd --;
                    if (numToAdd <= 0) { break; }
                    Vector2Int randDir = BoardUtils.GetRandOpenDir(this, randPos);
                    if (randDir == Vector2Int.zero) { continue; }
                    //Debug.Log("Randdir: " + randDir);
                    randPos.col += randDir.x;
                    randPos.row += randDir.y;
                }
            }
		}
        
        
        public void Debug_CopyLayoutToClipboard(bool isCompact) {
            SerializeAsData().Debug_CopyLayoutToClipboard(isCompact);
        }
        public void Debug_CopyXMLToClipboard(bool isCompact) {
            SerializeAsData().Debug_CopyXMLToClipboard(isCompact);
        }
        public string Debug_GetAsXML(bool isCompact) {
            return SerializeAsData().Debug_GetAsXML(isCompact);
        }
        public string Debug_GetLayout(bool isCompact) {
            return SerializeAsData().Debug_GetLayout(isCompact);
        }
        public void Debug_CopyXMLToClipboardWithDiff(int _diff) {
            Difficulty = _diff;
            Debug_CopyXMLToClipboard(true);
        }



	}
}

