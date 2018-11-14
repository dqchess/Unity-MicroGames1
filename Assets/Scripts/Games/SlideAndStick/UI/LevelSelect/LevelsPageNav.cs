using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class LevelsPageNav : MonoBehaviour {
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Button b_prev=null;
		[SerializeField] private Button b_next=null;
		[SerializeField] private Image i_selectedDot=null;
		private List<LevelsPageDotButton> b_dots=new List<LevelsPageDotButton>();
		// References
		[SerializeField] private PackSelectMenu psm;


		// Getters (Private)
		private int CurrPage { get { return psm.CurrPage; } }
		private int NumPages { get { return psm.NumLevelPages; } }
		private bool IsPresent {
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive(value); }
		}


		private void AddDotButton() {
			LevelsPageDotButton newObj = Instantiate(ResourcesHandler.Instance.slideAndStick_levelsPageDotButton).GetComponent<LevelsPageDotButton>();
			newObj.Initialize(this, b_dots.Count);
			b_dots.Add(newObj);
		}



		// ----------------------------------------------------------------
		//  Refreshing (Ahhh)
		// ----------------------------------------------------------------
		public void Refresh() {
			IsPresent = NumPages > 1;
			if (!IsPresent) { return; } // Not present? We can stop here.

			// Color i_selectedDot.
			i_selectedDot.color = psm.CurrentPackColor;

			// Add missing dots.
			for (int i=b_dots.Count; i<NumPages; i++) {
				AddDotButton();
			}
			// Hide extra dots.
			for (int i=NumPages; i<b_dots.Count; i++) {
				b_dots[i].Despawn();
			}
			// Spawn visible buttons!
			Vector2 buttonSize = new Vector2(16,16);
			float buttonSpacing = buttonSize.x + 32;
			for (int i=0; i<NumPages; i++) {
				float posX = (i+0.5f)*buttonSpacing - (NumPages*buttonSpacing*0.5f);
				Vector2 pos = new Vector2(posX, 0);
				b_dots[i].SetPosSize(pos, buttonSize);
				b_dots[i].Spawn(psm.CurrentPackColor);
			}

			// Position next/prev buttons.
			float bPrevX = (-0.6f - 1f		+0.5f)*buttonSpacing - (NumPages*buttonSpacing*0.5f);
			float bNextX = ( 0.6f + NumPages+0.5f)*buttonSpacing - (NumPages*buttonSpacing*0.5f);
			b_prev.transform.localPosition = new Vector3(bPrevX,b_prev.transform.localPosition.y,0);
			b_next.transform.localPosition = new Vector3(bNextX,b_next.transform.localPosition.y,0);

			// Update prev/next button colorss!
			ColorBlock cb = b_prev.colors;
			Color packColor = psm.CurrentPackColor;
			cb.normalColor = packColor;
			cb.highlightedColor = Color.Lerp(packColor, Color.white, 0.2f);
			cb.pressedColor = Color.Lerp(packColor, Color.black, 0.3f);
			cb.disabledColor = new Color(packColor.r,packColor.g,packColor.b, packColor.a*0.2f);
			b_prev.colors = cb;
			b_next.colors = cb;

			// Update for the current page, too.
			OnSetCurrPage();
		}

		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnPageDotButtonClick(int pageIndex) {
			psm.SetCurrPage(pageIndex);
		}
		public void OnSetCurrPage() {
			if (!IsPresent) { return; }

			b_prev.interactable = CurrPage > 0;
			b_next.interactable = CurrPage < NumPages-1;

			// Tween selectedDot highlight into place!
			if (CurrPage < b_dots.Count) { // Safety check.
				Transform currButtonTF = b_dots[CurrPage].transform;
				LeanTween.cancel(i_selectedDot.gameObject);
				LeanTween.moveLocal(i_selectedDot.gameObject, currButtonTF.localPosition, 0.5f).setEaseOutQuint();
			}
		}

	}
}