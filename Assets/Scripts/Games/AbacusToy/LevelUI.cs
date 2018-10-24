using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AbacusToy {
	public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform rt_levelName=null;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] private TextMeshProUGUI t_numMovesMade=null;
        [SerializeField] private TextMeshProUGUI t_par=null;
		// References
		[SerializeField] private Level level=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
            t_levelName.text = "LEVEL " + (level.MyAddress.level+1).ToString();
            t_par.text = "par: " + level.Board.ParMoves.ToString();
            //UpdateNumMovesMadeText();
		}
        

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void OnNumMovesMadeChanged() {
            UpdateNumMovesMadeText();
        }
        private void UpdateNumMovesMadeText() {
            t_numMovesMade.text = "moves: " + level.NumMovesMade.ToString();
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