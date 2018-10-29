using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardData {
        // Constants
        private readonly char[] LINE_BREAKS_CHARS = new char[] { ',' }; // our board layouts are comma-separated (because XML's don't encode line breaks).
		// Properties
        public int devRating;
        public int difficulty;
        public int numCols,numRows;
        public string fueID; // which tutorial this is gonna be!
		// BoardObjects
		public BoardSpaceData[,] spaceDatas;
        public List<TileData> tileDatas;
        public List<WallData> wallDatas;

		// Getters (Private)
		private BoardSpaceData GetSpaceData (int col,int row) { return spaceDatas[col,row]; }

        private string[] GetLevelStringArrayFromLayoutString (string layout) {
            List<string> stringList = new List<string>(layout.Split (LINE_BREAKS_CHARS, System.StringSplitOptions.None));
            // Remove the last element, which will be just empty space (because of how we format the layout in the XML).
            stringList.RemoveAt (stringList.Count-1);
            // Cut the excess white space.
            for (int i=0; i<stringList.Count; i++) {
                stringList[i] = TextUtils.RemoveWhitespace (stringList[i]);
            }
            string[] returnArray = stringList.ToArray();
            if (returnArray.Length == 0) { returnArray = new string[]{"."}; } // Safety catch.
            return returnArray;
        }

		public BoardData Clone() {
			Board b = new Board(this);
			return b.SerializeAsData();
		}


		/** Initializes a totally empty BoardData. */
		public BoardData (int _numCols,int _numRows) {
			numCols = _numCols;
			numRows = _numRows;
			MakeEmptyLists ();
		}
		public BoardData(BoardDataXML bdxml) {
            difficulty = bdxml.difficulty;
            devRating = bdxml.devRating;
            fueID = bdxml.fueID;
            string[] layoutArray = GetLevelStringArrayFromLayoutString(bdxml.layout);

            int numLayoutLayers = 1; // will increment for every "", we find.
            for (int i=0; i<layoutArray.Length; i++) {
                if (layoutArray[i].Length == 0) { // We found a break that denotes another layer of layout!
                    numLayoutLayers ++;
                }
            }

            // Set numCols and numRows!
            numCols = layoutArray[0].Length;
            numRows = (int)((layoutArray.Length-numLayoutLayers+1)/numLayoutLayers);

			// Make boardSpaceDatas, and prep empty lists!
			MakeEmptyLists();

			// Add all gameplay objects!
            for (int layer=0; layer<numLayoutLayers; layer++) {
    			for (int col=0; col<numCols; col++) {
    				for (int row=0; row<numRows; row++) {
                        
                        int stringArrayIndex = row + layer*(numRows+1);
                        if (stringArrayIndex>=layoutArray.Length || col>=layoutArray[stringArrayIndex].Length) {
                            Debug.LogError ("Whoops! Mismatch in layout in a board layout XML. stringArrayIndex: " + stringArrayIndex + ", col: " + col);
                            continue;
                        }
                        //int actualRow = row;//numRows-1-row; // note: flip y coordinates
                        //if (actualRow>=layoutArray.Length || col>=layoutArray[actualRow].Length) { // Safety check.
                        //    Debug.LogError("Whoa! Board layout has an issue."); continue;
                        //}
                        char c = layoutArray[stringArrayIndex][col];
    					switch (c) {
    						case '#': spaceDatas[col,row].isPlayable = false; break;
                            case '_': AddWallData (col,row+1, Sides.T); break; // note: because the underscore looks lower, consider it in the next row (so layout text file looks more intuitive).
                            case '|': AddWallData (col,row, Sides.L); break;
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
            wallDatas = new List<WallData>();
		}



        void AddTileData(int col,int row, int colorID) {
            TileData newData = new TileData(new BoardPos(col,row), colorID);
            tileDatas.Add(newData);
        }
        void AddWallData(int col,int row, int sideFacing) {
            WallData newData = new WallData(new BoardPos(col,row, sideFacing));
            wallDatas.Add(newData);
        }




	}
}