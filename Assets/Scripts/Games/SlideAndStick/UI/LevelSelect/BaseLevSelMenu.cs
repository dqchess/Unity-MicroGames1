using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class BaseLevSelMenu : MonoBehaviour {
		// Components
		[SerializeField] private CanvasGroup myCanvasGroup=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private float openLoc;
		private float alphaFull;
		private float xOpen,xClosed; // for tweening pos in/out.
    
        // Getters (Protected)
        protected LevelsManager lm { get { return LevelsManager.Instance; } }
        protected LevelAddress selectedAddress { get { return lm.selectedAddress; } }


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start() {
			xOpen = 0;
			alphaFull = 1;
			SetOpenLoc(0);
		}


		// ----------------------------------------------------------------
		//  Animation Stuff
		// ----------------------------------------------------------------
		private void SetOpenLoc(float val) {
			openLoc = val;
			myCanvasGroup.alpha = Mathf.Lerp(0,alphaFull, openLoc);
			float xPos = Mathf.Lerp(xClosed,xOpen, val);
			myRectTransform.anchoredPosition = new Vector2(xPos, myRectTransform.anchoredPosition.y);
		}


		// ----------------------------------------------------------------
		//  Open / Close
		// ----------------------------------------------------------------
		public void Open(MenuTransType transType) {
			this.gameObject.SetActive(true);

			alphaFull = 1;
			xClosed = xOpen + (transType==MenuTransType.Pop?-1:1) * 300;

			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject, SetOpenLoc, openLoc,1, 0.6f).setEaseOutQuint().setOnComplete(OnCompleteOpen);
		}
		public void Close(MenuTransType transType) {
			alphaFull = 0.55f;
			xClosed = xOpen + (transType==MenuTransType.Pop?1:-1) * 50;

			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject, SetOpenLoc, openLoc,0, 0.3f).setEaseOutQuint().setOnComplete(OnCompleteClose);
		}

		private void OnCompleteOpen() {
			SetOpenLoc(1);
		}
		private void OnCompleteClose() {
			SetOpenLoc(0);
			this.gameObject.SetActive(false);
		}
        
    }
}