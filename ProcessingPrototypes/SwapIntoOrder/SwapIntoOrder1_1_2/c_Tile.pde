class Tile {
  // Properties
  int colorID;
  int col,row=-1;
  float timeUntilDisappear=-1; // in SECONDS, the time until I DIE, but with less aggressive wording.
  boolean isDead; // this is true when a tile gets matched and fades out
  // Visuals
  float xDisplay,yDisplay;
  float targetXDisplay,targetYDisplay;
  float easeSpeed = 3; // default 3. Higher is slower.
  float scale = 0; // for animating in new tiles
  
  Tile(int Col,int Row, int ColorID) {
    col = Col;
    row = Row;
    colorID = ColorID;
    
    calculateTargetDisplays();
    xDisplay = targetXDisplay;
    yDisplay = targetYDisplay;
  }
  
  void update(float deltaTime) {
    // Ease to display location!
    xDisplay += (targetXDisplay-xDisplay) / easeSpeed;
    yDisplay += (targetYDisplay-yDisplay) / easeSpeed;
    // Grow if I'm not full-size yet!
    if (scale!=1) {
      scale += (1-scale)/4;
      if (scale>0.99) scale = 1;
    }
    // Maybe die!
    if (timeUntilDisappear >= 0) {
      timeUntilDisappear -= deltaTime;
      if (timeUntilDisappear <= 0) {
        isDead = true;
      }
    }
  }
  
  
  
  
  
  
  private void calculateTargetDisplays() {
    targetXDisplay = boardPosX + (col+0.5)*unitSize;
    targetYDisplay = boardPosY + (row+0.5)*unitSize;
  }
  //boolean isFirstGen() { return value<=1; }
  //private float calculateTargetXDisplay() { return boardPosX + (col+0.5)*unitSize; }
  //private float calculateTargetYDisplay() { return boardPosY + (row+0.5)*unitSize; }
  
  
}




