// CircleGrow level maker
// by Brett Taylor
// Started July 31, 2018
// For making levels for CircleGrow (which may have been renamed since this writing), a game within MicroGames1, planned for publishing with Simple Machine.



// Base Properties
PVector mousePos=new PVector(); // exact mouse pos
PVector mousePosGrab=new PVector(); // objs being grabbed use THIS value. It includes the grab offset. :)
PVector mouseGrabOffset=new PVector(); // set when we set objGrabbing. Used to calculate mousePosGrab.
// Properties
Rect levelBounds;
PVector levelCenter;
// Components
ArrayList growers;
// References
Grabbable objGrabbing;
Grabbable objOver;



void setup() {
  size(600,800);
  smooth();
  colorMode(HSB);
  frameRate(60);
  
  levelBounds = new Rect(25,25, 550,750);
  levelCenter = new PVector(width*0.5,height*0.5);
  
  ResetGrowers();
}

void draw() {
  // Update
  UpdateMousePos();
  UpdateObjOver();
  UpdateObjGrabbing();
  UpdateMouseCursor();
  
  // Draw
  background(255);
  DrawLevelBounds();
  DrawGrowers();
  DrawTotalGrowersValue();
}


// ----------------------------------------------------------------
//  Input Events
// ----------------------------------------------------------------
void keyPressed() {
  if (keyCode == ENTER) {
    ResetGrowers();
  }
//  else if (key == '[') { StartPrevLevel(); }
//  else if (key == ']') { StartNextLevel(); }

  else if (key == 'a') { AddGrower(mousePos.x, mousePos.y); }
  else if (keyCode==DELETE || keyCode==BACKSPACE) { DeleteGrowerOver(); }
  else if (key == 'p') { PrintGrowerCoordinates(); }
}


void mousePressed() {
  if (objOver != null) {
    SetObjGrabbing(objOver);
  }
}
void mouseReleased() {
  if (objGrabbing != null) {
    ReleaseObjGrabbing();
  }
}









