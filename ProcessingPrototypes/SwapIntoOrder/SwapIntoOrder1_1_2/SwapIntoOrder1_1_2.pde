// Swap-Into-Order
// started 10/5/2018



// Constants
float MIN_HORZ_GAP = 100;
float MIN_VERT_GAP = 80;

// grid properties
int cols;
int rows;
float unitSize; // how big each grid square is.
float tileDiameter; // how big each TILE is
float boardPosX;
float boardPosY;
// game properties
int score;
GridSpace spaceSelected; // the first space of two that I click on
GridSpace spaceSwappingWith; // the candidate space I'm swappin' with
// general variables
boolean canSwap_L,canSwap_R, canSwap_U,canSwap_D;
int mouseGridX,mouseGridY;
int pmouseGridX,pmouseGridY;
float mouseDownX,mouseDownY; // the position of the mouse when I clicked down
float draggingUnitDistanceFromOrigin; // used for aesthetics
// time-inspired variables
int pmillis; // previous millis()

// classes
GridSpace[][] spaces;
GridSpace[][] groups; // by color of the space

// other stuff
PFont myFont;


void setup() {
  size(800,600);
  colorMode(HSB);
  frameRate(50);
  rectMode(CENTER);
  
  myFont = loadFont("ArialRoundedMTBold-72.vlw");
  textFont(myFont);
  
  resetGame();
}


void resetGame() {
  // Reset primatives!
  pmillis = millis();
  score = 0;
  spaceSelected = null;
  spaceSwappingWith = null;
  draggingUnitDistanceFromOrigin = 0;
  
  cols = 3;
  rows = 3;
  
  unitSize = min((width-MIN_HORZ_GAP*2)/cols, (height-MIN_VERT_GAP*2)/rows);
  tileDiameter = unitSize * 0.9;
  boardPosX = (width - unitSize*cols) / 2;
  boardPosY = (height - unitSize*rows) / 2;
  
  // spaces!
  spaces = new GridSpace[cols][rows];
  for (int i=0; i<spaces.length; i++) {
    for (int j=0; j<spaces[i].length; j++) {
      spaces[i][j] = new GridSpace(i,j);//randomColorID());
    }
  }
  
  // Set space numbers and stuff!
  int numColors = 3;
  int numNumbers = 3;
  for (int c=0; c<numColors; c++) {
    for (int n=0; n<numNumbers; n++) {
      // TEST leave one space blank!
      if (c==numColors-1 && n==numNumbers-1) { break; }
      int count=0;
      while (true) {
        GridSpace randSpace = GetRandSpace();
        if (randSpace.colorID == -1) { // This space is vacant!
          randSpace.SetColorID(c);
          randSpace.SetNumberID(n+1); // start numberID at 1.
          break;
        }
        
        // Safety check.
        if (count++ > 999) { println("Whoa! No vacant board spaces left!"); c=numColors; n=numNumbers; break; }
      }
    }
  }
  
  recalculateGroups();
}




// ================================================================
//   DRAW
// ================================================================
void draw() {
  // -------- UPDATE --------
  float deltaTime = (millis()-pmillis) * 0.001;
  pmillis = millis();
  for (int i=0; i<spaces.length; i++) {
    for (int j=0; j<spaces[i].length; j++) {
      spaces[i][j].Update();
    }
  }
  
  
  // -------- DRAW --------
  // background
  background(240);
  
  // set mouseGridX and mouseGridY
  mouseGridX = floor((mouseX-boardPosX) / unitSize);
  mouseGridY = floor((mouseY-boardPosY) / unitSize);
  if (pmouseGridX!=mouseGridX || pmouseGridY!=mouseGridY) {
    onMouseEnterNewGridSpace();
  }
  
  // GRID
  pushMatrix();
  translate(boardPosX,boardPosY);
  drawGridLines();
  popMatrix();
  
  // GridSpaces!
  for (int i=0; i<spaces.length; i++) {
    for (int j=0; j<spaces[i].length; j++) {
      if (spaces[i][j] == spaceSelected) continue;
      spaces[i][j].Draw();
    }
  }
  if (spaceSelected!=null) spaceSelected.Draw();
  
  pmouseGridX = mouseGridX;
  pmouseGridY = mouseGridY;
}

void drawGridLines() {
  stroke(128, 200);
  strokeWeight(1);
  for (int i=0; i<cols+1; i++) {
    line(i*unitSize,0, i*unitSize,unitSize*rows);
  }
  for (int j=0; j<rows+1; j++) {
    line(0,j*unitSize, unitSize*cols,j*unitSize);
  }
}



void onMouseEnterNewGridSpace() {
  if (spaceSelected != null) {
    if (isSpaceInGrid(mouseGridX,mouseGridY)) {
      
    }
  }
}




void mousePressed() {
  mouseDownX = mouseX;
  mouseDownY = mouseY;
  
  // Mouse in da grid?
  if (isSpaceInGrid(mouseGridX,mouseGridY)) {
    if (spaceSelected == null) {
      selectSpace(spaces[mouseGridX][mouseGridY]);
    }
    else {
      swapSpaces(spaceSelected, spaces[mouseGridX][mouseGridY]);
    }
    
  }
}
void mouseReleased() {
  if (spaceSelected != null) {
    int col = spaceSelected.col;
    int row = spaceSelected.row;
    if (mouseGridX<spaceSelected.col) col = spaceSelected.col-1;
    else if (mouseGridX>spaceSelected.col) col = spaceSelected.col+1;
    else if (mouseGridY<spaceSelected.row) row = spaceSelected.row-1;
    else if (mouseGridY>spaceSelected.row) row = spaceSelected.row+1;
    if (canSwapWithSpaceAt(col,row)) {
      swapSpaces(spaceSelected, spaces[col][row]);
    }
  }
  
  spaceSelected = null;
  spaceSwappingWith = null;
  
}

void keyPressed() {
  if (keyCode == ENTER) {
    resetGame();
  }
  
  else if (keyCode == UP)    MoveTiles(0, -1);
  else if (keyCode == DOWN)  MoveTiles(0,  1);
  else if (keyCode == LEFT)  MoveTiles(-1, 0);
  else if (keyCode == RIGHT) MoveTiles( 1, 0);
  
}





