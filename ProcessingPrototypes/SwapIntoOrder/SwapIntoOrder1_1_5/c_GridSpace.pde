// c_GridSpace


class GridSpace {
  int col,row;
  float x,y;
  private Tile myTile;
  boolean canMoveMyTileInPreviewMove;
  
  GridSpace(int Col,int Row) {
    col = Col;
    row = Row;
    x = getGridPosX(col);
    y = getGridPosY(row);
    Reset();
  }
  void Reset() {
    myTile = null;
    canMoveMyTileInPreviewMove = false;
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
  }
  
}
