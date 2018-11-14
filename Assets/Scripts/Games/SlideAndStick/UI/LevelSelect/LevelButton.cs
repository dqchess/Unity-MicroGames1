using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelButton : MonoBehaviour {
        // Components
        [SerializeField] private CanvasGroup myCanvasGroup=null;
        [SerializeField] private Image i_backing=null;
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        // Properties
        private Vector2 posNeutral;
        private int col,row;
        private int myButtonIndex,myPageIndex;
        // References
        [SerializeField] private Sprite s_completed=null;
        [SerializeField] private Sprite s_notCompleted=null;
        private LevelData myLevelData;
        private PackSelectMenu packSelectMenu;
        
        // Setters (Private)
        private Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        
        
        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(PackSelectMenu _psm, Transform tf_parent, int _buttonIndex,int _pageIndex, int _col,int _row) {
            this.packSelectMenu = _psm;
            this.myButtonIndex = _buttonIndex;
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
        public void Spawn(LevelData _levelData) {//, int _col,int _row, int _myPageIndex) {
            this.gameObject.SetActive(true);
            this.myLevelData = _levelData;
            
            int levelIndexDisplay = myLevelData.myAddress.level + 1;
            
            this.name = "Level_" + levelIndexDisplay;
            t_levelName.text = levelIndexDisplay.ToString();
            
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
        
        // ----------------------------------------------------------------
        //  Animating In/Out
        // ----------------------------------------------------------------
        public void OnSetCurrPage(int currPage) {
            bool doAppear = myPageIndex == currPage;
            int pageDiff = myPageIndex - currPage;
            float duration = 0.3f + (col+row)*0.07f;
            float alphaTarget = doAppear ? 1 : 0;
            LeanTween.cancel(this.gameObject);
            LeanTween.value(gameObject, SetAlpha, myCanvasGroup.alpha,alphaTarget, duration).setEaseOutQuart();
            float targetX = posNeutral.x + pageDiff*200;
            LeanTween.value(gameObject, SetPosX, Pos.x,targetX, duration).setEaseOutQuart();
        }
        
        
        // ----------------------------------------------------------------
        //  Button Events
        // ----------------------------------------------------------------
        public void OnClick() {
            packSelectMenu.OnClickLevelButton(myLevelData.myAddress);
        }
    }
}