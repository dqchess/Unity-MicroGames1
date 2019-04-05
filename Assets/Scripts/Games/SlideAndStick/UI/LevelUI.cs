using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform rt_levelName=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private TextMeshProUGUI t_levelNameShadow=null;
        [SerializeField] private TextMeshProUGUI t_packName=null;
        [SerializeField] private TextMeshProUGUI t_packNameShadow=null;
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
            t_levelName.text = t_levelNameShadow.text = levelStr;
            t_packName.text = t_packNameShadow.text = packStr;
            
            levelCompletePopup.Hide();
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnBoardMade() {
            // Center levelName texts between top of screen and board.
            float x = rt_levelName.anchoredPosition.x;
            float y = level.BoardView.MyRectTransform.anchoredPosition.y*0.5f;
            rt_levelName.anchoredPosition = new Vector2(x,y);
        }


	}
}