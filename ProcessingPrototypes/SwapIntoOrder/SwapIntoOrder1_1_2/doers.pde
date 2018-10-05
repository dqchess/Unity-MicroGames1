



void swapSpaces(GridSpace s1, GridSpace s2) {
  int colorID1 = s1.colorID;
  int numberID1 = s1.numberID;
  s1.SetColorID(s2.colorID);
  s1.SetNumberID(s2.numberID);
  s2.SetColorID(colorID1);
  s2.SetNumberID(numberID1);
  
  /*
  int colorID1 = s1.colorID;
  int colorID2 = s2.colorID;
  for (int i=0; i<groups[s1.groupID].length; i++) {
    groups[s1.groupID][i].colorID = colorID2;
  }
  for (int i=0; i<groups[s2.groupID].length; i++) {
    groups[s2.groupID][i].colorID = colorID1;
  }
  */
  
  spaceSelected = null;
  spaceSwappingWith = null;
  draggingUnitDistanceFromOrigin = 0;
  
  recalculateGroups();
}


void selectSpace(GridSpace space) {
  spaceSelected = space;
  //spacesCanSwapWith = new GridSpace[0];
  int col = spaceSelected.col;
  int row = spaceSelected.row;
  canSwap_L = isSpaceInGrid(col-1,row);// && spaces[col-1][row].colorID!=spaceSelected.colorID;
  canSwap_R = isSpaceInGrid(col+1,row);// && spaces[col+1][row].colorID!=spaceSelected.colorID;
  canSwap_U = isSpaceInGrid(col,row-1);// && spaces[col][row-1].colorID!=spaceSelected.colorID;
  canSwap_D = isSpaceInGrid(col,row+1);// && spaces[col][row+1].colorID!=spaceSelected.colorID;
}


void MoveTiles(int dirX, int dirY) {
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
}

private void MoveTileAttempt(int col,int row, int dirX,int dirY) {
  Tile tile = GetTile(col,row);
  if (tile != null) {
    if (CanTileMoveInDir(tile, dirX,dirY)) {
      MoveTile(tile, dirX,dirY);
    }
  }
}








