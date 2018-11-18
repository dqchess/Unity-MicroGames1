// getters


//Well GetWellPathEnd(int col,int row) {
//  Well wellHere = GetWell(col,row);
//  if (wellHere == null) { return null; }
//  // Ah, there's a Well here, but it's not its last space.
//  if (!wellHere.IsLastSpace(col,row)) { return null; }
//  // There's a Well here and this is its last space!
//  return wellHere;
//}
Well GetWell(int col,int row) {
  if (GetSpace(col,row) == null) return null;
  return GetSpace(col,row).GetWellOnMe();
}
GridSpace GetSpace(Vector2Int pos) { return GetSpace(pos.x,pos.y); }
GridSpace GetSpace(int col,int row) {
  if (col<0 || row<0  ||  col>=cols || row>=rows) return null;
  return gridSpaces[col][row];
}

GridSpace GetRandSpace() {
  return gridSpaces[RandCol()][RandRow()];
}
int RandCol() { return int(random(cols)); }
int RandRow() { return int(random(rows)); }

PVector GridToPos(int col,int row) { return new PVector(GridToX(col), GridToY(row)); }
PVector GridToPos(Vector2Int boardPos) { return GridToPos(boardPos.x,boardPos.y); }
float GridToX(int col) { return gridX + unitSize.x*(col+0.5); }
float GridToY(int row) { return gridY + unitSize.y*(row+0.5); }



boolean CanWellPathEnterSpace(Vector2Int spacePos) {
  GridSpace space = GetSpace(spacePos);
  if (space==null) { return false; }
  if (!space.CanAddWellPath()) { return false; }
  return true;
}

//Boolean CanWellMoveInDir(Well tile, int dirX,int dirY) {//, int groupID) {
//  if (tile==null) return false; // No tile? No moving.
////  if (tile.groupID != groupID) { return false; } // Wrong groupID? No moving.
//  GridSpace gridSpace = GetSpace(tile.col+dirX,tile.row+dirY);
//  if (gridSpace==null) return false;
//  if (gridSpace.GetWellOnMe() != null) { return false; } // Another Well? Can't move in dir.
//// if (tile!=null&&gridSpace.getWell()!=null && !canWellsMerge(tile, gridSpace.getWell())) return false;
//  return true;
//}


//
//int randomNextCol() {
//  int col;
//  int count = 0;
//  do {
//    col = int(random(cols));
//    if (count++>100) return -1;
//  } while (didMoveInCol[col] == false);
//  return col;
//}
//int randomNextRow() {
//  int row;
//  int count = 0;
//  do {
//    row = int(random(rows));
//    if (count++>100) return -1;
//  } while (didMoveInRow[row] == false);
//  return row;
//}


// WELL VISUALS
color GetFillColor(int colorID) {
  switch (colorID) {
    case -1: return color(128, 20);
    case 0: return color(128,220,200);
    case 1: return color(58,220,200);
    case 2: return color(28,200,245);
    case 3: return color(190,150,245);
    case 4: return color(230,150,245);
    default: return color(255,0,0); // Hmm.
  }
}
color GetStrokeColor(int colorID) {
  switch (colorID) {
    case -1: return color(128, 50);
    case 0: return color(128,220,140);
    case 1: return color(58,220,140);
    case 2: return color(28,240,180);
    case 3: return color(190,230,160);
    case 4: return color(230,150,160);
    default: return color(255,0,0); // Hmm.
  }
}




