using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BouncePaint {
    public class FUEController : MonoBehaviour {
        // Constants
        private const int LEVEL_1 = 1;
        private const int LEVEL_2 = 2;
        // Properties
        private bool didSeeHowToPlay = false;
        private bool isActive; // only true for the first few levels
        private bool isPlayerFrozen;
        private int currentStep;
        private int levelIndex;
        // References
        [SerializeField] private GameController gameController;
        [SerializeField] private TextMeshProUGUI t_instructions;
        [SerializeField] private TextMeshProUGUI t_lossFeedback;
        [SerializeField] private GameObject go_howToPlay;
        private Level level;
        private Player player; // only ever refers to the FIRST ball.

        // Getters (Public)
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
            isPlayerFrozen = false;
            t_instructions.enabled = false;
            t_lossFeedback.enabled = false;
            go_howToPlay.SetActive(false);

            // Set my player reference.
            if (gameController.Players!=null && gameController.Players.Count>0) {
                player = gameController.Players[0];
            }
            else {
                player = null;
            }

            // Whaddawe gonna do this level??
            if (levelIndex == 1) {
                isActive = true;
                if (!didSeeHowToPlay) {
                    isPlayerFrozen = true;
                    didSeeHowToPlay = true;
                    go_howToPlay.SetActive(true);
                }
                else {
                    currentStep = 1;
                }
            }
            // NO FUE for this level.
            else {
                isActive = false;
            }
        }


        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate() {
            if (!isActive) { return; } // Not in this level? Do nothin'.
            if (player == null) { return; } // Safety check.

            if (levelIndex == LEVEL_1) {
                if (currentStep==1) {
                    float yBounds = level.Blocks[0].HitBox.yMax - 5f;
                    if (player.BottomY <= yBounds) {
                        // Freeze!
                        isPlayerFrozen = true;
                        t_instructions.enabled = true;
                        t_instructions.text = "tap now";
                        currentStep ++;
                    }
                }
            }
        }


        public void OnTapScreen() {
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
                isPlayerFrozen = false;
                t_instructions.enabled = false;
                currentStep ++;
            }
        }

        public void OnSetGameOver(LoseReasons reason) {
            if (levelIndex <= 2) {
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
