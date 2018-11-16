// c_Tile


class Tile {
  int col,row;
  int colorID;
  int value;
  float x,y;
  float targetX,targetY;
  float textSize;
  boolean isHighlighted;
  private float highlightLoc;
  color bodyColor;
  color sideColor;
  color textColor;
  boolean canMoveInPreview;
  boolean isAtTargetPosition;
  
  Tile clone() {
    Tile cloneTile = new Tile(col,row, colorID, value);
    cloneTile.x = x;
    cloneTile.y = y;
    cloneTile.colorID = colorID;
    cloneTile.value = value;
    cloneTile.targetX = targetX;
    cloneTile.targetY = targetY;
    cloneTile.textSize = textSize;
    cloneTile.bodyColor = bodyColor;
    cloneTile.sideColor = sideColor;
    cloneTile.textColor = textColor;
    cloneTile.highlightLoc = highlightLoc;
    cloneTile.canMoveInPreview = canMoveInPreview;
    cloneTile.isAtTargetPosition = isAtTargetPosition;
    return cloneTile;
  }
  
  Tile(int Col,int Row, int ColorID, int Value) {
    col = Col;
    row = Row;
    SetColorID(ColorID);
    value = Value;
    setTargetXY();
  }
  void setTargetXY() {
    targetX = getGridPosX(col);
    targetY = getGridPosY(row);
    isAtTargetPosition = false;
  }
  void SetColorID(int colorID) {
    this.colorID = colorID;
    bodyColor = GetFillColor(colorID);
    sideColor = GetStrokeColor(colorID);
    textColor = color(0, 200);
    textSize = unitSize.x*0.5;
  }
  
  
  void move(int dirX,int dirY) {
    if (isPreviewMove()) gridSpaces[col][row].canMoveMyTileInPreviewMove = true;
    
    removeTileFromItsGridSpace(this);
    col += dirX;
    row += dirY;
    addTileToItsGridSpace(this);
    setTargetXY();
  }
  
  public void GoToTargetPos() {
    onReachTargetPosition();
  }
  private void onReachTargetPosition() {
    isAtTargetPosition = true;
    x = targetX;
    y = targetY;
//    // Merge?!
//    if (doReplaceWhenReachTargetPos) {
//      Tile otherTile = GetTile(col,row);
//      otherTile.setValue(this.value + otherTile.value);
//      removeTile(this); // remove the remaining tile at this grid space
//      addTileToItsGridSpace(otherTile);
//    }
  }
  
  
  void Update() {
    UpdatePos();
    UpdateHighlightValues();
  }
  private void UpdatePos() {
    if (!isAtTargetPosition) {
      x += (targetX-x) / 3;
      y += (targetY-y) / 3;
      if (abs(x-targetX)<1 && abs(y-targetY)<1) {
        onReachTargetPosition();
      }
    }
  }
  private void UpdateHighlightValues() {
//    isHighlighted = groupIDToMove == groupID;
//    float highlightLocTarget = isHighlighted ? 1 : 0;
//    if (highlightLoc != highlightLocTarget) {
//      highlightLoc += (highlightLocTarget-highlightLoc) * 0.4;
//    }
  }
  
  void Draw() {
    pushMatrix();
    //canMoveInPreview = true;
    float cardThickness = 7 - (highlightLoc*5);
    
    float xDisplay = x;
    float yDisplay = y;
    if (canMoveInPreview) {
      xDisplay += previewMoveOffsetX;
      yDisplay += previewMoveOffsetY;
    }
    translate(xDisplay,yDisplay);
    
    // Base
    fill(sideColor);
    noStroke();
    rectMode(CENTER);
    rect(0,0, tileSize.x,tileSize.y);
    // Body
    fill(bodyColor);
    noStroke();
    rectMode(CENTER);
    rect(0,-cardThickness, tileSize.x,tileSize.y-cardThickness*0.5);
    // Text
    fill(0, 120);
    textAlign(CENTER, CENTER);
    textSize(textSize);
    text(value, 0,-cardThickness);
    
    // Highlight
    if (isHighlighted) {
      fill(255, highlightLoc*30);
      rect(0,-cardThickness*0.5, tileSize.x,tileSize.y+cardThickness);
    }
    
    popMatrix();
  }
  
  
}
