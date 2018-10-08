// DanceSync1_1
// started 10/5/2018


// Characters
Dancer[] dancers;
// Properties
boolean isPaused=false;
float groundY = 100;
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
  
  stageWidth = width*0.6;
  stageX = (width-stageWidth)*0.5;
  colWidth = stageWidth/(float)numDancers;
  
  dancers = new Dancer[numDancers];
  for (int i=0; i<numDancers; i++) {
    float posX = (i+0.5)*colWidth + stageX;
    float bounceApexY = 300;//random(300,360);
    dancers[i] = new Dancer(i, posX, bounceApexY);
  }
}



void draw() {
  background(32);
  
  Update();
  Draw();
}



void keyPressed() {
  if (keyCode==ENTER || keyCode==RETURN) {
    ResetLevel();
  }
  if (key == ' ') {
    isPaused = !isPaused;
  }
}








