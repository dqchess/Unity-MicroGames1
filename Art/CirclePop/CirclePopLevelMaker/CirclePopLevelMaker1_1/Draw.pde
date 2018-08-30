
void DrawGrowers() {
  pushMatrix();
  translate(levelCenter.x,levelCenter.y);
  
  for (int i=0; i<growers.size(); i++) {
    Grower obj = (Grower) growers.get(i);
    obj.Draw();
  }
  
  popMatrix();
}

void DrawLevelBounds() {
  fill(0, 20);
  noStroke();
  rect(levelBounds.x,levelBounds.y, levelBounds.w,levelBounds.h);
}
void DrawTotalGrowersValue() {
  int total = TotalGrowersValue();
  textAlign(TOP, LEFT);
  textSize(32);
  fill(0, 200);
  text(total, 10,60);
}

