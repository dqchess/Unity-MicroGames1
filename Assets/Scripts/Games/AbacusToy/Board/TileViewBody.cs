using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbacusToy {
    public class TileViewBody : MonoBehaviour {
        // Components
        private List<Image> bodyImages;
        private List<Image> betweenImages; // SIMPLE way to have flush shapes. These are the squares BETWEEN my footprint images (that hide the inside rounded corners).
        private List<Image> iconImages; // one for every bodyImage.
        // References
        [SerializeField] private TileView myTileView=null;
        [SerializeField] private Sprite s_bodyUnitRound=null;
        // Properties
        [SerializeField] internal bool isShadow=false; // a little weird how we handle the shadow here. Not a huge deal tho.
        private Color bodyColor;
        private HashSet<Vector2> bwPoses = new HashSet<Vector2>(); // one of each of the poses BETWEEN footprint spaces.
        private float highlightAlpha=0;
        
        // Getters (Static)
        private static Color GetBodyColor(int _colorID) {
            //return new Color(.8f,0.8f,0.8f);
            switch (_colorID) {
            case 0: return new ColorHSB(128/255f,250/255f,180/255f).ToColor();
            case 1: return new ColorHSB( 58/255f,250/255f,180/255f).ToColor();
            case 2: return new ColorHSB( 28/255f,250/255f,220/255f).ToColor();
            case 3: return new ColorHSB(180/255f,250/255f,180/255f).ToColor();
            case 4: return new ColorHSB(220/255f,210/255f,220/255f).ToColor();
            case 5: return new ColorHSB(  5/255f,250/255f,220/255f).ToColor();
            case 6: return new ColorHSB(200/255f,250/255f,220/255f).ToColor();
            case 7: return new ColorHSB( 80/255f,250/255f,220/255f).ToColor();
            case 8: return new ColorHSB(100/255f,250/255f,220/255f).ToColor();
            case 9: return new ColorHSB( 15/255f,180/255f,220/255f).ToColor();
            default: return Color.red; // Oops! Too many colors.
            }
        }
        // Getters (Private)
        private Color GetAppliedBodyColor() {
            Color color = bodyColor;
            //if (MyTile.IsInOnlyGroup) { color = Color.Lerp(color, Color.black, 0.3f); }
            color = Color.Lerp(color, Color.white, highlightAlpha);
            return color;
        }
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
            bodyColor = GetBodyColor(MyTile.ColorID);
            bodyImages = new List<Image>(); // Note: We add our first image in UpdateVisualsPostMove.
            betweenImages = new List<Image>();
            iconImages = new List<Image>();
            
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
            Image newImage = AddImage(footPos, "i_FootprintUnit", s_bodyUnitRound);
            newImage.color = GetAppliedBodyColor();
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            bodyImages.Add(newImage);
        }
        private void AddIconImage(Vector2Int footPos) {
            Image newImage = AddImage(footPos, "i_Icon", myTileView.GetIconSprite(MyTile.ColorID));
            newImage.color = new Color(1,1,1, 0.8f);
            iconImages.Add(newImage);
        }
        private void AddBetweenImage(Vector2 bwPos) {
            Image newImage = AddImage(bwPos, "i_FootprintBetween", null);
            newImage.color = GetAppliedBodyColor();
            newImage.transform.SetAsFirstSibling(); // put behind everything else.
            betweenImages.Add(newImage);
        }
        private Image AddImage(Vector2Int boardPos, string _name, Sprite sprite) { return AddImage(boardPos.ToVector2(), _name, sprite); }
        private Image AddImage(Vector2 boardPos, string _name, Sprite sprite) {
            Image newImage = new GameObject().AddComponent<Image>();
            GameUtils.ParentAndReset(newImage.gameObject, this.transform);
            newImage.name = _name;
            newImage.sprite = sprite;
            GameUtils.SizeUIGraphic(newImage, UnitSize,UnitSize);
            newImage.rectTransform.anchoredPosition = new Vector2(boardPos.x*UnitSize, -boardPos.y*UnitSize);
            return newImage;
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void ApplyBodyColor() {
            // FOR NOW, just color my body sprites instead of showing separate image(s).
            Color color = GetAppliedBodyColor();
            
            for (int i=0; i<bodyImages.Count; i++) { bodyImages[i].color = color; }
            for (int i=0; i<betweenImages.Count; i++) { betweenImages[i].color = color; }
            
            Color iconColor = MyTile.IsInOnlyGroup ? new Color(0,0,0, 0.16f) : new Color(1,1,1, 0.7f);
            for (int i=0; i<iconImages.Count; i++) { iconImages[i].color = iconColor; }
        }
        public void SetHighlightAlpha(float alpha) {
            highlightAlpha = alpha;
            ApplyBodyColor();
        }
        
        public void UpdateVisualsPreMove() {
            ApplyBodyColor();
        }
        public void UpdateVisualsPostMove() {
            // Add images to fit my footprint!
            if (bodyImages.Count != footprintLocal.Count) {
                for (int i=bodyImages.Count; i<footprintLocal.Count; i++) {
                    AddBodyImage(footprintLocal[i]);
                    if (!isShadow) { AddIconImage(footprintLocal[i]); }
                }
            }
            UpdateBetweenFootprintLocalPosesAndAddImages();
            ApplyBodyColor();
        }
        
        
    }
}