using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
		[SerializeField] private TextMeshProUGUI t_levelName=null;
		// References
		[SerializeField] private Level level=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
			t_levelName.text = "LEVEL " + level.MyAddress.level.ToString();
        }
        public void OnBoardMade() {
            // Center levelName text between top of screen and board.
            t_levelName.rectTransform.anchoredPosition = new Vector2(0, level.BoardView.MyRectTransform.anchoredPosition.y*0.5f);
		}


	}
}