using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform rt_levelNameTexts=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private TextMeshProUGUI t_packName=null;
        [SerializeField] private LevelCompletePopup levelCompletePopup=null;
		// References
		[SerializeField] private Level level=null;
        
        // Getters (Public)
        public LevelCompletePopup LevelCompletePopup { get { return levelCompletePopup; } }
        // Getters (Private)
        private string GetCollectionName() {
            PackCollectionData data = LevelsManager.Instance.GetPackCollectionData(level.MyAddress);
            return data.CollectionName;
        }
        private string GetPackName() {
            PackData data = LevelsManager.Instance.GetPackData(level.MyAddress);
            return data.PackName;
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
            // Update texts!
            string levelStr = "LEVEL " + (level.MyAddress.level+1).ToString();
            if (level.Board.DidRandGen) { levelStr += " (RAND)"; }
            string packStr = GetCollectionName();// + ",  " + GetPackName();// + " (D" + level.Board.Difficulty + ")";
            t_levelName.text = levelStr;
            t_packName.text = packStr;
            
            levelCompletePopup.Hide();
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnBoardMade() {
            // Center levelName texts GO between top of screen and board.
            float x = rt_levelNameTexts.anchoredPosition.x;
            float y = level.BoardView.MyRectTransform.anchoredPosition.y*0.5f;
            rt_levelNameTexts.anchoredPosition = new Vector2(x,y);
        }
        
        public void OnLevelAnimateIn() {
            // Move my texts OUT of place, then tween them back!
            Vector2 posPackName = t_packName.rectTransform.localPosition;
            Vector2 posLevelName = t_levelName.rectTransform.localPosition;
            Vector3 offsetDir = Level.animInOutOffset.normalized;
            t_packName.transform.localPosition += offsetDir * 300;
            t_levelName.transform.localPosition += offsetDir * 300;
            LeanTween.moveLocal(t_packName.gameObject, posPackName, 2.2f).setDelay(0.4f).setEaseOutQuint();
            LeanTween.moveLocal(t_levelName.gameObject, posLevelName, 2.2f).setDelay(0.6f).setEaseOutQuint();
        }
        public void OnLevelAnimateOut() {
            // Tell LevelCompletePopup.
            levelCompletePopup.OnLevelAnimateOut();
            // Move my texts OUT of place, then tween them back!
            Vector3 offsetDir = Level.animInOutOffset.normalized;
            Vector2 posPackTo = t_packName.rectTransform.localPosition + offsetDir*400;
            Vector2 posLevelTo = t_levelName.rectTransform.localPosition + offsetDir*400;
            LeanTween.moveLocal(t_packName.gameObject, posPackTo, 0.6f).setEaseInQuart().setDelay(0.05f);
            LeanTween.moveLocal(t_levelName.gameObject, posLevelTo, 0.6f).setEaseInQuart();
        }


	}
}