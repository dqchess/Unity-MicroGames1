class GridSpace {
  int col,row;
  int colorID;
  int numberID;
  int groupID; // which group of colors I'm a part of, as the index in the 2D array "groups"
  float xDisplay,yDisplay;
  float targetXDisplay,targetYDisplay;
  float easeSpeed = 3; // default 3. Higher is slower.
  private color fillColor;
  private color strokeColor;
  
  GridSpace(int Col,int Row) {//, int ColorID) {
    col = Col;
    row = Row;
    targetXDisplay = boardPosX + (col+0.5)*unitSize;
    targetYDisplay = boardPosY + (row+0.5)*unitSize;
    xDisplay = targetXDisplay;
    yDisplay = targetYDisplay;
    SetColorID(-1); // default to -1!
    SetNumberID(-1); // default to -1!
  }
  
  public void SetColorID(int colorID) {
    this.colorID = colorID;
    fillColor = GetFillColor(colorID);
    strokeColor = GetStrokeColor(colorID);
  }
  public void SetNumberID(int numberID) {
    this.numberID = numberID;
  }
  
  
  void Update() {
    if (spaceSelected == this) {
      if (abs(mouseX-targetXDisplay) > abs(mouseY-targetYDisplay)) {
        if ((mouseX>targetXDisplay && canSwap_R) || (mouseX<targetXDisplay && canSwap_L)) {
          draggingUnitDistanceFromOrigin = (mouseX-targetXDisplay)/unitSize;
          xDisplay = targetXDisplay + draggingUnitDistanceFromOrigin*unitSize * 1;
          xDisplay = max(targetXDisplay-unitSize*0.9, min(targetXDisplay+unitSize*0.9, xDisplay));
          if (mouseX>targetXDisplay && canSwap_R) spaceSwappingWith = spaces[col+1][row];
          else if (canSwap_L) spaceSwappingWith = spaces[col-1][row];
        }
        yDisplay += (targetYDisplay-yDisplay) / 1;
      }
      else {
        if ((mouseY>targetYDisplay && canSwap_D) || (mouseY<targetYDisplay && canSwap_U)) {
          draggingUnitDistanceFromOrigin = (mouseY-targetYDisplay)/unitSize;
          yDisplay = targetYDisplay + draggingUnitDistanceFromOrigin*unitSize * 1;
          yDisplay = max(targetYDisplay-unitSize*0.9, min(targetYDisplay+unitSize*0.9, yDisplay));
          if (mouseY>targetYDisplay && canSwap_D) spaceSwappingWith = spaces[col][row+1];
          else if (canSwap_U) spaceSwappingWith = spaces[col][row-1];
        }
        xDisplay += (targetXDisplay-xDisplay) / 1;
      }
    }
    else {
        xDisplay += (targetXDisplay-xDisplay) / easeSpeed;
        yDisplay += (targetYDisplay-yDisplay) / easeSpeed;
    }
  }
  
  void Draw() {
//    // Is another space filling this one?
//    if (spaceSelected!=null && groupID==spaceSelected.groupID && spaceSwappingWith!=null) {
//      float transparency = abs(draggingUnitDistanceFromOrigin)*255;
//      fill(TILE_FILLS[spaceSwappingWith.colorID], transparency);
//      noStroke();
//      rect(targetXDisplay,targetYDisplay, unitSize,unitSize);
//    }
//    if (spaceSwappingWith!=null && groupID==spaceSwappingWith.groupID) {
//      float transparency = abs(draggingUnitDistanceFromOrigin)*255;
//      fill(TILE_FILLS[spaceSelected.colorID], transparency);
//      noStroke();
//      rect(targetXDisplay,targetYDisplay, unitSize,unitSize);
//    }
    
    // Draw!
    float myDiameter = (tileDiameter) * (spaceSelected==this ? 0.9: 1);
    float alpha = 1;
    if ((spaceSwappingWith!=null && groupID==spaceSwappingWith.groupID) || (spaceSelected!=null&&this!=spaceSelected&&groupID==spaceSelected.groupID)) {
      alpha = 1-abs(draggingUnitDistanceFromOrigin);
    }
    float transparency = alpha*255;
    fill(fillColor, transparency);
    stroke(strokeColor, transparency);
    strokeWeight(2);
    rect(xDisplay,yDisplay, myDiameter,myDiameter);
    
    // Text
    if (numberID >= 0) {
      fill(0, 120);
      textAlign(CENTER, CENTER);
      textSize(myDiameter*0.7);
      text(numberID, xDisplay,yDisplay);
    }
  }
  
}
