class BoardData {
  Spool[] spools;
  
  public BoardData(Spool[] _spools) {
    spools = new Spool[_spools.length];
    for (int i=0; i<_spools.length; i++) {
      spools[i] = _spools[i].clone();
    }
  }
  
}
