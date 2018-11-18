class BoardData {
  Well[] wells;
  
  public BoardData(Well[] _wells) {
    wells = new Well[_wells.length];
    for (int i=0; i<_wells.length; i++) {
      wells[i] = _wells[i].clone();
    }
  }
  
}
