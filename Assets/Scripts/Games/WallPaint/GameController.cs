using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WallPaint {
    public enum GameStates { PreLevel, Playing, PostLevel, GameOver }

    public class GameController : BaseGameController {
        // Properties
        private GameStates gameState;
        private int currentLevelIndex;
        //private int numCols,numRows;
        // Components
        [SerializeField] private GameObject go_board;
		[SerializeField] private Player player;
        //[SerializeField] private WallSpace[] wallSpaces;
        //private BoardSpace[,] boardSpaces;
        // References
        [SerializeField] private GameUI ui;



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            SetCurrentLevel(0);
        }


        private void InitializeBoard() {
            DestroyBoard(); // Just in case

            Rect boardRect = new Rect();
            boardRect.size = new Vector2(64,78);
            boardRect.center = new Vector2(0,0);

            //boardSpaces = new BoardSpace[numCols,numRows];
        }
        private void DestroyBoard() {
            //if (wallSpaces!=null) {
            //    foreach (WallSpace space in wallSpaces) {
            //        Destroy(space.gameObject);
            //    }
            //}
            //wallSpaces = null;
        }


        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            currentLevelIndex = _levelIndex;

            // Set basics!
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.1f,0.1f,0.1f);

            // Tell the UI!
            ui.UpdateLevelName(currentLevelIndex);

            // Set properties!
            //numCols = 9;
            //numRows = 9;

            // Initialize spaces!
            InitializeBoard();
        }
        private void SetGameOver() {
            gameState = GameStates.GameOver;
            SetIsPaused(true);
            //Camera.main.backgroundColor = new Color(0.6f,0.1f,0.1f);
        }

        private void OnCompleteLevel() {
            gameState = GameStates.PostLevel;
            //Camera.main.backgroundColor = new Color(0.1f,0.8f,0.1f);
//          StartCoroutine(Coroutine_CompleteLevel());
//      }
//      private IEnumerator Coroutine_CompleteLevel() {
//          // Set properties and wait a brief moment!
//          gameState = GameStates.PostLevel;
////            i_correctIcon.enabled = true;
//          yield return new WaitForSecondsRealtime(0.3f);
//
//          // Wait for click!
//          while (true) {
//              if (Input.GetMouseButtonDown(0)) { break; }
//              yield return null;
//          }
//
//          // After click, start the next level!
//          StartNextLevel();
//          yield return null;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        override protected void Update () {
            base.Update();

            RegisterMouseInput();
        }

        private void RegisterMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }



        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        private void OnMouseDown() {
            if (gameState == GameStates.GameOver) {
                ReloadScene();
                return;
            }
            else if (gameState == GameStates.PostLevel) {
                StartNextLevel();
                return;
            }
        }


        // ----------------------------------------------------------------
        //  Game Events
        // ----------------------------------------------------------------
        //public void OnTapSpaceClicked(TapSpace tapSpace) {
        //    if (!tapSpace.CanTapMe) { return; } // Discard non-tappable spaces.

        //    // HACk. Allow tapping either end first.
        //    if (nextSpaceNumber==0 && tapSpace.MyNumber==tapSpaces.Length-1) {
        //        for (int i=0; i<tapSpaces.Length; i++) {
        //            tapSpaces[i].SetMyNumber(tapSpaces.Length-1-i);
        //        }
        //    }

        //    // GOOD tap!
        //    if (tapSpace.MyNumber == nextSpaceNumber) {
        //        OnCorrectTap(tapSpace);
        //    }
        //    // BAD tap!
        //    else {
        //        OnIncorrectTap(tapSpace);
        //    }
        //}

        //private void OnCorrectTap(TapSpace tapSpace) {
        //    tapSpace.OnCorrectTap();
        //    nextSpaceNumber ++;
        //    if (nextSpaceNumber >= tapSpaces.Length) {
        //        OnCompleteLevel();
        //    }
        //}
        //private void OnIncorrectTap(TapSpace tapSpace) {
        //    tapSpace.OnIncorrectTap();
        //    // End the game!
        //    SetGameOver();
        //}





    }
}
