class BoardData {
  Well[] wells;
  
  public BoardData(ArrayList _wells) {
    wells = new Well[_wells.size()];
    for (int i=0; i<_wells.size(); i++) {
      Well obj = (Well) _wells.get(i);
      wells[i] = obj.clone();
    }
  }
  
}
