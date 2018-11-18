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
  void RemoveWellOnMe() {
    wellOnMe = null;
  }
  public Well GetWellOnMe() {
    return wellOnMe;
  }
  
  void draw() {
    fill(255, 64);
    noStroke();
    rect(x,y, unitSize.x-3,unitSize.y-3);
  }
  
}
