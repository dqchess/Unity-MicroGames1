using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class TileView : BoardOccupantView {
		// Components
//        [SerializeField] private TileViewBody body=null;QQQ! disabled
        [SerializeField] private TileViewBody bodyShadow=null;
		private List<MergeSpotView> mergeSpotViews; // only exists when we're animating (aka between loc from and to).
        // References
        public Tile MyTile { get; private set; }



        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (BoardView _myBoardView, Tile _myObj) {
			MyTile = _myObj;
			base.InitializeAsBoardOccupantView (_myBoardView, _myObj);
//            body.Initialize();
            bodyShadow.Initialize();
		}


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
		override public void UpdateVisualsPostMove() {
			base.UpdateVisualsPostMove();
			DestroyMergeSpotViews();
            
//            body.UpdateVisualsPostMove();
            bodyShadow.UpdateVisualsPostMove();
		}
		override public void GoToValues (float lerpLoc) {
			base.GoToValues(lerpLoc);

			for (int i=0; i<mergeSpotViews.Count; i++) {
				mergeSpotViews[i].GoToValues(lerpLoc);
			}
		}
		override public void SetValues_To (BoardObject _bo) {
			base.SetValues_To(_bo);
			RemakeMergeSpotViews(_bo.BoardRef);
		}
		private void RemakeMergeSpotViews(Board simBoard) {
			DestroyMergeSpotViews(); // Just in case.
//			if (MyTile != null) {//TEST
			for (int i=0; i<simBoard.LastMergeSpots.Count; i++) {
				MergeSpot ms = simBoard.LastMergeSpots[i];
				if (MyTile.FootprintGlobal.Contains(ms.pos+ms.dir)) {
					AddMergeSpotView(ms);
				}
			}
//			}
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
			obj.Initialize(this, mergeSpot);
			mergeSpotViews.Add(obj);
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
//            body.SetHighlightAlpha(alpha);
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