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
        [SerializeField] private TextMeshProUGUI t_collectionName=null;
        [SerializeField] private LevelCompletePopup levelCompletePopup=null;
		// References
		[SerializeField] private Level level=null;
        
        // Getters (Public)
        public LevelCompletePopup LevelCompletePopup { get { return levelCompletePopup; } }
        // Getters (Private)
        private string GetCollectionName(int collectionIndex) {
            switch (collectionIndex) {
                case 0: return "easy";
                case 1: return "medium";
                case 2: return "hard";
                default: return "undefined";
            }
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
			t_levelName.text = "LEVEL " + (level.MyAddress.level+1).ToString();
            t_collectionName.text = GetCollectionName(level.MyAddress.collection);
            
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