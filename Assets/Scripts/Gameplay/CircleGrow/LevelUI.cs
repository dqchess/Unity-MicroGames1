using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class LevelUI : MonoBehaviour {
        // Components
        [SerializeField] private GameObject go_scoreBar=null;
        //[SerializeField] private Image i_barBorder=null;
        [SerializeField] private Image i_barFillPossible=null;
        [SerializeField] private Image i_barFillSolidified=null;
        [SerializeField] private Image i_fullBacking=null; // the solid square image that's behind EVERYthing (gameplay AND UI)
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        [SerializeField] public  TextMeshProUGUI t_moreLevelsComingSoon=null;
        [SerializeField] private TextMeshProUGUI t_score=null;
        [SerializeField] private TextMeshProUGUI t_scoreRequired=null;
        // References
        [SerializeField] private Level myLevel=null;
        // Properties
        //private readonly Color barColor_solid = Grower.color_solid;
        //private readonly Color barColor_possible = Grower.color_growing;
        private Vector2 scoreBarSize; // set in Initialize.

        // Getters (Private)
        private int levelIndex { get { return myLevel.LevelIndex; } }
        private int scoreRequired { get { return myLevel.ScoreRequired; } }



        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
            // Define scoreBarSize! Make it exactly my size.
            scoreBarSize = go_scoreBar.GetComponent<RectTransform>().rect.size;
            // Color elements right-o.
            //i_barBorder.color = barColor_solid;
            //i_barFillPossible.color = barColor_possible;
            //i_barFillSolidified.color = barColor_solid;
            //t_scoreRequired.color = barColor_solid;
            i_fullBacking.color = Grower.color_solid;

            // LevelIndex
            t_levelName.text = "LEVEL " + levelIndex.ToString();
            // Score
            t_scoreRequired.text = TextUtils.AddCommas(scoreRequired);
            //UpdateScoreUI(0);
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScoreUI(float scorePossible, float scoreSolidified) {
            t_score.text = TextUtils.AddCommas(scorePossible);
            // Update bars!
            float fillPossibleWidth = scoreBarSize.x * Mathf.Min(1, scorePossible/(float)scoreRequired);
            float fillSolidifiedWidth = scoreBarSize.x * Mathf.Min(1, scoreSolidified/(float)scoreRequired);
            i_barFillPossible.rectTransform.sizeDelta = new Vector2(fillPossibleWidth, scoreBarSize.y);
            i_barFillSolidified.rectTransform.sizeDelta = new Vector2(fillSolidifiedWidth, scoreBarSize.y);
            // Update color!
            if (scorePossible >= scoreRequired) { // We've potentially won already!!
                t_score.color = Color.green;
            }
            else { // Haven't won yet...!
                t_score.color = Color.white;
            }
        }


        // ----------------------------------------------------------------
        //  Game Flow Events
        // ----------------------------------------------------------------
        public void OnLoseLevel(LoseReasons loseReason) {
            // Lost because not high enough score? Make score text red!
            if (loseReason == LoseReasons.InsufficientScore) {
                t_score.color = new Color(244/255f, 23/255f, 80/255f);
            }
        }
        public void OnWinLevel() {
        }

    }
}
