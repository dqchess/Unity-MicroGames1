

int sign(float value) {
  if (value<0) return -1;
  else if (value>0) return 1;
  else return 0;
}


boolean isSpaceInGrid(int col,int row) { return col>=0 && col<cols  &&  row>=0 && row<rows; }
boolean canSwapWithSpaceAt(int col,int row) {
  return spaceSelected!=null && isSpaceInGrid(col,row);// && spaces[col][row].colorID!=.colorID;
}

int randomCol() { return int(random(cols)); }
int randomRow() { return int(random(rows)); }
int randomColorID() { return int(random(NumColors)); }
GridSpace getGridSpaceAt(int col,int row) {
  if (col>=0 && col<cols  &&  row>=0 && row<rows) return spaces[col][row];
  return null;
}





