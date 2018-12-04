// c_GridSpace


class GridSpace {
  final int MaxSpoolsOnMe = 3; // anything past this would be lunacy.
  int col,row;
  float x,y;
  private Spool[] spoolsOnMe;
  
  GridSpace(int Col,int Row) {
    col = Col;
    row = Row;
    x = GridToX(col);
    y = GridToY(row);
    Reset();
  }
  void Reset() {
    spoolsOnMe = new Spool[0];
  }
  
  
  int NumSpoolsOnMe() { return spoolsOnMe.length; }
  boolean CanAddSpoolPath() {
    return NumSpoolsOnMe() < MaxSpoolsOnMe;
  }
  void AddSpoolOnMe(Spool _spool) {
    spoolsOnMe = (Spool[]) append(spoolsOnMe, _spool);
  }
  void RemoveSpoolOnMe() {
    spoolsOnMe = (Spool[]) shorten(spoolsOnMe);
  }
  public Spool GetTopSpoolOnMe() {
    if (NumSpoolsOnMe() == 0) { return null; }
    return spoolsOnMe[spoolsOnMe.length-1];
  }
  
  void draw() {
    fill(255, 64);
    noStroke();
    rect(x,y, unitSize.x-3,unitSize.y-3);
  }
  
}
