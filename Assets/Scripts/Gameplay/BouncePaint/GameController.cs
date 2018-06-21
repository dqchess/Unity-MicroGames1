using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public enum GameStates { PreLevel, Playing, PostLevel, GameOver }

    public class GameController : BaseGameController {
        // Properties
        private GameStates gameState;
        private int currentLevelIndex;
        // Components
        [SerializeField] private Player player;
        private PaintSpace[] paintSpaces;
        // References
        [SerializeField] private GameUI ui=null;
		[SerializeField] private Image i_spacesAvailableRect=null;
        [SerializeField] private RectTransform rt_paintSpaces=null;

        // Getters
        public PaintSpace[] PaintSpaces { get { return paintSpaces; } }



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            SetCurrentLevel(0);
        }


        private void InitializePaintSpaces(int numSpaces) {
            DestroyPaintSpaces(); // Just in case

            Rect availableRect = new Rect();// i_spacesAvailableRect.rectTransform.rect;
            availableRect.size = i_spacesAvailableRect.rectTransform.rect.size;
            availableRect.position = new Vector2(0,200);
            Vector2 spaceSlotSize = new Vector2(availableRect.width/(float)numSpaces, availableRect.height);
            Vector2 spaceSize = new Vector2(availableRect.height, availableRect.height);//spaceSlotSize.x-spaceGapX, spaceSlotSize.y);
            float spaceGapX = spaceSlotSize.x-spaceSize.x;//4; // how many pixels between each space.

            paintSpaces = new PaintSpace[numSpaces];
            for (int i=0; i<numSpaces; i++) {
                PaintSpace newSpace = Instantiate(resourcesHandler.bouncePaint_paintSpace).GetComponent<PaintSpace>();
                Vector2 pos = new Vector2(spaceGapX*0.5f + spaceSlotSize.x*i, 0);
                pos += availableRect.position;
                newSpace.Initialize(rt_paintSpaces, pos, spaceSize);
                paintSpaces[i] = newSpace;
            }
        }
        private void DestroyPaintSpaces() {
            if (paintSpaces!=null) {
                foreach (PaintSpace space in paintSpaces) {
                    Destroy(space.gameObject);
                }
            }
            paintSpaces = null;
        }


        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void RestartLevel() { SetCurrentLevel(currentLevelIndex); }
        private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            currentLevelIndex = _levelIndex;

            SetIsPaused(false);

            // Set basics!
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Tell the UI!
            ui.UpdateLevelName(currentLevelIndex);

            // Initialize spaces and player!
            int numSpaces = 1 + currentLevelIndex;
            InitializePaintSpaces(numSpaces);
            player.Reset(currentLevelIndex);
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

        public void OnPlayerPaintLastSpace() {
            OnCompleteLevel();
        }
        public void OnPlayerDie() {
            if (gameState == GameStates.Playing) {
                SetGameOver();
            }
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
                RestartLevel();//ReloadScene();
                return;
            }
            else if (gameState == GameStates.PostLevel) {
                StartNextLevel();
                return;
            }
            else {
                player.OnPressJumpButton();
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
