// Swap-Into-Order
// started 10/5/2018


// Constants
float MIN_HORZ_GAP = 100;
float MIN_VERT_GAP = 80;
// Grid Properties
int cols,rows;
float gridX,gridY;
PVector gridSize;
PVector tileSize;
PVector unitSize;
float rectRoundRadius;

// Variables
boolean[] didMoveInCol;
boolean[] didMoveInRow;
int mouseCol,mouseRow;
int colorIDToMove;
int previewDirX,previewDirY; // when I slide in a direction to preview what the move will result in
float previewAmount; // from 0 to 1.
float mouseDownX,mouseDownY;
float previewMoveOffsetX,previewMoveOffsetY;

// Objects
GridSpace[][] gridSpaces;
ArrayList tiles;

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
  mouseDraggingMath();
  previewMoveOffsetX = previewDirX * previewAmount * unitSize.x;
  previewMoveOffsetY = previewDirY * previewAmount * unitSize.y;
  
  UpdateColorIDToMove();
  
  // Grid back
  fill(128,28,220);
  noStroke();
  rect(gridX+gridSize.x*0.5,gridY+gridSize.y*0.5, gridSize.x+20,gridSize.y+20, rectRoundRadius);
  
  // Grid spaces
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row].draw();
    }
  }
  
  // Tiles!
  for (int i=tiles.size()-1; i>=0; --i) {
    Tile tempTile = (Tile) tiles.get(i);
    tempTile.draw();
  }
}



void UpdateMousePosGrid() {
  mouseCol = floor((mouseX-gridX) / unitSize.x);
  mouseRow = floor((mouseY-gridY) / unitSize.y);
}
void mouseDraggingMath() {
  if (mousePressed) {
    // Are we NOT yet previewing?
    if (!isPreviewMove()) {
      // If we've moved a few pixels from where we pressed down...
      if (sqrt((mouseDownX-mouseX)*(mouseDownX-mouseX) + (mouseDownY-mouseY)*(mouseDownY-mouseY)) > 5) {
        // Dragging HORIZONTALLY?
        if (abs(mouseX-mouseDownX) > abs(mouseY-mouseDownY)) {
          previewDirY = 0;
          if (mouseX < mouseDownX) previewDirX = -1;
          else previewDirX = 1;
        }
        // Dragging VERTICALLY?
        else {
          previewDirX = 0;
          if (mouseY < mouseDownY) previewDirY = -1;
          else previewDirY = 1;
        }
        determineWhatTilesCanMoveInPreview();
        // Reset mouseDownX and Y (a little hacky)
        //mouseDownX = mouseX;
        //mouseDownY = mouseY;
      }
    }
    // We ARE previewing the move?!
    else {
      // HORIZONTAL
      if (previewDirX != 0) {
        previewAmount = max(0, min(0.95, previewDirX * (mouseX-mouseDownX) / unitSize.x));
      }
      // VERTICAL
      else {
        previewAmount = max(0, min(0.95, previewDirY * (mouseY-mouseDownY) / unitSize.x));
      }
    }
  }
}
void UpdateColorIDToMove() {
  if (!mousePressed) { // If the mouse is UP, then update the value!
    Tile tileMouseOver = GetTile(mouseCol,mouseRow);
    if (tileMouseOver != null) {
      colorIDToMove = tileMouseOver.colorID;
    }
    else {
      colorIDToMove = -1;
    }
  }
}



void mousePressed() {
  resetPreviewDrag();
  mouseDownX = mouseX;
  mouseDownY = mouseY;
}
void mouseReleased() {
  // Did we drag the preview far enough to count it as a move?!
  if (previewAmount > 0.5) {
    // HACKISHLY bump the positions of the tiles for a smoother animation into place.
    for (int i=tiles.size()-1; i>=0; --i) {
      Tile tempTile = (Tile) tiles.get(i);
      if (tempTile.canMoveInPreview) {
        tempTile.x += previewMoveOffsetX;
        tempTile.y += previewMoveOffsetY;
      }
    }
    // Move the tiles!!
    MoveTiles(previewDirX,previewDirY, colorIDToMove);
  }
  resetPreviewDrag();
}

void keyPressed() {
  if (keyCode == ENTER) resetGame();
  
  if (keyCode == UP) MoveTiles(0, -1, colorIDToMove);
  else if (keyCode == DOWN) MoveTiles(0, 1, colorIDToMove);
  else if (keyCode == LEFT) MoveTiles(-1, 0, colorIDToMove);
  else if (keyCode == RIGHT) MoveTiles(1, 0, colorIDToMove);
  
  if (key == 'p') printGridSpaces();
}






