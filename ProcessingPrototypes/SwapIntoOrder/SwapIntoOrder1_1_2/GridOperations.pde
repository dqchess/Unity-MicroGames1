void recalculateGroups() {
  // FIRST, tell all spaces they're not used in the search algorithm
  for (int i=0; i<cols; i++) {
    for (int j=0; j<rows; j++) {
      spaces[i][j].groupID = -1;
    }
  }
  
  groups = new GridSpace[0][0];
  
  for (int i=0; i<cols; i++) {
    for (int j=0; j<rows; j++) {
      if (spaces[i][j].groupID != -1) continue;
      groups = (GridSpace[][]) append(groups, new GridSpace[0]);
      infectiousGroupFinding(spaces[i][j].colorID, i,j);
    }
  }
  
}
void infectiousGroupFinding(int colorID, int col,int row) {
  if (spaces[col][row].groupID != -1) return;
  if (spaces[col][row].colorID != colorID) return;
  spaces[col][row].groupID = groups.length-1;
  groups[groups.length-1] = (GridSpace[]) append(groups[groups.length-1], spaces[col][row]);
  if (col>0) infectiousGroupFinding(colorID, col-1,row);
  if (row>0) infectiousGroupFinding(colorID, col,row-1);
  if (col<cols-1) infectiousGroupFinding(colorID, col+1,row);
  if (row<rows-1) infectiousGroupFinding(colorID, col,row+1);
}
