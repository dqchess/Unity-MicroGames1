// c_GridSpace


class GridSpace {
  int col,row;
  float x,y;
  private Well wellOnMe;
  
  GridSpace(int Col,int Row) {
    col = Col;
    row = Row;
    x = GridToX(col);
    y = GridToY(row);
    Reset();
  }
  void Reset() {
    wellOnMe = null;
  }
  
  boolean CanAddWellPath() {
    return wellOnMe == null;
  }
  void SetWellOnMe(Well _well) {
    wellOnMe = _well;
  }
  
  void draw() {
    fill(255, 64);
    noStroke();
    rect(x,y, unitSize.x,unitSize.y);
  }
  
}
