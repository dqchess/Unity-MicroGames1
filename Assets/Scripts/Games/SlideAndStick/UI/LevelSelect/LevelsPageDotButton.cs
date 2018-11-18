using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class LevelsPageDotButton : Button {
		// Components
		private RectTransform myRectTransform; // Set manually in Awake. #UnityUIButtons
		// References
		private LevelsPageNav levelsPageNav;
		// Properties
		private int myPageIndex;


		// ----------------------------------------------------------------
		//  Awake
		// ----------------------------------------------------------------
		override protected void Awake() {
            base.Awake();
			myRectTransform = GetComponent<RectTransform>();
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(LevelsPageNav _levelsPageNav, int _myPageIndex) {
			levelsPageNav = _levelsPageNav;
			myPageIndex = _myPageIndex;
			GameUtils.ParentAndReset(this.gameObject, levelsPageNav.transform);
		}


		// ----------------------------------------------------------------
		//  Spawn / Despawn
		// ----------------------------------------------------------------
		public void Spawn(Color packColor) {
			this.gameObject.SetActive(true);
			// Color me impressed.
			ColorBlock cb = this.colors;
			cb.normalColor = packColor;
			cb.highlightedColor = Color.Lerp(packColor, Color.white, 0.2f);
			cb.pressedColor = Color.Lerp(packColor, Color.black, 0.3f);
			cb.disabledColor = new Color(packColor.r,packColor.g,packColor.b, packColor.a*0.2f);
			this.colors = cb;
		}
		public void Despawn() {
			this.gameObject.SetActive(false);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void SetPosSize(Vector2 _pos, Vector2 _size) {
			myRectTransform.anchoredPosition = _pos;
			myRectTransform.sizeDelta = _size;
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnClick() {
			levelsPageNav.OnPageDotButtonClick(myPageIndex);
		}


	}
}