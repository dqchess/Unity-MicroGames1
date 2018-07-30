using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CircleGrow {
	public class GameUI : MonoBehaviour {
        // Components
        [SerializeField] private TextMeshProUGUI t_score;
        [SerializeField] private TextMeshProUGUI t_scoreRequired;



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetScoreText(float score) {
            //t_score.text = Mathf.RoundToInt(score).ToString();
            t_score.text = TextUtils.AddCommas(score);
        }


        // ----------------------------------------------------------------
        //  Game Flow Events
        // ----------------------------------------------------------------
        public void OnStartLevel(float scoreReq) {
            t_scoreRequired.text = TextUtils.AddCommas(scoreReq);
            t_score.color = Color.white;
            t_scoreRequired.color = Color.white;
        }

        public void OnLoseLevel(LoseReasons loseReason) {
            // Lost because not high enough score? Make score text red!
            if (loseReason == LoseReasons.InsufficientScore) {
                t_score.color = new Color(255/255f, 132/255f, 118/255f);
            }
        }
        public void OnWinLevel() {
            t_score.color = Color.green;
        }


	}
}