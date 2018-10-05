// doers


void addTile(int col,int row, int colorID) { addTile(new Tile(col,row, colorID)); }
void addTile(Tile newTile) {
  tiles.add(newTile);
  gridSpaces[newTile.col][newTile.row].setTile(newTile);
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

void determineWhatTilesCanMoveInPreview() {
  // Make a copy of all the tiles
  ArrayList tilesCopy = new ArrayList();
  for (int i=0; i<tiles.size(); i++) {
    Tile tempTile = (Tile) tiles.get(i);
    tilesCopy.add(tempTile.clone());
  }
  
  // ACTUALLY move the tiles!
  MoveTiles(previewDirX, previewDirY, groupIDToMove);
  
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
void resetPreviewDrag() {
  previewAmount = 0;
  previewDirX = previewDirY = 0;
  
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].canMoveMyTileInPreviewMove = false;
    }
  }
}



void moveTileInDir(Tile tile, int dirX,int dirY) {
  // What's the tile in the space I'm moving into?
  GridSpace gridSpace = GetSpace(tile.col+dirX,tile.row+dirY);
  // tileToMove may be different from the tile I've provided!! If it's a preview move.
  Tile tileToMove = GetTile(tile.col,tile.row);
  Tile otherTile = GetTile(tile.col+dirX,tile.row+dirY);
  // Into EMPTY space?
  if (otherTile==null && gridSpace!=null) {
    tileToMove.move(dirX,dirY);
  }
//  // MERGE?
//  else {
//    tileToMove.moveAndMerge(dirX,dirY);
//  }
  didMoveInCol[tileToMove.col] = true;
  didMoveInRow[tileToMove.row] = true;
}


void resetColsRowsMovedIn() {
  for (int i=0; i<didMoveInCol.length; i++) didMoveInCol[i] = false;
  for (int i=0; i<didMoveInRow.length; i++) didMoveInRow[i] = false;
}

//boolean CanMoveTiles(int dirX,int dirY, int groupID) {
//  
//}

void MoveTiles(int dirX,int dirY, int groupID) {
  resetColsRowsMovedIn();
  
  /*
  // Get a list of all the Tiles we're gonna move. (Note: We want the list first, as some spaces become empty as we're moving stuff around.)
  ArrayList tilesToMove = GetTilesToMove(dirX,dirY);
  // Move each Tile!
  for (int i=0; i<tilesToMove.size(); i++) {
    Tile tile = (Tile) tilesToMove.get(i);
    MoveTileAttempt(tile, dirX,dirY);
  }
  //*/
  
  //*
  // VERTICAL
  if (dirY != 0) {
    int startingRow = dirY<0 ? 0 : rows-1;
    for (int col=0; col<cols; col++) {
      for (int row=startingRow; row>=0 && row<rows; row-=dirY) {
        MoveTileAttempt(col,row, dirX,dirY, groupID);
      }
    }
  }
  // HORIZONTAL
  else {
    int startingCol = dirX<0 ? 0 : cols-1;
    for (int row=0; row<rows; row++) {
      for (int col=startingCol; col>=0 && col<cols; col-=dirX) {
        MoveTileAttempt(col,row, dirX,dirY, groupID);
      }
    }
  }
  //*/
  
  RecalculateTileGroups();
}
private void MoveTileAttempt(int col,int row, int dirX,int dirY, int groupID) {
  MoveTileAttempt(GetTile(col,row), dirX,dirY, groupID);
}
private void MoveTileAttempt(Tile tile, int dirX,int dirY, int groupID) {
  if (tile != null) {
    tile.canMoveInPreview = CanTileMoveInDir(tile, dirX,dirY, groupID);
    if (tile.canMoveInPreview) {
      moveTileInDir(tile, dirX,dirY);
    }
  }
}

void RecalculateTileGroups() {
  // FIRST, tell all Tiles they're not used in the search algorithm
  for (int i=0; i<tiles.size(); i++) {
    Tile tile = (Tile) tiles.get(i);
    tile.groupID = -1;
  }
  
  tileGroups = new Tile[0][0];
  
  for (int i=0; i<tiles.size(); i++) {
    Tile tile = (Tile) tiles.get(i);
    if (tile.groupID != -1) { continue; }
    tileGroups = (Tile[][]) append(tileGroups, new Tile[0]);
    InfectiousGroupFinding(tile.col,tile.row, tile.colorID);
  }
  
}
void InfectiousGroupFinding(int col,int row, int colorID) {
  Tile tile = GetTile(col,row);
  if (tile == null) { return; }
  if (tile.groupID != -1) { return; }
  if (tile.colorID != colorID) { return; }
  tile.groupID = tileGroups.length-1;
  tileGroups[tileGroups.length-1] = (Tile[]) append(tileGroups[tileGroups.length-1], tile);
  InfectiousGroupFinding(col-1,row, colorID);
  InfectiousGroupFinding(col,row-1, colorID);
  InfectiousGroupFinding(col+1,row, colorID);
  InfectiousGroupFinding(col,row+1, colorID);
}

//private void StickMatchingTiles() {
//  // For each Tile
//  for (int i=0; i<tiles.size(); i++) {
//    
//  }
//}












