class BoardData {
  Tile[] tiles;
  
  public BoardData(ArrayList _tiles) {
    tiles = new Tile[_tiles.size()];
    for (int i=0; i<_tiles.size(); i++) {
      Tile tempTile = (Tile) _tiles.get(i);
      tiles[i] = tempTile.clone();
    }
  }
  
}
