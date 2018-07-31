
int TotalGrowersValue() {
  float total = 0;
  for (int i=0; i<growers.size(); i++) {
    Grower obj = (Grower) growers.get(i);
    total += obj.ScoreValue();
  }
  return (int)total;
}
