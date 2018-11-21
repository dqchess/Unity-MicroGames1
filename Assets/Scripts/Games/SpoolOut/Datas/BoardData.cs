using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    [System.Serializable] // NOTE: Not currently serialized anywhere! (So this [and 1D spaceDatas] is here just in case we DO wanna serialize.)
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
        public BoardSpaceData[] spaceDatas; // NOTE: Unity Json serialization doesn't support 2D arrays. :P
        public List<SpoolData> spoolDatas;
        public List<WallData> wallDatas;

        // Getters (Public)
        public BoardSpaceData GetSpaceData (int col,int row) { return spaceDatas[Space1DIndex(col,row)]; }
        public void SetSpaceData(int col,int row, BoardSpaceData bsd) {
            spaceDatas[Space1DIndex(col,row)] = bsd;
        }
        // Getters (Private)
        private int Space1DIndex(int col,int row) { return MathUtils.GridIndex2Dto1D(col,row, numCols); }

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
    						case '#': GetSpaceData(col,row).isPlayable = false; break;
                            case '_': AddWallData(col,row+1, Sides.T); break; // note: because the underscore looks lower, consider it in the next row (so layout text file looks more intuitive).
                            case '|': AddWallData(col,row,   Sides.L); break;
                            case '0': AddSpoolData(col,row, 1+0); break; // Note: We don't want to make Spools with numSpacesToFill as 0.
    						case '1': AddSpoolData(col,row, 1+1); break; // hacky 1+! So the XML matches the visuals.
    						case '2': AddSpoolData(col,row, 1+2); break;
    						case '3': AddSpoolData(col,row, 1+3); break;
                            case '4': AddSpoolData(col,row, 1+4); break;
                            case '5': AddSpoolData(col,row, 1+5); break;
                            case '6': AddSpoolData(col,row, 1+6); break;
                            case '7': AddSpoolData(col,row, 1+7); break;
                            case '8': AddSpoolData(col,row, 1+8); break;
                            case '9': AddSpoolData(col,row, 1+9); break;
                            case 'A': AddSpoolData(col,row, 1+10); break;
                            case 'B': AddSpoolData(col,row, 1+11); break;
                            case 'C': AddSpoolData(col,row, 1+12); break;
                            case 'D': AddSpoolData(col,row, 1+13); break;
                            case 'E': AddSpoolData(col,row, 1+14); break;
                            case 'F': AddSpoolData(col,row, 1+15); break;
    					}
    				}
    			}
            }
		}

		private void MakeEmptyLists () {
			// Spaces
			spaceDatas = new BoardSpaceData[numCols*numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					SetSpaceData(i,j, new BoardSpaceData (i,j));
				}
			}
			// Props
			spoolDatas = new List<SpoolData>();
            wallDatas = new List<WallData>();
		}



        void AddSpoolData(int col,int row, int numSpacesToFill) {
			int colorID = spoolDatas.Count; // colorID just matches which Spool this is.
			SpoolData newData = new SpoolData(new BoardPos(col,row), colorID, numSpacesToFill, null);
            spoolDatas.Add(newData);
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
            // Make empty allChars grid string thing. (E.g. If a space has a spool AND a wall, that string'll be like "0|". We pick it apart later.)
            string[,] allChars = new string[numCols,numRows];
            for (int i=0; i<numCols; i++) { for (int j=0; j<numRows; j++) { allChars[i,j]=""; } }
            // Populate!
            foreach (SpoolData s in spoolDatas) {
                allChars[s.boardPos.col,s.boardPos.row] += (s.numSpacesToFill-1).ToString(); // hacky -1! So the visuals match the XML.
                //foreach (Vector2Int space in s.pathSpaces) {
                //}
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
                            BoardSpaceData space = GetSpaceData(col,row);
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