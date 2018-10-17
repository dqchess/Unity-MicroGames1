using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class MergeSpotView : MonoBehaviour {
		// Components
		[SerializeField] private Image myImage;
		[SerializeField] private RectTransform myRectTransform;
		// Properties
//		private MergeSpot myMergeSpot;
		private Vector2 posA;
		private Vector2 posB;

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(TileView myTileView, MergeSpot myMergeSpot) {
//			this.myMergeSpot = myMergeSpot;
			// Parent jazz.
			GameUtils.ParentAndReset(this.gameObject, myTileView.transform);
			// Size and color me, Ayla.
			float unitSize = myTileView.MyBoardView.UnitSize;
			float diameter = TileViewBody.GetDiameter(unitSize);
			myImage.rectTransform.sizeDelta = new Vector2(diameter,diameter);
			myImage.color = TileViewBody.GetBodyColor(myTileView.MyTile.ColorID); // TODO: darken for the shadow, yanno.

			BoardView boardView = myTileView.MyBoardView;
			posA = boardView.BoardToLocal(myMergeSpot.pos+myMergeSpot.dir);
			posB = boardView.BoardToLocal(myMergeSpot.pos);
			posB = Vector2.Lerp(posA,posB, 0.2f); // Ok, keep posB PRETTY dialed back-- keep it close to home.
			posA -= myTileView.Pos; // a little weirdly offset back so we're local to my TileView.
			posB -= myTileView.Pos; // a little weirdly offset back so we're local to my TileView.

			// Start at loc 0 for cleanness.
			GoToValues(0);
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void GoToValues(float loc) {
			// Scale loc a bunch, ok? (So the reaching effect only happens near the end of the visual drag.)
			loc = Mathf.Lerp(-3, 1, loc);

			myRectTransform.anchoredPosition = Vector2.Lerp(posA,posB, loc);
		}
	}
}