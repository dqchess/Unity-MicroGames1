using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class TileViewBody : MonoBehaviour {
        // Components
        private List<Image> bodyImages;
        private List<Image> betweenImages; // SIMPLE way to have flush shapes. These are the squares BETWEEN my footprint images (that hide the inside rounded corners).
        // References
        [SerializeField] private TileView myTileView=null;
        [SerializeField] private Sprite s_bodyUnitRound=null;
        // Properties
        [SerializeField] internal bool isShadow=false; // a little weird how we handle the shadow here. Not a huge deal tho.
        private Color bodyColor;
        private HashSet<Vector2> bwPoses = new HashSet<Vector2>(); // one of each of the poses BETWEEN footprint spaces.
        
        // Getters (Static)
        private static Color GetBodyColor(int _colorID) {
            switch (_colorID) {
            case 0: return new ColorHSB(128/255f,250/255f,180/255f).ToColor();
            case 1: return new ColorHSB( 58/255f,250/255f,180/255f).ToColor();
            case 2: return new ColorHSB( 28/255f,250/255f,220/255f).ToColor();
            case 3: return new ColorHSB(180/255f,250/255f,180/255f).ToColor();
            case 4: return new ColorHSB(220/255f,210/255f,220/255f).ToColor();
            case 5: return new ColorHSB(  5/255f,250/255f,220/255f).ToColor();
            //case 0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
            //case 1: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
            //case 2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
            //case 3: return new ColorHSB(180/255f,250/255f,200/255f).ToColor();
            //case 4: return new ColorHSB(220/255f,150/255f,245/255f).ToColor();
            //case 5: return new ColorHSB(  5/255f,250/255f,245/255f).ToColor();
            default: return Color.red; // Oops! Too many colors.
            }
        }
        // Getters (Private)
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
            //// Return!
            //Vector2[] returnArray = new Vector2[bwPoses.Count];
            //bwPoses.CopyTo(returnArray);
            //return returnArray;
        }
        

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
            bodyColor = GetBodyColor(MyTile.ColorID);
            bodyImages = new List<Image>(); // Note: We add our first image in UpdateVisualsPostMove.
            betweenImages = new List<Image>();
            
            // Kinda sloppy how we handle the shadow. :P
            if (isShadow) {
                bodyColor = Color.Lerp(bodyColor, Color.black, 0.3f);
                this.transform.localPosition = new Vector3(0, -UnitSize*0.03f, 0); // nudge shadow down
            }
            else {
                this.transform.localPosition = new Vector3(0, UnitSize*0.03f, 0); // nudge body up
            }
        }
        

        private void AddBodyImage(Vector2Int footPos) {
            float diameter = UnitSize*0.9f;
            Image newImage = new GameObject().AddComponent<Image>();
            GameUtils.ParentAndReset(newImage.gameObject, this.transform);
            newImage.sprite = s_bodyUnitRound;
            GameUtils.SizeUIGraphic(newImage, diameter,diameter);
            newImage.color = bodyColor;
            newImage.rectTransform.anchoredPosition = new Vector2(footPos.x*UnitSize, -footPos.y*UnitSize);
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            newImage.name = "i_FootprintUnit";
            bodyImages.Add(newImage);
        }
        private void AddBetweenImage(Vector2 bwPos) {
            float diameter = UnitSize*0.9f;
            Image newImage = new GameObject().AddComponent<Image>();
            GameUtils.ParentAndReset(newImage.gameObject, this.transform);
            //newImage.sprite = s_bodyUnitRound;
            GameUtils.SizeUIGraphic(newImage, diameter,diameter);
            newImage.color = bodyColor;
            newImage.rectTransform.anchoredPosition = new Vector2(bwPos.x*UnitSize, -bwPos.y*UnitSize);
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            newImage.name = "i_FootprintBetween";
            betweenImages.Add(newImage);
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void ApplyColor(Color color) {
            for (int i=0; i<bodyImages.Count; i++) { bodyImages[i].color = color; }
            for (int i=0; i<betweenImages.Count; i++) { betweenImages[i].color = color; }
        }
        public void SetHighlightAlpha(float alpha) {
            // FOR NOW, just color my body sprites instead of showing separate image(s).
            ApplyColor(Color.Lerp(bodyColor, Color.white, alpha));
        }
        
        public void UpdateVisualsPostMove() {
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
        
        
    }
}