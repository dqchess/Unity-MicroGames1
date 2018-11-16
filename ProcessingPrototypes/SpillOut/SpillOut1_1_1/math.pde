

class Vector2Int {
  int x,y;
  Vector2Int(int _x,int _y) {
    x = _x;
    y = _y;
  }
  public Vector2Int Copy() {
    return new Vector2Int(x,y);
  }
}


Vector2Int[] CopyVector2Int(Vector2Int[] originalArray) {
  if (originalArray == null) { return null; } // Safety check.
  Vector2Int[] newArray = new Vector2Int[originalArray.length];
  for (int i=0; i<newArray.length; i++) {
    newArray[i] = originalArray[i].Copy();
  }
  return newArray;
}

