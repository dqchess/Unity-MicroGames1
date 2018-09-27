using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BouncePaint {
    public class FUEController : MonoBehaviour {
        // Constants
        private const int LEVEL_1 = 1;
        private const int LossFeedbackLastLevelIndex = 10; // We show the "almost! tap sooner" etc. text until this level.
        // Properties
        private bool didSeeHowToPlay = false;
        //private bool isCantLose; // when TRUE, the user can't fail by tapping at the wrong time. ;)
        //private bool doAcceptInput;
        private float timeWhenAcceptInput; // in UNSCALED time.
        private bool isActive; // only true for the first few levels
        private bool isPlayerFrozen;
        private int currentStep;
        private int levelIndex;
        // References
        [SerializeField] private CanvasGroup cg_tapInstructions=null;
        [SerializeField] private GameController gameController=null;
        [SerializeField] private GameObject go_howToPlay=null;
        [SerializeField] private TextMeshProUGUI t_lossFeedback=null;
        //[SerializeField] private TextMeshProUGUI t_tutorialHeader=null;
        private Level level;
        private Player player; // only ever refers to the FIRST ball.

        // Getters (Public)
        //public bool IsCantLose { get { return isCantLose || isPlayerFrozen; } }
        public bool DoAcceptInput { get { return !isActive || Time.unscaledTime>=timeWhenAcceptInput; } }
        public bool DoShowLevelName(int levelIndex) { return levelIndex != LEVEL_1; } // Note: Provide levelIndex for safety.
        public bool IsPlayerFrozen { get { return isPlayerFrozen; } }
        // Getters (Private)
        private bool IsLevelComplete { get { return gameController.IsLevelComplete; } }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnStartLevel(Level _level) {
            level = _level;
            levelIndex = level.LevelIndex;

            // Reset values
            currentStep = 0;
            //isCantLose = false;
            //doAcceptInput = true;
            timeWhenAcceptInput = -1;
            isPlayerFrozen = false;
            cg_tapInstructions.gameObject.SetActive(false);
            t_lossFeedback.enabled = false;
            //t_tutorialHeader.enabled = false;
            go_howToPlay.SetActive(false);

            // Set my player reference.
            if (gameController.Players!=null && gameController.Players.Count>0) {
                player = gameController.Players[0];
            }
            else {
                player = null;
            }

            // Whaddawe gonna do this level??
            if (levelIndex == LEVEL_1) {
                isActive = true;
                //isCantLose = true;
                //doAcceptInput = false;
                timeWhenAcceptInput = Mathf.Infinity; // never accept input until further notice.
                //if (!didSeeHowToPlay) { Note: DISABLED how-to-play image!
                //    isPlayerFrozen = true;
                //    didSeeHowToPlay = true;
                //    go_howToPlay.SetActive(true);
                //}
                //else {
                //t_tutorialHeader.enabled = true;
                    currentStep = 1;
                //}
            }
            // NO FUE for this level.
            else {
                isActive = false;
            }
        }


        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void Update() {
            if (!isActive) { return; } // Not in this level? Do nothin'.
            if (player == null) { return; } // Safety check.

            if (levelIndex == LEVEL_1) {
                if (!isPlayerFrozen && !gameController.IsLevelComplete) {
                    float yBounds = level.Blocks[0].HitBox.yMax - 24f; // HARDCODED offset here. Nbd #fue.
                    if (player.Vel.y<0 && player.BottomY <= yBounds) {
                        // Freeze!
                        //doAcceptInput = true;
                        timeWhenAcceptInput = Time.unscaledTime + 0.2f; // accept input after a quick pause.
                        isPlayerFrozen = true;
                        cg_tapInstructions.gameObject.SetActive(true);
                        currentStep ++;
                    }
                }
                if (cg_tapInstructions.gameObject.activeSelf) {
                    cg_tapInstructions.alpha = MathUtils.SinRange(0.4f,1f, Time.unscaledTime*12f);
                }
            }
        }


        public void OnTapDown() {
            if (levelIndex == LEVEL_1) {
                if (currentStep == 0) {
                    isPlayerFrozen = false;
                    go_howToPlay.SetActive(false);
                    currentStep ++;
                }
            }

        }
        public void OnPlayerPaintBlock() {
            if (levelIndex == LEVEL_1) {
                // Unfreeze!
                //doAcceptInput = false;
                timeWhenAcceptInput = Mathf.Infinity; // don't accept input until the Player lands on the next Blocko!
                isPlayerFrozen = false;
                cg_tapInstructions.gameObject.SetActive(false);
                currentStep ++;
            }
        }

        public void OnSetGameOver(LoseReasons reason) {
            if (levelIndex <= LossFeedbackLastLevelIndex) {
                t_lossFeedback.enabled = true;
                t_lossFeedback.text = GetLoseFeedbackText(reason);
            }
        }
        private string GetLoseFeedbackText(LoseReasons reason) {
            switch (reason) {
                case LoseReasons.MissedTap: return "ooh! tap sooner.";
                case LoseReasons.TapEarly: return "almost!\ntap later.";
                case LoseReasons.TappedDontTap: return "don't tap these blocks.";
                default: return "";
            }
        }



    }
}
