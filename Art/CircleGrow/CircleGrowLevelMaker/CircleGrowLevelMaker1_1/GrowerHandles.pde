

abstract class Grabbable {
  // Properties
  PVector pos;
  // Functions
  abstract public boolean IsMouseOverMe();
  abstract public void UpdateAsObjGrabbing();
  
}

abstract class GrowerHandle extends Grabbable {
  // Properties
  protected float radius=5;
  protected PVector posLocal;
  // References
  protected Grower myGrower;
  
  // Getters
  public boolean IsMouseOverMe() {
    return PVector.dist(mousePos, pos) < radius+12; // expand my radius to make me easier to grab.
  }
  
  // ----------------------------------------------------------------
  //  Initialize
  // ----------------------------------------------------------------
  GrowerHandle(Grower myGrower) {
    this.myGrower = myGrower;
  }
  
  // ----------------------------------------------------------------
  //  Update
  // ----------------------------------------------------------------
  abstract public void UpdatePos();
  
  // ----------------------------------------------------------------
  //  Draw
  // ----------------------------------------------------------------
  public void Draw() {
    if (objGrabbing == this) {
      fill(24, 255, 230);
    }
    else if (objOver == this) {
      fill(40, 200, 255);
    }
    else {
      fill(128, 120);
    }
    stroke(0, 200);
    strokeWeight(1.5);
    ellipse(pos.x,pos.y, radius*2,radius*2);
  }
}


class RadiusHandle extends GrowerHandle {
  // ----------------------------------------------------------------
  //  Initialize
  // ----------------------------------------------------------------
  RadiusHandle(Grower myGrower) {
    super(myGrower);
  }
  
  
  // ----------------------------------------------------------------
  //  Doers
  // ----------------------------------------------------------------
  public void UpdatePos() {
    posLocal = new PVector(0, myGrower.radius+radius);
    pos = PVector.add(myGrower.pos, posLocal);
  }
  public void UpdateAsObjGrabbing() {
//    pos = mousePosGrab;
//    myLens.SetPos(PVector.sub(pos, posLocal));
    float dist = PVector.dist(myGrower.pos, mousePosGrab);
    myGrower.SetRadius(dist);
  }
}




























