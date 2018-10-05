// Swap-Into-Order
// started 10/5/2018

/* Additional game ideas

* "ShuntAndMerge". E.g. 4x4, 4 colors, 3 of each tile. They merge when touching.

*/


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
BoardData[] boardSnapshots;
boolean[] didMoveInCol;
boolean[] didMoveInRow;
int mouseCol,mouseRow;
int groupIDToMove;
int previewDirX,previewDirY; // when I slide in a direction to preview what the move will result in
float previewAmount; // from 0 to 1.
float mouseDownX,mouseDownY;
float previewMoveOffsetX,previewMoveOffsetY;

// Objects
GridSpace[][] gridSpaces;
ArrayList tiles;
Tile[][] tileGroups;

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
  
  UpdateGroupIDToMove();
  
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
    tempTile.Update();
    tempTile.Draw();
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
      // TEST: If we've previewed far enough, count the move!
      if (previewAmount > 0.94) {
        mouseReleased();
        mousePressed();
      }
    }
  }
}
void UpdateGroupIDToMove() {
  if (!mousePressed) { // If the mouse is UP, then update the value!
    Tile tileMouseOver = GetTile(mouseCol,mouseRow);
    if (tileMouseOver != null) {
      groupIDToMove = tileMouseOver.groupID;
    }
    else {
      groupIDToMove = -1;
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
    MoveTiles(previewDirX,previewDirY, groupIDToMove, false);
  }
  resetPreviewDrag();
}

void keyPressed() {
  if (keyCode == ENTER) resetGame();
  
  if (keyCode == UP) MoveTiles(0, -1, groupIDToMove, false);
  else if (keyCode == DOWN) MoveTiles(0, 1, groupIDToMove, false);
  else if (keyCode == LEFT) MoveTiles(-1, 0, groupIDToMove, false);
  else if (keyCode == RIGHT) MoveTiles(1, 0, groupIDToMove, false);
  
  else if (key == 'z') UndoMove();
  else if (key == 'x') RedoMove();
  
  else if (key == 'p') printGridSpaces();
}






