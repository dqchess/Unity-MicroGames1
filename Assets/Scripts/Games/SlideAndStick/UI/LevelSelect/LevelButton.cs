using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelButton : MonoBehaviour {
        // Components
//		[SerializeField] private Button myButton=null;
        [SerializeField] private CanvasGroup myCanvasGroup=null;
        [SerializeField] private Image i_backing=null;
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        // Properties
        private Vector2 posNeutral;
        private int col,row;
//		private int myButtonIndex;
		private int myPageIndex;
        // References
        [SerializeField] private Sprite s_completed=null;
        [SerializeField] private Sprite s_notCompleted=null;
        private LevelData myLevelData;
        private PackSelectMenu packSelectMenu;
        
		// Getters / Setters (Private)
		private float Scale { get { return myRectTransform.localScale.x; } }
		private void SetScale(float _scale) {
			myRectTransform.localScale = new Vector3(_scale,_scale,_scale);
		}
        private Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
		}
		private float GetTargetPosX(int currPage) {
			int pageDiff = myPageIndex - currPage;
			return posNeutral.x + pageDiff*200;
		}

        
        
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(PackSelectMenu _psm, Transform tf_parent, int _buttonIndex,int _pageIndex, int _col,int _row) {
            this.packSelectMenu = _psm;
//            this.myButtonIndex = _buttonIndex;
            this.myPageIndex = _pageIndex;
            this.col = _col;
            this.row = _row;
            
            // Parent jazz!
            GameUtils.ParentAndReset(this.gameObject, tf_parent);
            
            // Start hidden.
            SetAlpha(0);
        }
        
        
        // ----------------------------------------------------------------
        //  Spawn / Despawn
        // ----------------------------------------------------------------
        public void Despawn() {
            this.gameObject.SetActive(false);
        }
		public void Spawn(LevelData _levelData, int currPage) {//, int _col,int _row, int _myPageIndex) {
            this.gameObject.SetActive(true);
            this.myLevelData = _levelData;
            
            int levelIndexDisplay = myLevelData.myAddress.level + 1;
            
            this.name = "Level_" + levelIndexDisplay;
            t_levelName.text = levelIndexDisplay.ToString();
			UpdateInteractivityFromCurrPage(currPage);
            
            // Completed-ness visuals!
            if (myLevelData.DidCompleteLevel) {
                i_backing.sprite = s_completed;
                i_backing.color = packSelectMenu.CurrentPackColor;
                t_levelName.color = Color.white;
            }
            else {
                i_backing.sprite = s_notCompleted;
                i_backing.color = packSelectMenu.CurrentPackColor;//new Color(0.65f,0.65f,0.65f);
                t_levelName.color = packSelectMenu.CurrentPackColor;
            }

			// Pre-animate me!
			//LeanTween.cancel(this.gameObject);
			SetAlpha(0);
			float targetX = GetTargetPosX(currPage);
			SetPosX(targetX);
			// Tween in!
			bool isMyPage = myPageIndex == currPage;
			if (isMyPage && gameObject.activeInHierarchy) {
				// Animate!
				if (c_animateAlpha != null) { StopCoroutine(c_animateAlpha); }
				if (c_animateScale != null) { StopCoroutine(c_animateScale); }
				SetScale(0.5f);
				float easing = 0.2f;
				float delay = (col+row)*0.04f;
				c_animateAlpha = StartCoroutine(Coroutine_AnimateAlpha(1, easing,delay));
				c_animateScale = StartCoroutine(Coroutine_AnimateScale(1, easing,delay));
//				LeanTween.value(gameObject, SetAlpha, myCanvasGroup.alpha,1, duration).setEaseOutQuart().setDelay(delay);
//				LeanTween.value(gameObject, SetScale, Scale,1, duration).setEaseOutBack().setDelay(delay);
			}
		}
        
        public void UpdatePosSize(Vector2 _unitSize, Vector2 _size) {
            posNeutral = new Vector2(col*_unitSize.x, -row*_unitSize.y);
            Pos = posNeutral;
            myRectTransform.sizeDelta = _size;
        }
        private void SetAlpha(float _alpha) {
            myCanvasGroup.alpha = _alpha;
        }
        private void SetPosX(float posX) {
            Pos = new Vector2(posX, Pos.y);
        }
		private void UpdateInteractivityFromCurrPage(int currPage) {
			bool isMyPage = myPageIndex == currPage;
			myCanvasGroup.blocksRaycasts = myCanvasGroup.interactable = isMyPage;
		}


		// ----------------------------------------------------------------
		//  Button Events
		// ----------------------------------------------------------------
		public void OnClick() {
			packSelectMenu.OnClickLevelButton(myLevelData.myAddress);
		}

        
        // ----------------------------------------------------------------
        //  Animating In/Out
        // ----------------------------------------------------------------
		public void OnSetCurrPage(int currPage) {
			bool isMyPage = myPageIndex == currPage;
			// Update my interactivity!
			UpdateInteractivityFromCurrPage(currPage);

            if (!gameObject.activeInHierarchy) { return; } // Oh, if I'm not active in the hierarchy, don't animate.

			// Animate!
			if (c_animateAlpha != null) { StopCoroutine(c_animateAlpha); }
			if (c_animatePos != null) { StopCoroutine(c_animatePos); }
//			float duration = 0.3f + (col+row)*0.06f;
			float easing = 0.2f - (col+row)*0.01f;
			float delay = (col+row)*0.011f;
			float targetAlpha = isMyPage ? 1 : 0;
			float targetX = GetTargetPosX(currPage);
			c_animateAlpha = StartCoroutine(Coroutine_AnimateAlpha(targetAlpha, easing,delay));
			c_animatePos   = StartCoroutine(Coroutine_AnimatePos(new Vector2(targetX,Pos.y), easing,delay));

//            LeanTween.cancel(this.gameObject);
//			LeanTween.value(gameObject, SetAlpha, myCanvasGroup.alpha,alphaTarget, duration).setEaseOutQuart().setDelay(delay);
//			float targetX = GetTargetPosX(currPage);
//			LeanTween.value(gameObject, SetPosX, Pos.x,targetX, duration).setEaseOutQuart().setDelay(delay);
        }
        

		private Coroutine c_animateAlpha;
		private Coroutine c_animatePos;
		private Coroutine c_animateScale;
		private IEnumerator Coroutine_AnimateAlpha(float targetAlpha, float easing,float delay) {
			if (delay>0) { yield return new WaitForSeconds(delay); }
			while (myCanvasGroup.alpha != targetAlpha) {
				myCanvasGroup.alpha += (targetAlpha-myCanvasGroup.alpha) * easing;
				if (Mathf.Approximately(myCanvasGroup.alpha, targetAlpha)) {
					myCanvasGroup.alpha = targetAlpha;
					break;
				}
				yield return null;
			}
		}
		private IEnumerator Coroutine_AnimatePos(Vector2 targetPos, float easing,float delay) {
			if (delay>0) { yield return new WaitForSeconds(delay); }
			while (Pos != targetPos) {
				Pos += (targetPos-Pos) * easing;
				if (Vector2.Distance(Pos,targetPos) < 0.1f) {
					Pos = targetPos;
					break;
				}
				yield return null;
			}
		}
		private IEnumerator Coroutine_AnimateScale(float targetScale, float easing,float delay) {
			if (delay>0) { yield return new WaitForSeconds(delay); }
			while (Scale != targetScale) {
				SetScale (Scale + (targetScale-Scale) * easing);
				if (Mathf.Approximately(Scale, targetScale)) {
					SetScale(targetScale);
					break;
				}
				yield return null;
			}
		}
    }
}