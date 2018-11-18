// doers


void MakeBoardFromData(BoardData bd) {
  wells = new ArrayList();
  for (int i=0; i<bd.wells.length; i++) {
    AddWell(bd.wells[i].clone());
  }
}


Well AddWell(int col,int row, int colorID,int value) { return AddWell(new Well(col,row, null, colorID,value)); }
Well AddWell(Well newWell) {
  wells.add(newWell);
  return newWell;
}


void printGridSpaces() {
  for (int row=0; row<rows; row++) {
    String tempString = "";
    for (int col=0; col<cols; col++) {
      if (GetWell(col,row) != null) tempString += GetWell(col,row).colorID;
      else tempString += ".";
    }
    println(tempString);
  }
}


void TruncateWell(Well well, int col,int row) {
  Vector2Int truncPos = new Vector2Int(col,row);
  if (!well.PathContains(truncPos)) { // Safety check.
    println("Whoa!! Trying to truncate a Well, but it doesn't have the space we wanna truncate to!");
    return;
  }
  while (!well.lastSpacePos().Equals(truncPos)) {
    well.RemovePathSpace();
  }
}
void ResetAllWellPaths() {
//  for (int i=0
}

void SetWellGrabbing(Well _well) {
  wellGrabbing = _well;
}
void ReleaseWellGrabbing() {
  SetWellGrabbing(null);
}

// CANDO: a while loop. Keep trying to go until we can't.
void TryToDragWellEndToPos(Well well, int _col,int _row) {
  if (well.numSpacesLeft <= 0) { return; } // Whoa, no spaces left in it? Do nothin'.
  Vector2Int endPos = well.lastSpacePos();
  Vector2Int dir = new Vector2Int(sign(_col-endPos.x), sign(_row-endPos.y));
  if (dir.x!=0 && dir.y!=0) { dir.y = 0; } // Don't-allow-diagonals safety check.
  Vector2Int newPos = new Vector2Int(endPos.x+dir.x, endPos.y+dir.y);
  // This is the second-to-last space? REMOVE pathSpace.
  if (well.IsSecondLastSpacePos(newPos)) {
    well.RemovePathSpace();
  }
  // Otherwise, can we ADD this space to the path? Do!
  else if (CanWellPathEnterSpace(newPos)) {
    well.AddPathSpace(newPos);
  }
}






void OnMoveComplete() {
  // Add snapshot!
  boardSnapshots = (BoardData[]) append(boardSnapshots, new BoardData(wells));
//  undoSnapshotIndex = boardSnapshots.length-1;
}



//int undoSnapshotIndex;
boolean CanUndoMove() {
  return boardSnapshots.length > 0;//undoSnapshotIndex > 1;
}
void UndoMove() {
  if (CanUndoMove()) {
    BoardData snapshot = boardSnapshots[boardSnapshots.length-1];//undoSnapshotIndex];
    boardSnapshots = (BoardData[]) shorten(boardSnapshots);
    MakeBoardFromData(snapshot);
  }
}
void RedoMove() {
//  if (CanRedoMove()) {
//    
//  }
}











