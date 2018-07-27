using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	[System.Serializable]
	public class Board {
		// Properties
		private int numCols,numRows;
		// Reference Lists
		public List<Tile> tilesAddedThisMove;
		// Objects
		public BoardSpace[,] spaces;
		public List<Tile> tiles;

		// Getters
		public int NumCols { get { return numCols; } }
		public int NumRows { get { return numRows; } }

		public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
		public BoardSpace[,] Spaces { get { return spaces; } }
		public Tile GetTile(Vector2Int pos) { return BoardUtils.GetOccupant(this, pos.x,pos.y) as Tile; }

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

		private int RandomTileColorID() {
			return Random.Range(0, 4);
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
			tilesAddedThisMove = new List<Tile>();

			tiles = new List<Tile>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
			// Add Props to the lists!
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
			tilesAddedThisMove.Add(prop);
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
				AddTile(randPos, colorID);
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
		}
		private void AddTileInDirAttempt(Tile tile, Vector2Int dir) {
			if (CanExtrudeTileInDir(tile, dir)) {
				BoardPos newTilePos = new BoardPos(tile.Col+dir.x,tile.Row+dir.y);
				AddTile(newTilePos, tile.ColorID);
			}
		}
		public void MatchCongruentTiles() {
			// TODO: This!
        }
        //void clearCongruentTiles() {
        //    calculateTileGroups();

        //    // Clear tiles in big groups!
        //    for (int i=0; i<tileGroups.length; i++) {
        //        if (tileGroups[i].length >= MIN_GROUP_SIZE) {
        //            clearTilesInGroup(tileGroups[i]);
        //        }
        //    }
        //}//*/




	}
}