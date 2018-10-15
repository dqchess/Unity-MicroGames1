// doers


void addTile(int col,int row, int colorID,int numberID) { addTile(new Tile(col,row, colorID,numberID)); }
void addTile(Tile newTile) {
  tiles.add(newTile);
  gridSpaces[newTile.col][newTile.row].setTile(newTile);
}
void RemoveTile(int col,int row) { RemoveTile(GetTile(col,row)); }
void RemoveTile(Tile tile) {
  if (tile == null) { return; } // Safety check.
  removeTileFromItsGridSpace(tile);
  tiles.remove(tile);
}

void AddRandTile(int fromDirX,int fromDirY) {
  int count=0;
  while (true) {
    GridSpace randSpace = GetRandSpace(fromDirX,fromDirY);
    if (randSpace!=null && randSpace.IsOpen()) { // This space is vacant!
      int colorID = (int)random(0, 4);
      int numberID = (int)random(0, 3)+1; // start numberID at 1.
      addTile(randSpace.col,randSpace.row, colorID,numberID);
      break;
    }
    // Safety check.
    if (count++ > 999) { println("Whoa! No vacant board spaces left!"); break; }
  }
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

void determineWhatTilesCanMoveInPreview() {
  // Make a copy of all the tiles
  ArrayList tilesCopy = new ArrayList();
  for (int i=0; i<tiles.size(); i++) {
    Tile tempTile = (Tile) tiles.get(i);
    tilesCopy.add(tempTile.clone());
  }
  
  // ACTUALLY move the tiles!
  MoveTiles(previewDirX,previewDirY);
  
  // Restore the tiles as they were!
  // Nullify grid spaces' tiles
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].setTile(null);
    }
  }
  // Re-add all the tiles again
  tiles = new ArrayList();
  for (int i=0; i<tilesCopy.size(); i++) {
    Tile tempTile = (Tile) tilesCopy.get(i);
    addTile(tempTile);
  }
  // For all the spaces that DID succeed in moving a tile or whatever, let the tiles on those spaces know!
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      Tile tile = GetTile(col,row);
      if (tile != null) {
        tile.canMoveInPreview = true;
        tile.previewXDisplay = gridSpaces[col][row].myTilePreviewXDisplay;
        tile.previewYDisplay = gridSpaces[col][row].myTilePreviewYDisplay;
//        // If this space was informed that a tile moved on it for the preview...
//        if (gridSpaces[col][row].canMoveMyTileInPreviewMove) {
//          gridSpaces[col][row].getTile().canMoveInPreview = true;
//        }
//        // Otherwise, if there is at least a tile here, tell it it CAN'T move from the preview
//        else {
//          GetTile(col,row).canMoveInPreview = false;
//        }
      }
    }
  }
}
void resetPreviewDrag() {
  previewLoc = 0;
  previewDirX = previewDirY = 0;
  
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].ResetPreview();
    }
  }
}
void ResetStopperPos() {
  stopperCol = -1;
  stopperRow = -1;
}



void moveTileInDir(Tile tile, int dirX,int dirY) {
  // What's the tile in the space I'm moving into?
  GridSpace gridSpace = GetSpace(tile.col+dirX,tile.row+dirY);
  // tileToMove may be different from the tile I've provided!! If it's a preview move.
  Tile tileToMove = GetTile(tile.col,tile.row);
  Tile otherTile = GetTile(tile.col+dirX,tile.row+dirY);
  // Into EMPTY space?
  if (gridSpace!=null && gridSpace.IsOpen()) {
    tileToMove.move(dirX,dirY);
    didMoveInCol[tileToMove.col] = true;
    didMoveInRow[tileToMove.row] = true;
  }
}


void resetColsRowsMovedIn() {
  for (int i=0; i<didMoveInCol.length; i++) didMoveInCol[i] = false;
  for (int i=0; i<didMoveInRow.length; i++) didMoveInRow[i] = false;
}

ArrayList GetTilesToMove(int dirX,int dirY) {
  ArrayList tilesToMove = new ArrayList();
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      GridSpace space = GetSpace(col,row);
      // This space is empty!
      if (space.getTile() == null) {
        Tile tileToMove = GetTile(col-dirX,row-dirY); // use -; dir is the dir of the *move*.
        if (tileToMove != null) {
          tilesToMove.add(tileToMove);
        }
      }
    }
  }
  return tilesToMove;
}
void MoveTiles(int dirX,int dirY) {
  resetColsRowsMovedIn();
  
  // VERTICAL
  if (dirY != 0) {
    int startingRow = dirY<0 ? 0 : rows-1;
    for (int col=0; col<cols; col++) {
      for (int row=startingRow; row>=0 && row<rows; row-=dirY) {
        MoveTileAttempt(col,row, dirX,dirY);
      }
    }
  }
  // HORIZONTAL
  else {
    int startingCol = dirX<0 ? 0 : cols-1;
    for (int row=0; row<rows; row++) {
      for (int col=startingCol; col>=0 && col<cols; col-=dirX) {
        MoveTileAttempt(col,row, dirX,dirY);
      }
    }
  }
  
  // Add new tile!
  AddRandTile(dirX,dirY);
}
private void MoveTileAttempt(int col,int row, int dirX,int dirY) {
  MoveTileAttempt(GetTile(col,row), dirX,dirY);
}
private void MoveTileAttempt(Tile tile, int dirX,int dirY) {
  if (tile != null) {
    tile.canMoveInPreview = CanTileMoveInDir(tile, dirX,dirY);
    if (tile.canMoveInPreview) {
      moveTileInDir(tile, dirX,dirY);
    }
  }
}











