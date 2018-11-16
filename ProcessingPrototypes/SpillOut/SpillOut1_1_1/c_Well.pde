class Well {
  int col,row;
  int colorID;
  int numSpacesToFill;
  PVector corePos;
  float textSize;
  Vector2Int[] pathSpaces;
  boolean isHighlighted;
  private float highlightLoc;
  color bodyColor;
  color sideColor;
  color textColor;
  
  // Getters
  private Vector2Int lastPathSpace() {
    return pathSpaces[pathSpaces.length-1];
  }
  Well clone() {
    Well clone = new Well(col,row, pathSpaces, colorID, numSpacesToFill);
    clone.corePos = new PVector(corePos.x,corePos.y);
    clone.textSize = textSize;
    clone.bodyColor = bodyColor;
    clone.sideColor = sideColor;
    clone.textColor = textColor;
    clone.highlightLoc = highlightLoc;
    return clone;
  }
  
  Well(int Col,int Row, Vector2Int[] pathSpaces, int ColorID, int Value) {
    col = Col;
    row = Row;
    pathSpaces = CopyVector2Int(pathSpaces);
    if (pathSpaces == null) { // Convenience: Pass in null to default to just empty path.
      pathSpaces = new Vector2Int[1];
      pathSpaces[0] = new Vector2Int(col,row);
    }
    SetColorID(ColorID);
    value = Value;
    corePos = GridToPos(col,row);
  }
  void SetColorID(int colorID) {
    this.colorID = colorID;
    bodyColor = GetFillColor(colorID);
    sideColor = GetStrokeColor(colorID);
    textColor = color(0, 180);
    textSize = unitSize.x*0.5;
  }
  
  void AddPathSpace(Vector2Int spacePos) {
    pathSpaces = (Vector2Int[]) append(pathSpaces, spacePos);
  }
  void RemovePathSpace() {
    if (pathSpaces.length < 2) { return; } // Safety check!
    pathSpaces = (Vector2Int[]) shorten(pathSpaces);
  }
  
  
  void Update() {
    UpdateHighlightValues();
  }
  private void UpdateHighlightValues() {
//    isHighlighted = groupIDToMove == groupID;
//    float highlightLocTarget = isHighlighted ? 1 : 0;
//    if (highlightLoc != highlightLocTarget) {
//      highlightLoc += (highlightLocTarget-highlightLoc) * 0.4;
//    }
  }
  
  void Draw() {
    DrawPath();
    DrawCore();
    DrawEndHighlight();
  }
  private void DrawPath() {
    // Path!
    noFill();
    stroke(bodyColor);
    strokeWeight(unitSize.x*0.6);
    beginShape();
    for (int i=0; i<pathSpaces.length; i++) {
      vertex(GridToX(pathSpaces[i].x), GridToY(pathSpaces[i].y));
    }
    endShape();
  }
  private void DrawCore() {
    pushMatrix();
    translate(corePos.x,corePos.y);
    
    float cardThickness = 7 - (highlightLoc*5);
    
    // Base
    fill(sideColor);
    noStroke();
    rectMode(CENTER);
    rect(0,0, unitSize.x,unitSize.y);
    // Body
    fill(bodyColor);
    noStroke();
    rectMode(CENTER);
    rect(0,-cardThickness, unitSize.x,unitSize.y-cardThickness*0.5);
    // Text
    fill(0, 120);
    textAlign(CENTER, CENTER);
    textSize(textSize);
    text(numSpacesLeft, 0,-cardThickness);
    
    popMatrix();
  }
  private void DrawEndHighlight() {
    if (isHighlighted) {
      fill(255, highlightLoc*30);
      PVector lastPos = GridToPos(lastPathSpace());
      rect(lastPos.x,lastPos.y-cardThickness*0.5, unitSize.x,unitSize.y+cardThickness);
    }
  }
  
  
}
