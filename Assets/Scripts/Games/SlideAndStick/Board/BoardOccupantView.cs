using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class BoardOccupantView : BoardObjectView {
		// References
		protected BoardOccupant myOccupant; // a direct reference to my model. Doesn't change.

		// Getters (Public)
		public BoardOccupant MyBoardOccupant { get { return myOccupant; } }


		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		protected void InitializeAsBoardOccupantView (BoardView _myBoardView, Transform tf_parent, BoardOccupant _myOccupant) {
			base.InitializeAsBoardObjectView (_myBoardView, tf_parent, _myOccupant);
			myOccupant = _myOccupant;
		}

		/** This is called once all the animation is finished! */
		override public void UpdateVisualsPostMove () {
			base.UpdateVisualsPostMove();
			if (!myOccupant.IsInPlay) {
				DestroySelf ();
			}
		}
		private void DestroySelf () {
			MyBoardView.OnOccupantViewDestroyedSelf(this);
			// Legit destroy me, yo!
			Destroy(this.gameObject);
		}
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void PreAnimateInFreshBoard() {
            this.gameObject.transform.localScale = Vector3.one * 0.7f;
        }
        public void AnimateInFreshBoard() {
            LeanTween.scale(this.gameObject, Vector3.one, 0.8f).setEaseOutBounce();
        }


	}
}
