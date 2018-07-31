class Grower extends Grabbable {
  // Constants
  private final float RADIUS_MIN = 20;
  private final float RADIUS_MAX = 400;
  // Properties
  float radius;
  // Components
  RadiusHandle radiusHandle;
  
  // Getters
  public boolean IsMouseOverMe() {
//    return mousePos.x>pos.x-radius && mousePos.x<pos.x+radius  &&  mousePos.y>pos.y-radius && mousePos.y<pos.y+radius; // squares!
    return PVector.dist(mousePos, pos) < radius; // circles!
  }
  private float Area() { return PI * radius*radius; }
  public int ScoreValue() {
    return (int)(Area()/100.0);
  }
  
  // ----------------------------------------------------------------
  //  Initialize
  // ----------------------------------------------------------------
  public Grower(float x,float y, float radius) {
    radiusHandle = new RadiusHandle(this);
    SetPos(x,y);
    SetRadius(radius);
  }
  
  // ----------------------------------------------------------------
  //  Setters
  // ----------------------------------------------------------------
  private void UpdateHandlePoses() {
    radiusHandle.UpdatePos();
  }
  void SetPos(float x,float y) { SetPos(new PVector(x,y)); }
  void SetPos(PVector pos) { 
    this.pos = pos;
    UpdateHandlePoses();
  }
  void SetRadius(float radius) {
    this.radius = max(RADIUS_MIN, min(RADIUS_MAX, radius)); // clamp it, Steve!
    UpdateHandlePoses();
  }
  
  
  // ----------------------------------------------------------------
  //  Doers
  // ----------------------------------------------------------------
  public void UpdateAsObjGrabbing() {
    SetPos(mousePosGrab);
  }
  
  
  // ----------------------------------------------------------------
  //  Draw
  // ----------------------------------------------------------------
  void Draw() {
    DrawBody();
    radiusHandle.Draw();
  }
  private void DrawBody() {
    // Circle
    ellipseMode(CENTER);
    noFill();
    float diameter = radius*2;
    if (objGrabbing == this) { stroke(24, 255, 250); }
    else if (objOver == this) { stroke(36, 220, 255); }
    else { stroke(0,140); }
    strokeWeight(4);
    ellipse(pos.x,pos.y, diameter,diameter);
    
    // Area
    textAlign(CENTER, CENTER);
    textSize(32);
    fill(0, 80);
    text(ScoreValue(), pos.x,pos.y);
  }
  
}




