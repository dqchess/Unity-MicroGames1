// Snake Cross
// started 12/4/2018

/*
TODOS
Add Targets!
Allow adding Spool anywhere open.

DONE
One Spool with infinite length
Spool can overlap itself
When Spool overlaps, it adds to its tier.

*/



// Constants
final int MinRandSpoolPathLength = 3;
float MIN_HORZ_GAP = 100;
float MIN_VERT_GAP = 80;
// Grid Properties
int cols,rows;
float gridX,gridY;
PVector gridSize;
PVector unitSize;

// Variables
BoardData[] boardSnapshots;
int levelIndex;
int mouseCol,mouseRow;
float mouseDownX,mouseDownY;
Spool spoolOver;
Spool spoolGrabbing;

// Objects
GridSpace[][] gridSpaces;
Spool[] spools;

// Assorted stuff
PFont myFont;


void setup() {
  size(800,600);
  colorMode(HSB);
  frameRate(40);
  smooth();
  
  myFont = createFont("Helvetica", 64);
  textFont(myFont);
  
  resetGame(2);
}


void draw() {
  background(250);
  
  // Where the dragging mouse be at?
  UpdateMousePosGrid();
  UpdateSpoolOver();
  
  // Grid back
  fill(128,28,220);
  noStroke();
  rect(gridX+gridSize.x*0.5,gridY+gridSize.y*0.5, gridSize.x+20,gridSize.y+20, 24);
  
  // Grid spaces
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].draw();
    }
  }
  
  // Spools!
  for (int i=spools.length-1; i>=0; --i) {
    spools[i].Update();
    spools[i].Draw();
  }
}



void UpdateMousePosGrid() {
  int pmouseCol = mouseCol;
  int pmouseRow = mouseRow;
  mouseCol = floor((mouseX-gridX) / unitSize.x);
  mouseRow = floor((mouseY-gridY) / unitSize.y);
  if (pmouseCol!=mouseCol || pmouseRow!=mouseRow) {
    OnMousePosGridChanged();
  }
}
void OnMousePosGridChanged() {
  if (spoolGrabbing != null) {
    TryToDragSpoolEndToPos(spoolGrabbing, mouseCol,mouseRow);
  }
}

void UpdateSpoolOver() {
  spoolOver = GetTopSpool(mouseCol,mouseRow);
}




void mousePressed() {
  mouseDownX = mouseX;
  mouseDownY = mouseY;
  // Grab Spool??
  if (spoolOver != null) {
    TruncateSpool(spoolOver, mouseCol,mouseRow);
    SetSpoolGrabbing(spoolOver);
  }
}
void mouseReleased() {
  ReleaseSpoolGrabbing();
}

void keyPressed() {
  if (keyCode == ENTER) resetGame(levelIndex);
  else if (key == '[') { resetGame(levelIndex-1); }
  else if (key == ']') { resetGame(levelIndex+1); }
  
  else if (key == 'z') UndoMove();
  else if (key == 'x') RedoMove();
  
  else if (key == 'p') printGridSpaces();
}






