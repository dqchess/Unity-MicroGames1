﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum LoseReasons { Undefined, CircleOverlap, InsufficientScore }

    public class GameController : BaseLevelGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.CircleGrow; }
        // Components
        [SerializeField] private GameUI gameUI=null;
//        [SerializeField] private Image i_levelBounds=null;
        private Level level;
        // Properties
        private LoseReasons loseReason;
        private int score;
        


        // Getters (Public)
        //public Rect r_LevelBounds { get { return r_levelBounds; } }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            //levelBounds = new Rect(-300,-300, 600,600);
            //i_levelBounds.anchoredPosition = levelBounds.position;
            //i_levelBounds.sizeDelta = levelBounds.size;
//          r_levelBounds = i_levelBounds.rectTransform.rect;

            base.Start();
        }




        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScore() {
            score = 0;
            foreach (Circle c in level.Circles) {
                if (c.CurrentState != CircleStates.Solidified) { continue; } // Skip ones that haven't been solidified, of course.
                score += c.ScoreValue();
            }
            // Update the UI!
            gameUI.SetScoreText(score);
        }





        // ----------------------------------------------------------------
        //  Game Flow Events
        // ----------------------------------------------------------------
        public void OnCircleIllegalOverlap() {
            loseReason = LoseReasons.CircleOverlap;
            LoseLevel();
        }

        override public void LoseLevel() {
            base.LoseLevel();
            // Tell people!
            gameUI.OnLoseLevel(loseReason);
        }
        override protected void WinLevel() {
            base.WinLevel();
            StartCoroutine(Coroutine_StartNextLevel());
            // Tell people!
            gameUI.OnWinLevel();
        }

        private IEnumerator Coroutine_StartNextLevel() {
            yield return new WaitForSecondsRealtime(1.2f);
            SetCurrentLevel(LevelIndex+1, true);
        }

        override protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false) {
            StartCoroutine(Coroutine_SetCurrentLevel(_levelIndex, doAnimate));
        }
        override protected void InitializeLevel(GameObject _levelGO, int _levelIndex) {
            level = _levelGO.GetComponent<Level>();
            level.Initialize(this, canvas.transform, _levelIndex);
            base.OnInitializeLevel(level);
        }
        private IEnumerator Coroutine_SetCurrentLevel(int _levelIndex, bool doAnimate) {
            Level prevLevel = level;

            // Reset basics.
            loseReason = LoseReasons.Undefined;

            // Make the new level!
            InitializeLevel(Instantiate(resourcesHandler.circleGrow_level), _levelIndex);

            // DO animate!
            if (doAnimate) {
                float duration = 1f;

                level.IsAnimatingIn = true;
                float height = 1200;
                Vector3 levelDefaultPos = level.transform.localPosition;
                level.transform.localPosition += new Vector3(0, height, 0);
                LeanTween.moveLocal(level.gameObject, levelDefaultPos, duration).setEaseInOutQuart();
                LeanTween.moveLocal(prevLevel.gameObject, new Vector3(0, -height, 0), duration).setEaseInOutQuart();
                yield return new WaitForSeconds(duration);

                level.IsAnimatingIn = false;
                if (prevLevel!=null) {
                    Destroy(prevLevel.gameObject);
                }
            }
            // DON'T animate? Ok, just destroy the old level.
            else {
                if (prevLevel!=null) {
                    Destroy(prevLevel.gameObject);
                }
            }

            // Update the UI!
            gameUI.OnStartLevel(level.ScoreRequired);

            yield return null;
        }


        public void OnAllCirclesSolidified() {
            bool didWin = score >= level.ScoreRequired;
            if (didWin) {
                WinLevel();
            }
            else {
                loseReason = LoseReasons.InsufficientScore;
                LoseLevel();
            }
        }



        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        override protected void OnTapScreen() {
            // Paused? Ignore input.
            if (Time.timeScale == 0f) { return; }

            if (IsGameStatePlaying) {
                level.SolidifyCurrentCircle();
            }
        }



        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        override protected void Debug_WinLevel() {
            WinLevel();
        }




    }
}

/*
        private void SolidifyOscillatingCircles() {
            // Make 'em stop oscillating!
            for (int i=circles.Count-1; i>=0; --i) {
                if (circles[i].IsOscillating) {
                    SolidifyCircle(circles[i]);
                }
            }
            // If we didn't lose, add a new circle!
            if (IsGameStatePlaying) {
                AddNewCircle();
            }
        }
        */
