
private void Draw() {
  // Translate into Unity coordinates! 0,0 is BOTTOM-left.
  pushMatrix();
  translate(0,height);
  scale(1, -1);
  
  DrawDancers();
  DrawGround();
  
  popMatrix();
}

private void DrawGround() {
  fill(130, 180, 80);
  noStroke();
  rect(stageX,0, stageWidth,groundY);
}
private void DrawDancers() {
  for (int i=0; i<dancers.length; i++) {
    dancers[i].Draw();
  }
}
