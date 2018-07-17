using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class GameUI : MonoBehaviour {
        // Components
        [SerializeField] private Text t_score=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start () {
            //SetScoreText(0);
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnStartLevel() {
            SetScoreText(0);
        }

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetScoreText(float score) {
            score = Mathf.Round(score); // no decimal places, ok?
            t_score.text = score.ToString();
        }


    }
}