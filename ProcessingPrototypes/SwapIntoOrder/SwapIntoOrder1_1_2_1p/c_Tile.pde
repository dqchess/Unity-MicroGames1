// c_Tile


class Tile {
  int col,row;
  int colorID;
  int numberID;
  float x,y;
  float targetX,targetY;
  float textSize;
  float cardThickness; // higher value cards look thicker
  color bodyColor;
  color sideColor;
  color textColor;
  boolean canMoveInPreview;
  boolean isAtTargetPosition;
  
  Tile clone() {
    Tile cloneTile = new Tile(col,row, colorID,numberID);
    cloneTile.x = x;
    cloneTile.y = y;
    cloneTile.targetX = targetX;
    cloneTile.targetY = targetY;
    cloneTile.textSize = textSize;
    cloneTile.bodyColor = bodyColor;
    cloneTile.sideColor = sideColor;
    cloneTile.textColor = textColor;
    cloneTile.canMoveInPreview = canMoveInPreview;
    cloneTile.isAtTargetPosition = isAtTargetPosition;
    return cloneTile;
  }
  
  Tile(int Col,int Row, int ColorID,int NumberID) {
    col = Col;
    row = Row;
    SetColorID(ColorID);
    SetNumberID(NumberID);
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
//    bodyColor = BODY_COLORS[min(2,value-1)];
//    sideColor = SIDE_COLORS[min(2,value-1)];
    cardThickness = GetCardThicknessFromColorID(colorID);
    textColor = color(40);
    textSize = unitSize.x*0.5;
  }
  void SetNumberID(int numberID) {
    this.numberID = numberID;
  }
  
  
  void move(int dirX,int dirY) {
    if (isPreviewMove()) gridSpaces[col][row].canMoveMyTileInPreviewMove = true;
    
    removeTileFromItsGridSpace(this);
    col += dirX;
    row += dirY;
    addTileToItsGridSpace(this);
    setTargetXY();
  }
//  void moveAndMerge(int dirX,int dirY) {
//    if (isPreviewMove()) gridSpaces[col][row].canMoveMyTileInPreviewMove = true;
//    
//    doReplaceWhenReachTargetPos = true;
//    removeTileFromItsGridSpace(this);
//    col += dirX;
//    row += dirY;
//    setTargetXY();
//  }
  
  
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
  
  
  
  void draw() {
    // Update
    if (!isAtTargetPosition) {
      x += (targetX-x) / 3;
      y += (targetY-y) / 3;
      if (abs(x-targetX)<1 && abs(y-targetY)<1) {
        onReachTargetPosition();
      }
    }
    
    
    // Draw
    pushMatrix();
    //canMoveInPreview = true;
    
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
    rect(0,0, tileSize.x,tileSize.y, rectRoundRadius);
    // Body
    fill(bodyColor);
    noStroke();
    rectMode(CENTER);
    rect(0,-cardThickness, tileSize.x,tileSize.y-cardThickness*0.5, rectRoundRadius);
    
    fill(textColor);
    textAlign(CENTER, CENTER);
    textSize(textSize);
    text(numberID, 0,-cardThickness);
    
    popMatrix();
  }
  
  
}
