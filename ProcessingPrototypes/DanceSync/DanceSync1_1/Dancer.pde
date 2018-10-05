class Dancer {
  // Properties
  PVector pos;
  PVector size;
  boolean isFreezingMe; // true when isHighlightingMe AND mousePressed! When true, we don't move.
  boolean isHighlightingMe; // true when mouse over my column.
  float velY;
  int index;
  float gravity = -0.3;
  float bounceApexY;
  
  
  // ----------------------------------------------------------------
  //  Initialize
  // ----------------------------------------------------------------
  public Dancer(int index, float posX) {//, float startingLocOffset) {
    this.index = index;
    this.pos = new PVector(posX, groundY+random(0, height));
    //this.locOffset = startingLocOffset;
    this.size = new PVector(30,50);
    this.bounceApexY = 300;//random(200, 300);
  }
  
  
  // ----------------------------------------------------------------
  //  Update
  // ----------------------------------------------------------------
  void Update() {
    UpdateInteractionBools();
    UpdateApplyVel();
    ApplyBounds();
  }
  private void UpdateInteractionBools() {
    isHighlightingMe = mousePosCol == index;
    isFreezingMe = isHighlightingMe && mousePressed;
  }
  private void UpdateApplyVel() {
    if (!isFreezingMe) {
      velY += gravity;
      pos.y += velY;
    }
  }
  private void ApplyBounds() {
    if (pos.y <= groundY) {
      Bounce();
    }
  }
  private void Bounce() {
    pos.y = groundY;
    float distToApex = bounceApexY - pos.y;
    velY = sqrt(2*-gravity*distToApex); // 0 = y^2 + 2*g*dist  ->  y = sqrt(2*g*dist)
  }
  
  
  
  // ----------------------------------------------------------------
  //  Draw
  // ----------------------------------------------------------------
  void Draw() {
    DrawColHighlight();
    DrawBody();
  }
  
  void DrawColHighlight() {
    // Mouse is in my column!
    if (isHighlightingMe) {
      if (isFreezingMe) fill(255, 50);
      else fill(255, 24);
      noStroke();
      rect(pos.x-colWidth*0.5,0, colWidth,height);
    }
  }
  void DrawBody() {
    fill(255, 180);
    stroke(255, 220);
    strokeWeight(2);
    rect(pos.x-size.x*0.5,pos.y, size.x,size.y);
  }
  
  
}







