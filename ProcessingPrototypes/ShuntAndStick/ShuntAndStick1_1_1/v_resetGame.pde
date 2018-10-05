// v_resetGame


void resetGame() {
  cols = 5;
  rows = 5;
  
  float unitDiameter = min((width-MIN_HORZ_GAP*2)/cols, (height-MIN_VERT_GAP*2)/rows);
  unitSize = new PVector(unitDiameter,unitDiameter);
  tileSize = new PVector(unitSize.x*0.9, unitSize.y*0.9);
  gridSize = new PVector(cols*unitSize.x, rows*unitSize.y);
  gridX = (width-gridSize.x) * 0.5;
  gridY = (height-gridSize.y) * 0.5;
  
  rectRoundRadius = tileSize.x * 0.25;
  
  didMoveInCol = new boolean[cols];
  didMoveInRow = new boolean[rows];
  boardSnapshots = new BoardData[0];
  
  
  gridSpaces = new GridSpace[cols][rows];
  tiles = new ArrayList();
  
  
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row] = new GridSpace(col,row);
    }
  }
  
  // Set space numbers and stuff!
  int numColors = 3;
  int numNumbers = 5;
  for (int c=0; c<numColors; c++) {
    for (int n=0; n<numNumbers; n++) {
      int count=0;
      while (true) {
        GridSpace randSpace = GetRandSpace();
        if (randSpace.getTile() == null) { // This space is vacant!
          AddTile(randSpace.col,randSpace.row, c);
          break;
        }
        // Safety check.
        if (count++ > 999) { println("Whoa! No vacant board spaces left!"); c=numColors; n=numNumbers; break; }
      }
    }
  }
  
  /*
  int numStartingTiles = int(cols*rows * 0.6);
  int randCol,randRow;
  for (int i=0; i<numStartingTiles; i++) {
    do {
      randCol = int(random(cols));
      randRow = int(random(rows));
    } while (GetTile(randCol,randRow) != null);
    addTile(randCol,randRow, random(numColors));
  }
  */
  
  OnMoveComplete();
}