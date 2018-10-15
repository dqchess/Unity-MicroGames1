// c_Tile


class Tile {
  int col,row;
  int colorID;
  int numberID;
  float x,y;
  float targetX,targetY;
  float textSize;
  boolean isHighlighted;
  private float highlightLoc;
//  float cardThickness; // higher value cards look thicker
  color bodyColor;
  color sideColor;
  color textColor;
  boolean canMoveInPreview;
  boolean isAtTargetPosition;
  float previewXDisplay,previewYDisplay;
  
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
    cloneTile.highlightLoc = highlightLoc;
    cloneTile.canMoveInPreview = canMoveInPreview;
    cloneTile.isAtTargetPosition = isAtTargetPosition;
    cloneTile.previewXDisplay = previewXDisplay;
    cloneTile.previewYDisplay = previewYDisplay;
    return cloneTile;
  }
  
  Tile(int Col,int Row, int ColorID,int NumberID) {
    col = Col;
    row = Row;
    SetColorID(ColorID);
    SetNumberID(NumberID);
    SetTargetPos();
    GoToTargetPos();
  }
  void SetTargetPos() {
    targetX = getGridPosX(col);
    targetY = getGridPosY(row);
    isAtTargetPosition = false;
  }
  void SetColorID(int colorID) {
    this.colorID = colorID;
    bodyColor = GetFillColor(colorID);
    sideColor = GetStrokeColor(colorID);
//    cardThickness = GetCardThicknessFromColorID(colorID);
    textColor = color(0, 200);
    textSize = unitSize.x*0.5;
  }
  void SetNumberID(int numberID) {
    this.numberID = numberID;
  }
  void GoToPreviewPos() {
    if (canMoveInPreview) {
      x = previewXDisplay;
      y = previewYDisplay;
    }
  }
  
  void move(int dirX,int dirY) {
    GridSpace prevSpace = gridSpaces[col][row];
    
    removeTileFromItsGridSpace(this);
//    while (true) { // Keep moving until we can't anymore!
      col += dirX;
      row += dirY;
      GridSpace nextSpace = GetSpace(col+dirX,row+dirY);
//      if (nextSpace==null || !nextSpace.IsOpen()) { break; }
//    }
    addTileToItsGridSpace(this);
    SetTargetPos();
    
    if (isPreviewMove()) {
      prevSpace.canMoveMyTileInPreviewMove = true;
      prevSpace.myTilePreviewXDisplay = targetX;
      prevSpace.myTilePreviewYDisplay = targetY;
    }
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
    GoToTargetPos();
//    // Merge?!
//    if (doReplaceWhenReachTargetPos) {
//      Tile otherTile = GetTile(col,row);
//      otherTile.setValue(this.value + otherTile.value);
//      removeTile(this); // remove the remaining tile at this grid space
//      addTileToItsGridSpace(otherTile);
//    }
  }
  private void GoToTargetPos() {
    isAtTargetPosition = true;
    x = targetX;
    y = targetY;
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
    isHighlighted = false;
    float highlightLocTarget = isHighlighted ? 1 : 0;
    if (highlightLoc != highlightLocTarget) {
      highlightLoc += (highlightLocTarget-highlightLoc) * 0.4;
    }
  }
  
  void Draw() {
    pushMatrix();
    //canMoveInPreview = true;
    float cardThickness = tileSize.y*0.05;// - (highlightLoc*10);
    
    float xDisplay = x;
    float yDisplay = y;
    if (canMoveInPreview) {
      xDisplay = lerp(x,previewXDisplay, previewLoc);
      yDisplay = lerp(y,previewYDisplay, previewLoc);
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
    
//    // Text
//    fill(textColor);
//    textAlign(CENTER, CENTER);
//    textSize(textSize);
//    text(numberID, 0,-cardThickness);
    
    // Highlight
    if (isHighlighted) {
      fill(255, highlightLoc*30);
      rect(0,-cardThickness*0.5, tileSize.x,tileSize.y+cardThickness, rectRoundRadius);
    }
    
    popMatrix();
  }
  
  
}
