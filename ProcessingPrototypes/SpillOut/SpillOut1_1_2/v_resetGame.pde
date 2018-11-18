// v_resetGame


void resetGame() {
  cols = 5;
  rows = 5;
  
  float unitDiameter = min((width-MIN_HORZ_GAP*2)/cols, (height-MIN_VERT_GAP*2)/rows);
  unitSize = new PVector(unitDiameter,unitDiameter);
  gridSize = new PVector(cols*unitSize.x, rows*unitSize.y);
  gridX = (width-gridSize.x) * 0.5;
  gridY = (height-gridSize.y) * 0.5;
  
  boardSnapshots = new BoardData[0];
  
  
  gridSpaces = new GridSpace[cols][rows];
  wells = new ArrayList();
  
  
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row] = new GridSpace(col,row);
    }
  }
  
  // Set space numbers and stuff!
//  AddWell(1,1, 0, 5);
  int numWells = 4;
  for (int i=0; i<numWells; i++) {
    int count=0;
    while (true) {
      GridSpace randSpace = GetRandSpace();
      if (randSpace.CanAddWellPath()) { // This space is vacant!
        int colorID = i;
        int value = (int)random(3,8);
        AddWell(randSpace.col,randSpace.row, colorID,value);
        break;
      }
      // Safety check.
      if (count++ > 999) { println("Whoa! No vacant board spaces left!"); i=numWells; break; }
    }
  }
  
  /*
  int numStartingWells = int(cols*rows * 0.6);
  int randCol,randRow;
  for (int i=0; i<numStartingWells; i++) {
    do {
      randCol = int(random(cols));
      randRow = int(random(rows));
    } while (GetWell(randCol,randRow) != null);
    addWell(randCol,randRow, random(numColors));
  }
  */
  
  OnMoveComplete();
}
