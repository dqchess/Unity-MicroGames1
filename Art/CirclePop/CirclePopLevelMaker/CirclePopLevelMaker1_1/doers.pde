


void AddGrower(float x,float y) {
  Grower newObj = new Grower(x,y, 50);
  growers.add(newObj);
}
void DeleteGrowerOver() {
  if (objOver!=null && objOver.getClass() == Grower.class) {
    Grower growerGrabbing = (Grower)objOver;
    growers.remove(growerGrabbing);
  }
}


void SetObjOver(Grabbable obj) {
  objOver = obj;
}


void SetObjGrabbing(Grabbable obj) {
  objGrabbing = obj;
  mouseGrabOffset = PVector.sub(obj.pos, mousePos);
  // If it's a Grower, move it to the front of the array!
  if (objGrabbing.getClass() == Grower.class) {
    Grower growerGrabbing = (Grower)objGrabbing;
    PutGrowerAtBackOfList(growerGrabbing);
  }
}
void ReleaseObjGrabbing() {
  objGrabbing = null;
}

void PutGrowerAtBackOfList(Grower grower) {
  growers.remove(grower);
  growers.add(0, grower);
}
void PutGrowerAtFrontOfList(Grower grower) {
  growers.remove(grower);
  growers.add(grower);
}


void PrintGrowerCoordinates() {
  println("Grower Coordinates:");
  for (int i=0; i<growers.size(); i++) {
    Grower obj = (Grower) growers.get(i);
    println((int)obj.pos.x, (int)obj.pos.y);
  }
}



