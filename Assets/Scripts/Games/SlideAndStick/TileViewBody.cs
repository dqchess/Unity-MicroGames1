using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class TileViewBody : MonoBehaviour {
        // Components
        private List<Image> bodyImages;
		private List<Image> betweenImages; // SIMPLE way to have flush shapes. These are the squares BETWEEN my footprint images (that hide the inside rounded corners).
		private List<MergeSpotView> mergeSpotViews; // only exists when we're animating (aka between loc from and to).
        // References
        [SerializeField] private TileView myTileView=null;
        [SerializeField] private Sprite s_bodyUnitRound=null;
        // Properties
        [SerializeField] internal bool isShadow=false; // a little weird how we handle the shadow here. Not a huge deal tho.
		public Color BodyColor { get; private set; }
        private HashSet<Vector2> bwPoses = new HashSet<Vector2>(); // one of each of the poses BETWEEN footprint spaces.
        private float highlightAlpha=0;
        
        // Getters (Static)
		static public float GetDiameter(float unitSize) { return unitSize * 0.9f; }
		static public Color GetBodyColor(int _colorID) {
            switch (_colorID) {
            case 0: return new Color( 71/255f,128/255f,214/255f);
            case 1: return new Color(100/255f,220/255f, 95/255f);
            case 2: return new Color(231/255f,100/255f,192/255f);
            case 3: return new Color(232/255f,182/255f,110/255f);
            case 4: return new Color(110/255f,215/255f,233/255f);
            case 5: return new Color(  5/255f,250/255f,220/255f);
            default: return Color.red; // Oops! Too many colors.
            }
        }
        // Getters (Private)
		private Color GetAppliedBodyColor() { return Color.Lerp(BodyColor, Color.white, highlightAlpha); }
        private float UnitSize { get { return myTileView.MyBoardView.UnitSize; } }
        private Tile MyTile { get { return myTileView.MyTile; } }
        private List<Vector2Int> footprintLocal { get { return MyTile.FootprintLocal; } }
        
        /** If footprint is toes, think of this as spaces between the toes. E.g. If I have (0,0), (0,1), (1,0), this'll return (0,0.5), (0.5,0). */
        private void UpdateBetweenFootprintLocalPosesAndAddImages() {
            HashSet<Vector2Int> fpLocalHash = new HashSet<Vector2Int>(footprintLocal); // make HashSet for easy accessin'.
            Vector2Int[] dirs = new Vector2Int[] {Vector2Int.L, Vector2Int.R, Vector2Int.B, Vector2Int.T};
            //Vector2Int[] dirs = new Vector2Int[] {Vector2Int.L,Vector2Int.R,Vector2Int.B,Vector2Int.T, Vector2Int.BL,Vector2Int.BR,Vector2Int.TL,Vector2Int.TR};
            foreach (Vector2Int fpLocal in footprintLocal) { // For each footprint space...
                foreach (Vector2Int dir in dirs) { // For each side...
                    Vector2Int posInDir = fpLocal + dir;
                    if (fpLocalHash.Contains(posInDir)) { // I have a footprint here!...
                        Vector2 bwBoardPos = Vector2.Lerp(fpLocal.ToVector2(), posInDir.ToVector2(), 0.5f);
                        // This between-pos hasn't been found yet? Add it to list!
                        if (!bwPoses.Contains(bwBoardPos)) {
                            bwPoses.Add(bwBoardPos);
                            AddBetweenImage(bwBoardPos);
                        }
                    }
                }
            }
        }
        

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
			BodyColor = GetBodyColor(MyTile.ColorID);
//			BodyColor = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f)); // DEBUG TEST
            bodyImages = new List<Image>(); // Note: We add our first image in UpdateVisualsPostMove.
            betweenImages = new List<Image>();
            // Note: Can't set this in the Editor for some reason. :P
            this.GetComponent<Canvas>().overrideSorting = true;
            
            // Kinda sloppy how we handle the shadow. :P
            if (isShadow) {
				BodyColor = Color.Lerp(BodyColor, Color.black, 0.3f); // Darkness.
                this.transform.localPosition = new Vector3(0, -UnitSize*0.03f, 0); // nudge shadow down
            }
            else {
                this.transform.localPosition = new Vector3(0, UnitSize*0.03f, 0); // nudge body up
            }
        }
        

        private void AddBodyImage(Vector2Int footPos) {
			float diameter = GetDiameter(UnitSize);
            Image newImage = new GameObject().AddComponent<Image>();
            GameUtils.ParentAndReset(newImage.gameObject, this.transform);
            newImage.sprite = s_bodyUnitRound;
//			newImage.type = Image.Type.Sliced;
            GameUtils.SizeUIGraphic(newImage, diameter,diameter);
            newImage.color = GetAppliedBodyColor();
            newImage.rectTransform.anchoredPosition = new Vector2(footPos.x*UnitSize, -footPos.y*UnitSize);
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            newImage.name = "i_FootprintUnit";
            bodyImages.Add(newImage);
        }
        private void AddBetweenImage(Vector2 bwPos) {
			float diameter = GetDiameter(UnitSize);
            Image newImage = new GameObject().AddComponent<Image>();
            GameUtils.ParentAndReset(newImage.gameObject, this.transform);
            //newImage.sprite = s_bodyUnitRound;
            GameUtils.SizeUIGraphic(newImage, diameter,diameter);
            newImage.color = GetAppliedBodyColor();
            newImage.rectTransform.anchoredPosition = new Vector2(bwPos.x*UnitSize, -bwPos.y*UnitSize);
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            newImage.name = "i_FootprintBetween";
            betweenImages.Add(newImage);
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void ApplyBodyColor() {
            // FOR NOW, just color my body sprites instead of showing separate image(s).
            Color color = GetAppliedBodyColor();
            
            for (int i=0; i<bodyImages.Count; i++) { bodyImages[i].color = color; }
            for (int i=0; i<betweenImages.Count; i++) { betweenImages[i].color = color; }
        }
        public void SetHighlightAlpha(float alpha) {
            highlightAlpha = alpha;
            ApplyBodyColor();
        }
        
		public void UpdateVisualsPostMove() {
			DestroyMergeSpotViews(); // We wanna nix any MergeSpotViews for finished-animation.
            // Add images to fit my footprint!
            if (bodyImages.Count != footprintLocal.Count) {
                for (int i=bodyImages.Count; i<footprintLocal.Count; i++) {
                    AddBodyImage(footprintLocal[i]);
                }
            }
            UpdateBetweenFootprintLocalPosesAndAddImages();
            //Vector2[] bwPoses = GetBetweenFootprintLocalPoses();
            //if (betweenImages.Count != bwPoses.Length) {
            //    for (int i=betweenImages.Count; i<bwPoses.Length; i++) {
            //    }
            //}
		}

		public void GoToValues(float loc) {
			for (int i=0; i<mergeSpotViews.Count; i++) {
				mergeSpotViews[i].GoToValues(loc);
			}
		}
//		public void SetValues_From_ByCurrentValues() {
//			for (int i=0; i<mergeSpotViews.Count; i++) {
//				mergeSpotViews[i].SetValues_From_ByCurrentValues();
//			}
//		}
		public void SetValues_To(Tile simTile) {
			RemakeMergeSpotViews(simTile);
		}
		private void RemakeMergeSpotViews(Tile simTile) {
			DestroyMergeSpotViews(); // Just in case.

//			if (bodyImages != null) {
//				BodyColor = new Color(BodyColor.r,BodyColor.g,BodyColor.b, simBO.IsInPlay ? 1 : 0.4f);
//				ApplyBodyColor();
//			}
            
            // ONLY make MergeSpotViews for Tiles still in play!
            if (simTile.IsInPlay) {
                Board simBoard = simTile.BoardRef;
    			for (int i=0; i<simBoard.LastMergeSpots.Count; i++) {
    				MergeSpot ms = simBoard.LastMergeSpots[i];
    				if (simTile.FootprintGlobal.Contains(ms.pos+ms.dir)) {
    					AddMergeSpotView(ms);
    				}
    			}
            }
		}


		private void DestroyMergeSpotViews() {
			if (mergeSpotViews != null) {
				for (int i=0; i<mergeSpotViews.Count; i++) {
					Destroy(mergeSpotViews[i].gameObject);
				}
			}
			mergeSpotViews = new List<MergeSpotView>();
		}
		private void AddMergeSpotView(MergeSpot mergeSpot) {
			MergeSpotView obj = Instantiate(ResourcesHandler.Instance.slideAndStick_mergeSpotView).GetComponent<MergeSpotView>();
			obj.Initialize(myTileView, this, mergeSpot);
			mergeSpotViews.Add(obj);
//			print(Time.frameCount + "  " + MyTile.BoardRef.tiles.IndexOf(MyTile) + "  add mergeSpot: " + mergeSpot.pos + ", dir: " + mergeSpot.dir + ". footprint size: " + MyTile.FootprintGlobal.Count);
		}
        
        
    }
}


//case 0: return new ColorHSB(128/255f,250/255f,180/255f).ToColor();
//case 1: return new ColorHSB( 58/255f,250/255f,180/255f).ToColor();
//case 2: return new ColorHSB( 28/255f,250/255f,220/255f).ToColor();
//case 3: return new ColorHSB(180/255f,250/255f,180/255f).ToColor();
//case 4: return new ColorHSB(220/255f,210/255f,220/255f).ToColor();
//case 5: return new ColorHSB(  5/255f,250/255f,220/255f).ToColor();
//case 0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
//case 1: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
//case 2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
//case 3: return new ColorHSB(180/255f,250/255f,200/255f).ToColor();
//case 4: return new ColorHSB(220/255f,150/255f,245/255f).ToColor();
//case 5: return new ColorHSB(  5/255f,250/255f,245/255f).ToColor();
