using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private LevelCompletePopup levelCompletePopup=null;
		// References
		[SerializeField] private Level level=null;
        
        // Getters (Public)
        public LevelCompletePopup LevelCompletePopup { get { return levelCompletePopup; } }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
			t_levelName.text = "LEVEL " + level.MyAddress.level.ToString();
            levelCompletePopup.Hide();
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnBoardMade() {
            // Center levelName text between top of screen and board.
            t_levelName.rectTransform.anchoredPosition = new Vector2(0, level.BoardView.MyRectTransform.anchoredPosition.y*0.5f);
		}
        public void OnWinLevel() {
            // TODO: Disable buttons!
        }


	}
}