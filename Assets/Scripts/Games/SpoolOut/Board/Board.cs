using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
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
        public bool DidRandGen { get; private set; } // TRUE when any Spools are added randomly.
		// Objects
		public BoardSpace[,] spaces;
        public List<Spool> spools;
        public List<Wall> walls;
        // Reference Lists
		public List<BoardObject> objectsAddedThisMove;

		// Getters (Private)
		private bool GetAreGoalsSatisfied() {
            if (Debug_noWin) { return false; } // debug_noWin? Test level. We're never satisfied.
			// Ask each Spool.
            foreach (Spool w in spools) {
                if (!w.IsSatisfied) { return false; }
            }
            return true;
		}

        // Getters (Public)
        public BoardSpace GetSpace(int col,int row) { return BoardUtils.GetSpace(this, col,row); }
		public BoardSpace[,] Spaces { get { return spaces; } }
        public Spool GetSpool(BoardPos pos) { return GetSpool(pos.col,pos.row); }
        public Spool GetSpool(Vector2Int pos) { return GetSpool(pos.x,pos.y); }
        public Spool GetSpool(int col,int row) { return BoardUtils.GetSpool(this, col,row); }

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
            foreach (Spool p in spools) { bd.spoolDatas.Add (p.SerializeAsData()); }
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

			// Add all gameplay objects!
			MakeEmptyPropLists ();
			MakeBoardSpaces (bd);
			AddPropsFromBoardData (bd);

			// Start our solo bubbas out merged, goro!
			OnMoveComplete();
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
			spools = new List<Spool>();
            walls = new List<Wall>();
			objectsAddedThisMove = new List<BoardObject>();
		}
		private void AddPropsFromBoardData (BoardData bd) {
            foreach (SpoolData data in bd.spoolDatas) { AddSpool (data); }
            foreach (WallData data in bd.wallDatas) { AddWall (data); }
		}


		// ----------------------------------------------------------------
		//  Adding / Removing
		// ----------------------------------------------------------------
		//private Spool AddSpool(BoardPos pos, int colorID) {
  //          return AddSpool(new SpoolData(pos, colorID));
		//}
        private Spool AddSpool(SpoolData data) {
            Spool prop = new Spool (this, data);
            spools.Add (prop);
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
			if (bo is Spool) { spools.Remove(bo as Spool); }
			else { Debug.LogError ("Trying to RemoveFromPlay an Object of type " + bo.GetType().ToString() + ", but our OnObjectRemovedFromPlay function doesn't recognize this type!"); }
		}





        private void RemoveAllSpoolPaths() {
            foreach (Spool spool in spools) {
                spool.RemoveAllPathSpaces();
            }
        }







		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void OnMoveComplete () {
			AreGoalsSatisfied = GetAreGoalsSatisfied();
		}


		// ----------------------------------------------------------------
		//  Debug
		// ----------------------------------------------------------------
        public void Debug_SetDifficulty(int _difficulty) {
            Difficulty = _difficulty;
        }
        
        /*
void AddRandomGoodWells() {
  int count=0;
  while (count++<99) {
    AddRandomWells();
    // Are these good enough? Nice! Stop. :)
//    println("count: " + count + " AreRandomlyMadeWellsGood: " + AreRandomlyMadeWellsGood());
//    printGridSpaces();
    if (AreRandomlyMadeWellsGood()) { break; }
  }
}
boolean AreRandomlyMadeWellsGood() {
  for (int i=0; i<wells.length; i++) {
    if (wells[i].pathSpaces.length < MinRandWellPathLength) { return false; }
  }
  return true;
}


void AddRandomWells() {
  RemoveAllWells();
  
  int numSpacesOpen = cols*rows;
  
  int count=0;
  while (numSpacesOpen>0 && count++<999) {
    Well newWell = AddRandomWell();
    if (newWell == null) { break; } // Safety check.
    numSpacesOpen -= newWell.pathSpaces.length;
  }
}
Well AddRandomWell() {
  int count=0;
  while (true) {
    GridSpace randSpace = GetRandSpace();
    if (randSpace.CanAddWellPath()) { // This space is vacant!
      int colorID = wells.length;
      int value = 1; // will set next.
      Well newWell = AddWell(randSpace.col,randSpace.row, colorID,value);
      GiveWellRandPath(newWell);
      return newWell;
    }
    // Safety check.
    if (count++ > 999) { println("Whoa! No vacant board spaces left!"); return null; }//printGridSpaces(); 
  }
}

void GiveWellRandPath(Well well) {
  int numSpacesToFill = (int)random(cols-1,cols+3);
  int count=0;
  Vector2Int currSpace = new Vector2Int(well.col,well.row);
  while (well.pathSpaces.length<numSpacesToFill && count++<99) {
    int randSide = RandOpenSide(currSpace);
    if (randSide == -1) { break; } // Whoa! Hit a dead end? Ok, totally stop.
    Vector2Int dir = GetDir(randSide);
    currSpace = currSpace.Plus(dir);
    well.AddPathSpace(currSpace);
  }
  // Set this Well's numSpacesToFill!
  well.numSpacesToFill = well.pathSpaces.length;
  well.UpdateNumSpacesLeft();
}

int RandOpenSide(Vector2Int sourcePos) {
  int[] randSides = GetShuffledIntArray(4);
  // Try each side one by one.
  for (int i=0; i<randSides.length; i++) {
    Vector2Int dir = GetDir(randSides[i]);
    if (IsSpaceOpen(sourcePos.Plus(dir))) {
      return randSides[i];
    }
  }
  // Nah, nothin' fits.
  return -1;
}
*/


		public void Debug_AddSpoolsIfNone(RandGenParams rgp) {
			if (spools.Count > 0) { return; } // Nah, we've got some.
			int numToAdd = 2; // TODO: This whole chestnut.
			Debug_AddRandomWalls(rgp.NumWalls);
			Debug_AddRandomSpools(numToAdd);
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
		private void Debug_AddRandomSpools(int numToAdd) {
            for (int i=0; i<numToAdd; i++) {
                BoardPos randPos = BoardUtils.GetRandOpenPos(this);
                if (randPos == BoardPos.undefined) { break; } // No available spaces left?? Get outta here.
                int colorID = i;
                SpoolData spoolData = new SpoolData(randPos, colorID, null);
                AddSpool(spoolData);
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

