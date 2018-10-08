using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
	public class BoardData {
		// Properties
		public int numCols,numRows;
		// BoardObjects
		public BoardSpaceData[,] spaceDatas;
		public List<TileData> tileDatas;

		// Getters (Private)
		private BoardSpaceData GetSpaceData (int col,int row) { return spaceDatas[col,row]; }


		/** Initializes a totally empty BoardData. */
		public BoardData (int _numCols,int _numRows) {
			numCols = _numCols;
			numRows = _numRows;
			MakeEmptyLists ();
		}
		public BoardData(string[] layoutArray) {
			// Set numCols and numRows!
			numCols = layoutArray[0].Length;
			numRows = layoutArray.Length;

			// Make boardSpaceDatas, and prep empty lists!
			MakeEmptyLists();

			// Add all gameplay objects!
			for (int col=0; col<numCols; col++) {
				for (int row=0; row<numRows; row++) {
					int actualRow = row;//numRows-1-row; // note: flip y coordinates
					if (actualRow>=layoutArray.Length || col>=layoutArray[actualRow].Length) { // Safety check.
						Debug.LogError("Whoa! Board layout has an issue."); continue;
					}
//					int spaceIndex = Space1DIndex(col,row);
					// PROPS
					char c = layoutArray[actualRow][col];
					switch (c) {
						case '#': spaceDatas[col,row].isPlayable = false; break;
						case '0': AddTileData(col,row, 0); break;
						case '1': AddTileData(col,row, 1); break;
						case '2': AddTileData(col,row, 2); break;
						case '3': AddTileData(col,row, 3); break;
						case '4': AddTileData(col,row, 4); break;
						case '5': AddTileData(col,row, 5); break;
						case '6': AddTileData(col,row, 6); break;
					}
				}
			}
		}

		private void MakeEmptyLists () {
			// Spaces
			spaceDatas = new BoardSpaceData[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaceDatas[i,j] = new BoardSpaceData (i,j);
				}
			}
			// Props
			tileDatas = new List<TileData>();
		}



		void AddTileData (int col,int row, int colorID) {
			TileData newData = new TileData (new BoardPos(col,row), colorID);
			tileDatas.Add (newData);
		}



	}
}