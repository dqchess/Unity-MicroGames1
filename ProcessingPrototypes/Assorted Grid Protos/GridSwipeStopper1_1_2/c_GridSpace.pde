// c_GridSpace


class GridSpace {
  int col,row;
  float x,y;
  private Tile myTile;
  boolean canMoveMyTileInPreviewMove;
  float myTilePreviewXDisplay,myTilePreviewYDisplay; // NOTE: Cheap workaround for method that DOESN'T clone the Board. (Cloning would be way easier.)
  
  boolean IsOpen() {
    return myTile==null && !IsStopper();
  }
  boolean IsStopper() {
    return col==stopperCol && row==stopperRow;
  }
  
  GridSpace(int Col,int Row) {
    col = Col;
    row = Row;
    x = getGridPosX(col);
    y = getGridPosY(row);
    Reset();
  }
  void Reset() {
    myTile = null;
    ResetPreview();
  }
  void ResetPreview() {
    canMoveMyTileInPreviewMove = false;
    myTilePreviewXDisplay = x;
    myTilePreviewYDisplay = y;
  }
  
  Tile getTile() {
    //if (isPreviewMove()) return myPreviewTile;
    //else 
    return myTile;
  }
  void setTile(Tile tile) {
    //if (isPreviewMove()) myPreviewTile = tile;
    //else 
    myTile = tile;
    //myTile = myPreviewTile = tile;
  }
  
  void draw() {
    fill(255, 64);
    noStroke();
    rect(x,y, tileSize.x+0,tileSize.y+0, rectRoundRadius);
    
    // Stopper image!
    if (IsStopper()) {
      fill(128);
      noStroke();
      ellipse(x,y, unitSize.x*0.8,unitSize.y*0.8);
    }
  }
  
}
