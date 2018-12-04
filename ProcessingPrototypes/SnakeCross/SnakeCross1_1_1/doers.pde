// doers


void MakeBoardFromData(BoardData bd) {
  spools = new Spool[0];
  for (int i=0; i<bd.spools.length; i++) {
    AddSpool(bd.spools[i].clone());
  }
}


Spool AddSpool(int col,int row, int colorID) { return AddSpool(new Spool(col,row, null, colorID)); }
Spool AddSpool(Spool newSpool) {
  spools = (Spool[]) append(spools, newSpool);
  return newSpool;
}
void RemoveAllSpools() {
  spools = new Spool[0];
  // Reset all GridSpaces, too, yo.
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].Reset();
    }
  }
}

void printGridSpaces() {
  for (int row=0; row<rows; row++) {
    String str = "";
    for (int col=0; col<cols; col++) {
      if (GetTopSpool(col,row) != null) str += GetTopSpool(col,row).colorID;
      else str += ".";
    }
    println(str);
  }
}


void TruncateSpool(Spool spool, int col,int row) {
  Vector2Int truncPos = new Vector2Int(col,row);
  if (!spool.PathContains(truncPos)) { // Safety check.
    println("Whoa!! Trying to truncate a Spool, but it doesn't have the space we wanna truncate to!");
    return;
  }
  while (!spool.lastSpacePos().Equals(truncPos)) {
    spool.RemovePathSpace();
  }
}
void ClearAllSpoolPaths() {
  for (int i=0; i<spools.length; i++) {
    spools[i].RemoveAllPathSpaces();
  }
}

void SetSpoolGrabbing(Spool _spool) {
  spoolGrabbing = _spool;
}
void ReleaseSpoolGrabbing() {
  SetSpoolGrabbing(null);
}

// CANDO: a while loop. Keep trying to go until we can't.
void TryToDragSpoolEndToPos(Spool spool, int _col,int _row) {
  Vector2Int endPos = spool.lastSpacePos();
  Vector2Int dir = new Vector2Int(sign(_col-endPos.x), sign(_row-endPos.y));
  if (dir.x!=0 && dir.y!=0) { dir.y = 0; } // Don't-allow-diagonals safety check.
  Vector2Int newPos = new Vector2Int(endPos.x+dir.x, endPos.y+dir.y);
  // This is the second-to-last space? REMOVE pathSpace.
  if (spool.IsSecondLastSpacePos(newPos)) {
    spool.RemovePathSpace();
  }
  // Otherwise, can we ADD this space to the path? Do!
  else if (CanSpoolPathEnterSpace(spool, newPos)) {
    spool.AddPathSpace(newPos);
  }
}






void OnMoveComplete() {
  // Add snapshot!
  boardSnapshots = (BoardData[]) append(boardSnapshots, new BoardData(spools));
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











