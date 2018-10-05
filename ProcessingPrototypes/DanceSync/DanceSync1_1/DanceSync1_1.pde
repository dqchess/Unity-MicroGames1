// DanceSync1_1
// started 10/5/2018


// Characters
Dancer[] dancers;
// Properties
float groundY = 0;
float colWidth;
float stageWidth;
float stageX; // the left pos of the stage.
int mousePosCol; // which Dancer column the mouse is in.



void setup() {
  size(800,600);
  smooth();
  frameRate(60);
  colorMode(HSB);
  
  ResetLevel();
}

void ResetLevel() {
  int numDancers = 4;
  
  stageWidth = width;//*0.5;
  stageX = (width-stageWidth)*0.5;
  colWidth = stageWidth/(float)numDancers;
  
  dancers = new Dancer[numDancers];
  for (int i=0; i<numDancers; i++) {
    float posX = (i+0.5)*colWidth + stageX;
    dancers[i] = new Dancer(i, posX);
  }
}



void draw() {
  background(32);
  
  Update();
  Draw();
}



private void Update() {
  UpdateDancerMouseOver();
}
private void UpdateDancerMouseOver() {
  mousePosCol = (int)floor((stageX+mouseX)/colWidth);
}


private void Draw() {
  // Translate into Unity coordinates! 0,0 is BOTTOM-left.
  pushMatrix();
  translate(0,height);
  scale(1, -1);
  
  for (int i=0; i<dancers.length; i++) {
    dancers[i].Update();
    dancers[i].Draw();
  }
  
  popMatrix();
}



void keyPressed() {
  if (keyCode==ENTER || keyCode==RETURN) {
    ResetLevel();
  }
}








