// Spill-Out
// started 11/16/2018

/*
TODOS
Allow clicking anywhere on a path to truncate it to that point
Rand generate level ;)

*/



// Constants
float MIN_HORZ_GAP = 100;
float MIN_VERT_GAP = 80;
// Grid Properties
int cols,rows;
float gridX,gridY;
PVector gridSize;
PVector unitSize;

// Variables
BoardData[] boardSnapshots;
int mouseCol,mouseRow;
float mouseDownX,mouseDownY;
Well wellOver;
Well wellGrabbing;

// Objects
GridSpace[][] gridSpaces;
ArrayList wells;

// Assorted stuff
PFont myFont;


void setup() {
  size(800,600);
  colorMode(HSB);
  frameRate(40);
  smooth();
  
  myFont = createFont("Helvetica", 64);
  textFont(myFont);
  
  resetGame();
}


void draw() {
  background(250);
  
  // Where the dragging mouse be at?
  UpdateMousePosGrid();
  UpdateWellOver();
  
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
  
  // Wells!
  for (int i=wells.size()-1; i>=0; --i) {
    Well obj = (Well) wells.get(i);
    obj.Update();
    obj.Draw();
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
  if (wellGrabbing != null) {
    TryToDragWellEndToPos(wellGrabbing, mouseCol,mouseRow);
  }
}

void UpdateWellOver() {
  wellOver = GetWell(mouseCol,mouseRow);
}




void mousePressed() {
  mouseDownX = mouseX;
  mouseDownY = mouseY;
  // Grab Well??
  if (wellOver != null) {
    TruncateWell(wellOver, mouseCol,mouseRow);
    SetWellGrabbing(wellOver);
  }
}
void mouseReleased() {
  ReleaseWellGrabbing();
}

void keyPressed() {
  if (keyCode == ENTER) resetGame();
  
  else if (key == 'z') UndoMove();
  else if (key == 'x') RedoMove();
  
  else if (key == 'p') printGridSpaces();
}






