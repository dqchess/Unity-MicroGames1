﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum LoseReasons { Undefined, IllegalOverlap, InsufficientScore }

    public class GameController : BaseLevelGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.CircleGrow; }
        // Components
        [SerializeField] private LevelLoader levelLoader;
        private Level level;
        // Properties
        private LoseReasons loseReason;
        private int scorePossible; // includes the Grower that's currently growing!
        private int scoreSolidified; // ONLY includes Growers that've been solidified.
        


        // Getters (Public)
        public LevelLoader LevelLoader { get { return levelLoader; } }
        //public Rect r_LevelBounds { get { return r_levelBounds; } }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            //levelBounds = new Rect(-300,-300, 600,600);
            //i_levelBounds.anchoredPosition = levelBounds.position;
            //i_levelBounds.sizeDelta = levelBounds.size;
//          r_levelBounds = i_levelBounds.rectTransform.rect;

            // Initialize LevelLoader!
            levelLoader.ReloadLevelsFile();

            base.Start();
        }




        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void UpdateScore() {
            scorePossible = 0;
            scoreSolidified = 0;
            foreach (Grower g in level.Growers) {
                scorePossible += g.ScoreValue();
                if (g.CurrentState==GrowerStates.Solidified) {
                    scoreSolidified += g.ScoreValue();
                }
            }
            // Update the UI!
            level.UpdateScoreUI(scorePossible, scoreSolidified);
        }





        // ----------------------------------------------------------------
        //  Game Flow Events
        // ----------------------------------------------------------------
        public void OnIllegalOverlap() {
            if (IsGameStatePlaying) { // If we're playing...!
                loseReason = LoseReasons.IllegalOverlap;
                LoseLevel();
            }
        }

        override public void LoseLevel() {
            base.LoseLevel();
            // Tell people!
            level.OnLoseLevel(loseReason);
        }
        override protected void WinLevel() {
            base.WinLevel();
            StartCoroutine(Coroutine_StartNextLevel());
            // Tell people!
            level.OnWinLevel();
			// Update best score!
			int bestScore = SaveStorage.GetInt(SaveKeys.BestScore(MyGameName(), LevelIndex));
			if (scoreSolidified > bestScore) {
				SaveStorage.SetInt(SaveKeys.BestScore(MyGameName(),LevelIndex), scoreSolidified);
			}
        }

        private IEnumerator Coroutine_StartNextLevel() {
            yield return new WaitForSecondsRealtime(1.2f);
            SetCurrentLevel(LevelIndex+1, true);
        }

        override protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false) {
            StopCoroutine("Coroutine_SetCurrentLevel");
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

                level.IsAnimating = true;
                prevLevel.IsAnimating = true;
                float height = 1200;
                Vector3 levelDefaultPos = level.transform.localPosition;
                level.transform.localPosition += new Vector3(0, height, 0);
                LeanTween.moveLocal(level.gameObject, levelDefaultPos, duration).setEaseInOutQuart();
                LeanTween.moveLocal(prevLevel.gameObject, new Vector3(0, -height, 0), duration).setEaseInOutQuart();
                yield return new WaitForSeconds(duration);

                level.IsAnimating = false;
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

            yield return null;
        }


        public void OnAllGrowersSolidified() {
            bool didWin = scoreSolidified >= level.ScoreRequired;
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
				level.OnTapScreen();
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
