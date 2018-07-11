using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BouncePaint {
    public class FUEController : MonoBehaviour {
        // Constants
        private const int LEVEL_1 = 1;
        private const int LEVEL_2 = 2;
        // Properties
        private bool isActive; // only true for the first few levels
        private int levelIndex;
        // References
        [SerializeField] private GameController gameController;
        private Player player; // only ever refers to the FIRST ball.

        // Getters (Private)
        private bool IsLevelComplete { get { return gameController.IsLevelComplete; } }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnStartLevel(int _levelIndex) {
            levelIndex = _levelIndex;

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
                if (!IsLevelComplete) {
                    //float yBounds = level.Blocks[0]
                    //if (player.)
                }
            }
        }



    }
}
