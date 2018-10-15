// getters


Tile GetTile(int col,int row) {
  if (GetSpace(col,row) == null) return null;
  return GetSpace(col,row).getTile();
}
GridSpace GetSpace(int col,int row) {
  if (col<0 || row<0  ||  col>=cols || row>=rows) return null;
  return gridSpaces[col][row];
}

GridSpace GetRandSpace() {
  return gridSpaces[RandCol()][RandRow()];
}
int RandCol() { return int(random(cols)); }
int RandRow() { return int(random(rows)); }
int RandColor() { return int(random(numColors)); }

float getGridPosX(int col) { return gridX + unitSize.x*(col+0.5); }
float getGridPosY(int row) { return gridY + unitSize.y*(row+0.5); }

Boolean isPreviewMove() { return previewDirX!=0 || previewDirY!=0; }


//Boolean canTilesMerge(Tile tile1,Tile tile2) {
//  //if (tile1==null || tile2==null) return false; // No tile? No merging.
//  if (tile1.value+tile2.value == 3) return true;
//  else if (tile1.value+tile2.value < 6) return false;
//  return tile1.value == tile2.value;
//}

Boolean CanTileMoveInDir(Tile tile, int dirX,int dirY) {
  if (tile==null) return false; // No tile? No moving.
  GridSpace gridSpace = GetSpace(tile.col+dirX,tile.row+dirY);
  if (gridSpace==null) return false;
  if (gridSpace.getTile() != null) { return false; } // Another Tile? Can't move in dir.
// if (tile!=null&&gridSpace.getTile()!=null && !canTilesMerge(tile, gridSpace.getTile())) return false;
  return true;
}


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


// TILE VISUALS
color GetFillColor(int colorID) {
//  return color((colorID*50)%255,200,200);
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
float GetCardThicknessFromColorID(int colorID) {
  return 4;
//  switch (colorID) {
//    case -1: return 0;
//    case 0: return 4;
//    case 1: return 6;
//    case 2: return 8;
//    case 3: return 10;
//    case 4: return 12;
//    default: return 22; // Hmm.
//  }
}
//  BODY_COLORS = new color[3];
//  BODY_COLORS[0] = #6AC8F5;
//  BODY_COLORS[1] = #FFC15D;
//  BODY_COLORS[2] = color(32,8,250);
//  SIDE_COLORS = new color[BODY_COLORS.length];
//  SIDE_COLORS[0] = #2394BF;
//  SIDE_COLORS[1] = #D8A44F;
//  SIDE_COLORS[2] = color(32,120,255);




