using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardData {
        // Constants
        private readonly char[] LINE_BREAKS_CHARS = new char[] { ',' }; // our board layouts are comma-separated (because XML's don't encode line breaks).
		// Properties
        public bool debug_noWin; // true for many test levels, where we don't want any move to trigger a win.
        public int devRating;
        public int difficulty;
        public int numCols,numRows;
        public string description; // developer description.
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
            debug_noWin = bdxml.debug_noWin;
            difficulty = bdxml.difficulty;
            description = bdxml.desc;
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



        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        public void Debug_CopyLayoutToClipboard(bool isCompact) {
            GameUtils.CopyToClipboard(Debug_GetLayout(isCompact));
        }
        public void Debug_CopyXMLToClipboard(bool isCompact) {
            GameUtils.CopyToClipboard(Debug_GetAsXML(isCompact));
        }
        public string Debug_GetAsXML(bool isCompact) {
            string layoutStr = Debug_GetLayout(isCompact);
            string str = "    <Level ";
            str += "diff=\"" + difficulty +"\" ";
            str += "layout=\"" + layoutStr + "\" ";
            if (!string.IsNullOrEmpty(fueID)) { str += "fueID=\"" + fueID + "\" "; }
            if (devRating!=0) { str += "r=\"" + devRating + "\" "; }
            if (!string.IsNullOrEmpty(description)) { str += "desc=\"" + description + "\" "; }
            str += "/>\n";
            return str;
        }
        public string Debug_GetLayout(bool isCompact) {
            // Make empty allChars grid string thing. (E.g. If a space has a tile AND a wall, that string'll be like "0|". We pick it apart later.)
            string[,] allChars = new string[numCols,numRows];
            for (int i=0; i<numCols; i++) { for (int j=0; j<numRows; j++) { allChars[i,j]=""; } }
            // Populate!
            foreach (TileData t in tileDatas) {
                foreach (Vector2Int fpLocal in t.footprintLocal) {
                    Vector2Int fpGlobal = fpLocal + t.boardPos.ToVector2Int();
                    allChars[fpGlobal.x,fpGlobal.y] += t.colorID.ToString();
                }
            }
            foreach (WallData w in wallDatas) {
                int c = w.boardPos.col;// + (w.IsVertical ? 1 : 0);
                int r = w.boardPos.row + (w.IsVertical() ? 0 : -1);
                allChars[c,r] += w.IsVertical() ? "|" : "_";
            }
            // How many layers is that?
            int numLayers = 0;
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    numLayers = Mathf.Max(numLayers, allChars[i,j].Length);
                }
            }

            // Now combine all this into ONE big ol' string!
            string tab = isCompact ? "" : "        ";
            string lb = isCompact ? " " : "\n";
            string str = "" + lb;
            for (int layer=0; layer<numLayers; layer++) {
                for (int row=0; row<numRows; row++) {
                    str += tab;
                    for (int col=0; col<numCols; col++) {
                        string spaceStr = allChars[col,row];
                        // There IS a thing here!
                        if (layer < spaceStr.Length) {
                            str += spaceStr[layer];
                        }
                        // There is NOT a thing here. Use the info about the BoardSpace then.
                        else {
                            BoardSpaceData space = spaceDatas[col,row];
                            if (!space.isPlayable) { str += "#"; }
                            else { str += "."; }
                        }
                    }
                    str += ",";
                    if (row < numRows-1) { str += lb; }
                }
                if (layer < numLayers-1) { str += lb+tab+","+lb; }
            }

            return str;
        }


	}
}