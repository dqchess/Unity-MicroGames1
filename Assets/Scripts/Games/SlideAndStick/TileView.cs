using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class TileView : BoardOccupantView {
		// Components
        [SerializeField] private TileViewBody body=null;
        [SerializeField] private TileViewBody bodyShadow=null;
        // References
        public Tile MyTile { get; private set; }



        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (BoardView _myBoardView, Tile _myObj) {
			MyTile = _myObj;
			base.InitializeAsBoardOccupantView (_myBoardView, _myObj);
            body.Initialize();
            bodyShadow.Initialize();
		}


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        override public void UpdateVisualsPostMove() {
			base.UpdateVisualsPostMove();
            
            body.UpdateVisualsPostMove();
            bodyShadow.UpdateVisualsPostMove();
		}
		override public void GoToValues (float lerpLoc) {
			base.GoToValues(lerpLoc);
			body.GoToValues(lerpLoc);
			bodyShadow.GoToValues(lerpLoc);
		}
//		override public void SetValues_From_ByCurrentValues () {
//			base.SetValues_From_ByCurrentValues();
//			body.SetValues_From_ByCurrentValues();
//			bodyShadow.SetValues_From_ByCurrentValues();
//		}
		override public void SetValues_To(BoardObject _bo) {
			base.SetValues_To(_bo);
            Tile simTile = _bo as Tile; // _bo must be a Tile.
			body.SetValues_To(simTile);
			bodyShadow.SetValues_To(simTile);
		}


		public void OnMouseOut() {
			SetHighlightAlpha(0);
		}
		public void OnMouseOver() {
			SetHighlightAlpha(0.25f);
		}
		public void OnStopGrabbing() {
			SetHighlightAlpha(0);
		}
		public void OnStartGrabbing() {
			SetHighlightAlpha(0.35f);
		}
        private void SetHighlightAlpha(float alpha) {
            //body.SetHighlightAlpha(alpha);TEMP! Disabled highlighting!
        }


        // ----------------------------------------------------------------
        //  Animating
        // ----------------------------------------------------------------
//        public void AnimateIn(Tile sourceTile) {
//            StartCoroutine(Coroutine_AnimateIn(sourceTile));
//        }
//        private IEnumerator Coroutine_AnimateIn(Tile sourceTile) {
//            Vector2 targetPos = Pos;
//            float targetScale = 1;
//
//            Scale = 0f; // shrink me down
//
//            // If I came from a source, then start me there and animate me to my actual position!
//            if (sourceTile != null) {
//                Vector2 sourcePos = GetPosFromBO(sourceTile);
//                Pos = sourcePos;
//            }
//            // Animate!
//            float easing = 0.5f; // higher is faster.
//            while (Pos!=targetPos || Scale!=targetScale) {
//                Pos += new Vector2((targetPos.x-Pos.x)*easing, (targetPos.y-Pos.y)*easing);
//                Scale += (targetScale-Scale) * easing;
//                if (Vector2.Distance(Pos,targetPos)<0.5f) { Pos = targetPos; } // Almost there? Get it!
//                if (Mathf.Approximately(Scale,targetScale)) { Scale = targetScale; } // Almost there? Get it!
//                yield return null;
//            }
//        }

	}
}