using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CircleGrow {
    public class FUEController : MonoBehaviour {
        // Constants
        private const int LEVEL_1 = 1;
        private const int LEVEL_3 = 3;
        // Properties
        private bool doIgnoreTaps;
        private bool isActive; // only true for the first few levels
        private bool isGameplayFrozen;
		private float timestamp; // in UNSCALED SECONDS. Set to Time.unscaledTime whenever you want! Use whenever you want. (Useful for setting delays or waiting for the next step.)
        private int currentStep;
        private int levelIndex;
        // Components
        [SerializeField] private CanvasGroup cg_tap=null;
        [SerializeField] private GameObject go_loseFeedback=null;
        [SerializeField] private TextMeshProUGUI t_loseFeedback=null;
        // References
        [SerializeField] private GameController gameController=null;
        private Level level;

        // Getters (Public)
        public bool DoIgnoreTaps { get { return doIgnoreTaps; } }
        public bool IsGameplayFrozen { get { return isGameplayFrozen; } }
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
            doIgnoreTaps = false;
            isGameplayFrozen = false;
            cg_tap.gameObject.SetActive(false);
            go_loseFeedback.SetActive(false);

            // Whaddawe gonna do this level??
            if (levelIndex == LEVEL_1) {
                isActive = true;
                isGameplayFrozen = true;
                cg_tap.gameObject.SetActive(true); // Show "Tap!"
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

            if (levelIndex == LEVEL_1) {
                // Always oscillate the "TAP" image's alpha (yeah, even when it's not visible, nbd).
                cg_tap.alpha = MathUtils.SinRange(0.5f, 1f, Time.time*15f);

                // Waiting for user to do first TAP.
                if (currentStep == 0) { }
                // The Grower is growin'!
                else if (currentStep==1) {
                    if (gameController.ScorePossible >= level.ScoreRequired) { // We've hit the target score!!
						isGameplayFrozen = true; // Freeze!
						cg_tap.gameObject.SetActive(true); // Show "Tap!"
						timestamp = Time.unscaledTime;
                        currentStep ++;
                    }
                }
				// We JUST froze time for the player-- allow taps in a moment!
				else if (currentStep==2) {
					if (Time.unscaledTime > timestamp+0.4f) {
						doIgnoreTaps = false;
						currentStep ++;
					}
				}
            }
        }


        public void OnTapScreen() {
            if (levelIndex == LEVEL_1) {
                if (currentStep == 0) {
                    doIgnoreTaps = true; // ignore any taps until we've hit the target score.
                    cg_tap.gameObject.SetActive(false);
                    isGameplayFrozen = false;
                    currentStep ++;
                }
                else if (currentStep == 3) {
                    cg_tap.gameObject.SetActive(false);
                    isGameplayFrozen = false;
                    currentStep ++;
                }
            }

        }
        //public void OnPlayerPaintBlock() {
        //    if (levelIndex == LEVEL_1) {
        //        // Unfreeze!
        //        isGameplayFrozen = false;
        //        t_instructions.enabled = false;
        //        currentStep ++;
        //    }
        //}

        public void OnSetGameOver(LoseReasons reason) {
            if (levelIndex <= 3) {
                go_loseFeedback.SetActive(true);
                t_loseFeedback.text = GetLoseFeedbackText(reason);
            }
        }
        private string GetLoseFeedbackText(LoseReasons reason) {
            switch (reason) {
                case LoseReasons.IllegalOverlap: return "almost!\ntap SOONER.";
                case LoseReasons.InsufficientScore: return "not big enough!\ntap LATER.";
                default: return "";
            }
        }



    }
}
