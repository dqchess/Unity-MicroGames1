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
		private Vector2 posA;
		private Vector2 posB;

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(TileView myTileView, TileViewBody myTileViewBody, MergeSpot myMergeSpot) {
			BoardView boardView = myTileView.MyBoardView;
			Tile myTile = myTileView.MyTile;

			// Parent jazz.
			GameUtils.ParentAndReset(this.gameObject, myTileViewBody.transform);

			// Size and color me, Ayla.
			float unitSize = boardView.UnitSize;
			float diameter = TileViewBody.GetDiameter(unitSize);
			myImage.rectTransform.sizeDelta = new Vector2(diameter,diameter);
			myImage.color = myTileViewBody.BodyColor;

			posA = boardView.BoardToLocal(myMergeSpot.pos); // "home-base" space that's gonna lean into the merge.
			posB = boardView.BoardToLocal(myMergeSpot.pos+myMergeSpot.dir); // "away" space that we're leaning into.
			posA -= myTileView.Pos; // a little weirdly offset back so we're local to my TileView.
			posB -= myTileView.Pos; // a little weirdly offset back so we're local to my TileView.

			// Start at loc 0 for cleanness.
			GoToValues(0);
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void GoToValues(float loc) {
			// Scale loc to fit along animation curve! So we only pop out a little initially, and lots at the end.
			//loc = Mathf.Lerp(-3, 1, loc);
            float appliedLoc = ac_locMovement.Evaluate(loc);

			myRectTransform.anchoredPosition = Vector2.Lerp(posA,posB, appliedLoc);
		}
//		/** Same principle as SetValues_From_ByCurrentValues in BoardObjectView. */
//		public void SetValues_From_ByCurrentValues() {
//			posA = myRectTransform.anchoredPosition;
//		}
	}
}