// v_resetGame


void resetGame() {
  cols = 4;
  rows = 4;
  
  float unitDiameter = min((width-MIN_HORZ_GAP*2)/cols, (height-MIN_VERT_GAP*2)/rows);
  unitSize = new PVector(unitDiameter,unitDiameter);
  gridSize = new PVector(cols*unitSize.x, rows*unitSize.y);
  gridX = (width-gridSize.x) * 0.5;
  gridY = (height-gridSize.y) * 0.5;
  
  boardSnapshots = new BoardData[0];
  
  
  gridSpaces = new GridSpace[cols][rows];
  for (int col=0; col<cols; col++) {
    for (int row=0; row<rows; row++) {
      gridSpaces[col][row] = new GridSpace(col,row);
    }
  }
  
  AddRandomGoodWells();
  ClearAllWellPaths();
  
  /*
  int numStartingWells = int(cols*rows * 0.6);
  int randCol,randRow;
  for (int i=0; i<numStartingWells; i++) {
    do {
      randCol = int(random(cols));
      randRow = int(random(rows));
    } while (GetWell(randCol,randRow) != null);
    addWell(randCol,randRow, random(numColors));
  }
  */
  
  OnMoveComplete();
}

void AddRandomGoodWells() {
  int count=0;
  while (count++<99) {
    AddRandomWells();
    // Are these good enough? Nice! Stop. :)
//    println("count: " + count + " AreRandomlyMadeWellsGood: " + AreRandomlyMadeWellsGood());
//    printGridSpaces();
    if (AreRandomlyMadeWellsGood()) { break; }
  }
}
boolean AreRandomlyMadeWellsGood() {
  for (int i=0; i<wells.length; i++) {
    if (wells[i].pathSpaces.length < MinRandWellPathLength) { return false; }
  }
  return true;
}


void AddRandomWells() {
  RemoveAllWells();
  
  int numSpacesOpen = cols*rows;
  
  int count=0;
  while (numSpacesOpen>0 && count++<999) {
    Well newWell = AddRandomWell();
    if (newWell == null) { break; } // Safety check.
    numSpacesOpen -= newWell.pathSpaces.length;
  }
}
Well AddRandomWell() {
  int count=0;
  while (true) {
    GridSpace randSpace = GetRandSpace();
    if (randSpace.CanAddWellPath()) { // This space is vacant!
      int colorID = wells.length;
      int value = 1; // will set next.
      Well newWell = AddWell(randSpace.col,randSpace.row, colorID,value);
      GiveWellRandPath(newWell);
      return newWell;
    }
    // Safety check.
    if (count++ > 999) { println("Whoa! No vacant board spaces left!"); return null; }//printGridSpaces(); 
  }
}

void GiveWellRandPath(Well well) {
  int numSpacesToFill = (int)random(5,8);
  int count=0;
  Vector2Int currSpace = new Vector2Int(well.col,well.row);
  while (well.pathSpaces.length<numSpacesToFill && count++<99) {
    int randSide = RandOpenSide(currSpace);
    if (randSide == -1) { break; } // Whoa! Hit a dead end? Ok, totally stop.
    Vector2Int dir = GetDir(randSide);
    currSpace = currSpace.Plus(dir);
    well.AddPathSpace(currSpace);
  }
  // Set this Well's numSpacesToFill!
  well.numSpacesToFill = well.pathSpaces.length;
  well.UpdateNumSpacesLeft();
}

int RandOpenSide(Vector2Int sourcePos) {
  int[] randSides = GetShuffledIntArray(4);
  // Try each side one by one.
  for (int i=0; i<randSides.length; i++) {
    Vector2Int dir = GetDir(randSides[i]);
    if (IsSpaceOpen(sourcePos.Plus(dir))) {
      return randSides[i];
    }
  }
  // Nah, nothin' fits.
  return -1;
}

/*
  public static void AddRandomPath(Board originalBoard, List<DragPath> paths, int maxPathSpaces) {
    DragPath path = new DragPath(paths.Count);
    paths.Add (path);
    Board b = originalBoard.Clone(); // make a copy we can edit.
    b.SetPremadePaths (paths); // Assign the paths list directly to the Board! We're modifying it directly for speed (instead of copying lots of stuff a lot).
    // Start on a random space!
    b.AddSpaceToPath(GetRandomSpaceToStartPath(b), path);
    // Keep adding spaces in random available directions until we can't anymore!
    AddRandomNextSpaceToPathRecursively(ref b, path.Index, maxPathSpaces);
  }
  //*
  public static DragPath GetRandomPath(Board originalBoard, List<DragPath> existingPaths, int maxPathSpaces) {
    List<DragPath> clonedPaths = new List<DragPath>();
    foreach (DragPath p in existingPaths) { clonedPaths.Add(p.Copy()); } // Duplicate the whole existing list.
    Board b = originalBoard.Clone(); // make a copy we can edit.
    b.SetPremadePaths (clonedPaths); // stamp the cloned list onto the cloned board!
    // Add one new path to the cloned board!
    DragPath path = new DragPath(b.paths.Count);
    b.paths.Add (path);
    // Start on a random space!
    b.AddSpaceToPath(GetRandomSpaceToStartPath(b), path);
    // Keep adding spaces in random available directions until we can't anymore!
    AddRandomNextSpaceToPathRecursively(ref b, path.Index, maxPathSpaces);
    // Return!
    return path;
  }
  private static void AddRandomNextSpaceToPathRecursively(ref Board b, int pi, int maxSpaces) {
    if (b.paths[pi].NumPoses >= maxSpaces) { return; } // Stop once we've hit the max spaces.

    int[] randSides = MathUtils.GetShuffledIntArray(GetAvailableSides(b));
    // Try each side one by one.
    for (int i=0; i<randSides.Length; i++) {
      if (CanAddPathSpaceAtSide(b, pi, randSides[i])) {
        AddPathSpaceAtSideRecursively(ref b, pi, randSides[i], maxSpaces);
        break;
      }
    }
  }
  private static bool CanAddPathSpaceAtSide(Board b, int pathIndex, int side) {
    DragPath path = b.paths[pathIndex];
    Vector2Int spacePos = path.PosRelative_Last + GetDir(side);
    return b.CanAddSpaceToPath(spacePos, path);
  }
  private static void AddPathSpaceAtSideRecursively(ref Board b, int pathIndex, int side, int maxSpaces) {
    DragPath path = b.paths[pathIndex];
    Vector2Int spacePos = path.PosRelative_Last + GetDir(side);
    b.AddSpaceToPath(spacePos, path);
    AddRandomNextSpaceToPathRecursively(ref b, pathIndex, maxSpaces);
  }
  */











