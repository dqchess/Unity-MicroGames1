
void UpdateMousePos() {
  mousePos = new PVector(mouseX-levelCenter.x, mouseY-levelCenter.y);
  mousePosGrab = PVector.add(mousePos, mouseGrabOffset);
}



void UpdateObjOver() {
  objOver = null;
  if (objGrabbing != null) { return; } // If we're GRABBING something, DON'T set objOver!
  // First try the HANDLES!
  for (int i=growers.size()-1; i>=0; --i) {
    Grower grower = (Grower) growers.get(i);
    if (grower.radiusHandle.IsMouseOverMe()) {
      SetObjOver(grower.radiusHandle);
      return;
    }
  }
  // Now, try the actual GROWERS themselves!
  for (int i=growers.size()-1; i>=0; --i) {
    Grower grower = (Grower) growers.get(i);
    if (grower.IsMouseOverMe()) {
      SetObjOver(grower);
      return;
    }
  }
}

void UpdateObjGrabbing() {
  if (objGrabbing != null) {
    objGrabbing.UpdateAsObjGrabbing();
  }
}

void UpdateMouseCursor() {
  if (objGrabbing != null) {
    cursor(MOVE);
  }
  else if (objOver != null) {
    cursor(HAND);
  }
  else {
    cursor(ARROW);
  }
}
