class GridSpace {
  int col,row;
  int colorID;
  int groupID; // which group of colors I'm a part of, as the index in the 2D array "groups"
  float xDisplay,yDisplay;
  float targetXDisplay,targetYDisplay;
  float easeSpeed = 3; // default 3. Higher is slower.
  
  GridSpace(int Col,int Row, int ColorID) {
    col = Col;
    row = Row;
    colorID = ColorID;
    targetXDisplay = boardPosX + (col+0.5)*unitSize;
    targetYDisplay = boardPosY + (row+0.5)*unitSize;
    xDisplay = targetXDisplay;
    yDisplay = targetYDisplay;
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
    // Is another space filling this one?
    if (spaceSelected!=null && groupID==spaceSelected.groupID && spaceSwappingWith!=null) {
      float transparency = abs(draggingUnitDistanceFromOrigin)*255;
      fill(TILE_FILLS[spaceSwappingWith.colorID], transparency);
      noStroke();
      rect(targetXDisplay,targetYDisplay, unitSize,unitSize);
    }
    if (spaceSwappingWith!=null && groupID==spaceSwappingWith.groupID) {
      float transparency = abs(draggingUnitDistanceFromOrigin)*255;
      fill(TILE_FILLS[spaceSelected.colorID], transparency);
      noStroke();
      rect(targetXDisplay,targetYDisplay, unitSize,unitSize);
    }
    
    // Draw!
    float myDiameter = (tileDiameter) * (spaceSelected==this ? 0.9: 1);
    // Body!
    float alpha = 1;
    if ((spaceSwappingWith!=null && groupID==spaceSwappingWith.groupID) || (spaceSelected!=null&&this!=spaceSelected&&groupID==spaceSelected.groupID)) {
      alpha = 1-abs(draggingUnitDistanceFromOrigin);
    }
    float transparency = alpha*255;
    fill(TILE_FILLS[colorID], transparency);
    stroke(TILE_STROKES[colorID], transparency);
    strokeWeight(2);
    rect(xDisplay,yDisplay, myDiameter,myDiameter);
    // Highlight if part of dragging group!
    if (false) {
      fill(255, 0.3*transparency);
      noStroke();
      rect(xDisplay,yDisplay, myDiameter,myDiameter);
    }
  }
  
}
