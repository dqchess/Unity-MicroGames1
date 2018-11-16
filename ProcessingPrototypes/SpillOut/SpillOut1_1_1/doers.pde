// doers


void MakeBoardFromData(BoardData bd) {
  wells = new ArrayList();
  for (int i=0; i<bd.wells.length; i++) {
    AddWell(bd.wells[i].clone());
  }
}


void AddWell(int col,int row, int colorID,int value) { AddTile(new Tile(col,row, null, colorID,value)); }
void AddWell(Tile newTile) {
  tiles.add(newTile);
  gridSpaces[newTile.col][newTile.row].setTile(newTile);
  newTile.GoToTargetPos();
}
void removeTile(Tile tile) {
  removeTileFromItsGridSpace(tile);
  tiles.remove(tile);
}

void addTileToItsGridSpace(Tile tile) {
  if (GetSpace(tile.col,tile.row).getTile() != null && GetSpace(tile.col,tile.row).getTile() != tile) {
    throw new Error("Hey, a tile is trying to ADD itself to a gridSpace that already has a tile on it ("+tile.col+","+tile.row+").");
  }
  gridSpaces[tile.col][tile.row].setTile(tile);
}
void removeTileFromItsGridSpace(Tile tile) {
  //if (GetSpace(tile.col,tile.row).getTile() != tile) {
    //throw new Error("Hey, a tile is trying to REMOVE itself from a gridSpace that doesn't own it ("+tile.col+","+tile.row+").");
  //}
  gridSpaces[tile.col][tile.row].setTile(null);
}

void printGridSpaces() {
  for (int row=0; row<rows; row++) {
    String tempString = "";
    for (int col=0; col<cols; col++) {
      if (gridSpaces[col][row].canMoveMyTileInPreviewMove) tempString += "M";
      //if (GetTile(col,row) != null) tempString += GetTile(col,row).value;
      else tempString += ".";
    }
    println(tempString);
  }
}


void SetTileGrabbing(Tile _tile) {
  tileGrabbing = _tile;
}
void ReleaseTileGrabbing() {
  SetTileGrabbing(null);
}





void determineWhatTilesCanMoveInPreview() {
  // Make a snapshot of the Board
  BoardData snapshot = new BoardData(tiles);
  
  // ACTUALLY move tileGrabbing!
//  MoveTile(tileGrabbing, previewDirX,previewDirY, true);QQQ
  
  // Restore the tiles as they were!
  // Nullify grid spaces' tiles
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].setTile(null);
    }
  }
  // Re-add all the tiles again
  MakeBoardFromData(snapshot);
  // For all the spaces that DID succeed in moving a tile or whatever, let the tiles on those spaces know!
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      // Is there a tile here?!
      if (GetTile(col,row) != null) {
        // If this space was informed that a tile moved on it for the preview...
        if (gridSpaces[col][row].canMoveMyTileInPreviewMove) {
          gridSpaces[col][row].getTile().canMoveInPreview = true;
        }
        // Otherwise, if there is at least a tile here, tell it it CAN'T move from the preview
        else {
          GetTile(col,row).canMoveInPreview = false;
        }
      }
    }
  }
}


void OnMoveComplete() {
  // Add snapshot!
  boardSnapshots = (BoardData[]) append(boardSnapshots, new BoardData(tiles));
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











