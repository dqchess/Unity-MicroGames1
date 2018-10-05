

int sign(float value) {
  if (value<0) return -1;
  else if (value>0) return 1;
  else return 0;
}


boolean isSpaceInGrid(int col,int row) { return col>=0 && col<cols  &&  row>=0 && row<rows; }
boolean canSwapWithSpaceAt(int col,int row) {
  return spaceSelected!=null && isSpaceInGrid(col,row);// && spaces[col][row].colorID!=.colorID;
}


GridSpace GetRandSpace() {
  return spaces[randomCol()][randomRow()];
}
int randomCol() { return int(random(cols)); }
int randomRow() { return int(random(rows)); }
//int randomColorID() { return int(random(numColors)); }
GridSpace getGridSpaceAt(int col,int row) {
  if (col>=0 && col<cols  &&  row>=0 && row<rows) return spaces[col][row];
  return null;
}


GridSpace GetSpace(int col,int row) {
  return spaces[col][row];
}
Tile GetTile(int col,int row) {
  return null;
//  return spaces[col][row].myTile;
}



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


