using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class MergeSpotView : MonoBehaviour {
		// Components
		[SerializeField] private Image myImage=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
        [SerializeField] private AnimationCurve ac_locMovement=null;
        private float unitSize;
        private float diameter;
        private Vector2 sizeA;
        private Vector2 sizeB;
        // References
        public MergeSpot MyMergeSpot { get; private set; }
        
        public bool IsMergeSpot(Vector2 pos) {
            return MyMergeSpot.pos == pos;
        }

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(TileView myTileView, TileViewBody myTileViewBody, MergeSpot myMergeSpot) {
            BoardView boardView = myTileView.MyBoardView;
            Tile myTile = myTileView.MyTile;
            this.MyMergeSpot = myMergeSpot;

            // Parent jazz.
            GameUtils.ParentAndReset(this.gameObject, myTileViewBody.transform);

            // Size and color me, Ayla.
            unitSize = boardView.UnitSize;
            diameter = TileViewBody.GetDiameter(unitSize);
            myImage.rectTransform.sizeDelta = new Vector2(diameter,diameter);
            myImage.color = myTileViewBody.BodyColor;
            if (!myTileViewBody.isShadow) { // If I'm NOT the shadow, texture me!
                myImage.material = ResourcesHandler.Instance.SlideAndStickTileBodyMat(myTile.ColorID);
            }
            
            Vector2 pos = boardView.BoardToLocal(myMergeSpot.pos) - myTileView.Pos; // a little weirdly offset back so we're local to my TileView.
            Vector2 dirVec2 = new Vector2(myMergeSpot.dir.x, -myMergeSpot.dir.y); // flip y.
            pos -= dirVec2*0.4f * unitSize;
            myRectTransform.anchoredPosition = pos;
            myRectTransform.localEulerAngles = new Vector3(0,0, MathUtils.DirToRotation(myMergeSpot.dir));
            
            sizeA = new Vector2(diameter, diameter*0.9f); // from my center to my edge (half a unit).
            sizeB = new Vector2(diameter, diameter*1.9f); // from my center to the edge of the next space (one and a half units).

            // Start at loc 0 for cleanness.
            GoToValues(0);
        }

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void GoToValues(float loc) {
            // Scale loc to fit along animation curve! So we only pop out a little initially, and lots at the end.
            float appliedLoc = ac_locMovement.Evaluate(loc);
            myRectTransform.sizeDelta = Vector2.Lerp(sizeA,sizeB, appliedLoc);
        }

//		/** Same principle as SetValues_From_ByCurrentValues in BoardObjectView. */
//		public void SetValues_From_ByCurrentValues() {
//			posA = myRectTransform.anchoredPosition;
//		}
	}
}