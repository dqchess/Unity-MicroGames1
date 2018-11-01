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
        [SerializeField] private TextMeshProUGUI t_packName=null;
        [SerializeField] private LevelCompletePopup levelCompletePopup=null;
		// References
		[SerializeField] private Level level=null;
        
        // Getters (Public)
        public LevelCompletePopup LevelCompletePopup { get { return levelCompletePopup; } }
        // Getters (Private)
        private string GetPackName() {
            PackData packData = LevelsManager.Instance.GetPackData(level.MyAddress);
            return packData.PackName;
            //switch (collectionIndex) {
            //    case 0: return "TESTS";
            //    case 1: return "easy";
            //    case 2: return "medium";
            //    case 3: return "hard";
            //    default: return "undefined";
            //}
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
            t_levelName.text = "LEVEL " + (level.MyAddress.level+1).ToString();
            if (level.Board.DidRandGen) { t_levelName.text += " (RAND)"; }
            t_packName.text = GetPackName();
            
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