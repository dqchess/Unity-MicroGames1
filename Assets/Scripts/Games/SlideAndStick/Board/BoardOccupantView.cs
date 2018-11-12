using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class BoardOccupantView : BoardObjectView {
        //// Properties
        //[SerializeField] private AnimationCurve ac_appear=null;
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
        //  Animations
        // ----------------------------------------------------------------
        //public void PreAnimateInFreshBoard() {
        //    float offset = (myOccupant.Col + myOccupant.Row*1.1f) * 0.04f;
        //    this.gameObject.transform.localScale = Vector3.one * (0.9f-offset);
        //}
        //public void AnimateInFreshBoard() {
        //    float offset = (myOccupant.Col + myOccupant.Row*1.1f) * 0.04f;
        //    LeanTween.scale(this.gameObject, Vector3.one, 0.5f + offset).setEaseOutBounce();
        //}
        public void PreAnimateInFreshBoard() {
            // TODO: Maybe. I'd really like to have these start at like 0.5 scale. For that, we need to use an AnimationClip.
            //float offset = (myOccupant.Col + myOccupant.Row*1.1f) * 0.1f;
            this.gameObject.transform.localScale = Vector3.zero;//Vector3.one * (0.4f-offset);
        }
        public void AnimateInFreshBoard() {
            //float offset = (myOccupant.Col + myOccupant.Row) * 0.05f;
            float delay = (myOccupant.Col + myOccupant.Row) * 0.042f;
            LeanTween.scale(this.gameObject, Vector3.one, 0.52f).setEaseOutBack().setDelay(delay);// + offset
        }


	}
}
