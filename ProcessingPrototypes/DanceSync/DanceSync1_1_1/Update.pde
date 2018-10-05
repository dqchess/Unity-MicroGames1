

private void Update() {
  if (isPaused) { return; }
  UpdateDancers();
  UpdateDancerMouseOver();
}
private void UpdateDancers() {
  for (int i=0; i<dancers.length; i++) {
    dancers[i].Update();
  }
}
private void UpdateDancerMouseOver() {
  mousePosCol = (int)floor((mouseX-stageX)/colWidth);
}


