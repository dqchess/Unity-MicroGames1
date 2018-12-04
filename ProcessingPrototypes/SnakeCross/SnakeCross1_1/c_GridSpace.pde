// c_GridSpace


class GridSpace {
  int col,row;
  float x,y;
  private Spool spoolOnMe;
  
  GridSpace(int Col,int Row) {
    col = Col;
    row = Row;
    x = GridToX(col);
    y = GridToY(row);
    Reset();
  }
  void Reset() {
    spoolOnMe = null;
  }
  
  boolean CanAddSpoolPath() {
    return spoolOnMe == null;
  }
  void SetSpoolOnMe(Spool _spool) {
    spoolOnMe = _spool;
  }
  void RemoveSpoolOnMe() {
    spoolOnMe = null;
  }
  public Spool GetSpoolOnMe() {
    return spoolOnMe;
  }
  
  void draw() {
    fill(255, 64);
    noStroke();
    rect(x,y, unitSize.x-3,unitSize.y-3);
  }
  
}
