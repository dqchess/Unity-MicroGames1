using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterClear {
    public class GameController : BaseGameController {
        // Properties
        private int currentLevelIndex;
        // Components
        private List<WordTile> wordTiles;
        // References
        [SerializeField] private RectTransform rt_letterTiles=null;

        // Getters



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            SetCurrentLevel(0);
        }


        private void InitializeLevel(string levelSentence) {
            DestroyWordTiles(); // Just in case

            // Initialize 'em!
            string[] words = levelSentence.Split(' ');
            wordTiles = new List<WordTile>();
            foreach (string word in words) {
                WordTile newObj = Instantiate(resourcesHandler.letterClear_wordTile).GetComponent<WordTile>();
                newObj.Initialize(this, rt_letterTiles, word);
                wordTiles.Add(newObj);
            }

            UpdateWordsPositions();
        }
        private void UpdateWordsPositions() {
            // Position 'em!
            Rect availableRect = rt_letterTiles.rect;
            float tempX = availableRect.xMin;
            float tempY = availableRect.yMax;
            const int fontSize = 60;
            float spaceSize = fontSize*0.4f;
            float lineHeight = fontSize;
            foreach (WordTile tile in wordTiles) {
                // Set the font size!
                tile.SetFontSize(fontSize);
                // Determine if this word should spill over to the next line.
                float tileWidth = tile.GetWidth();
                if (tempX+tileWidth>availableRect.xMax && tileWidth<availableRect.width) { // If this word spills over AND it's not exceedingly big (in which case, just let it be)?
                    tempX = availableRect.xMin;
                    tempY -= lineHeight;
                }
                tile.Pos = new Vector2(tempX, tempY);
                tempX += tileWidth + spaceSize;
            }
        }
        private void DestroyWordTiles() {
            if (wordTiles!=null) {
                foreach (WordTile obj in wordTiles) {
                    Destroy(obj.gameObject);
                }
            }
            wordTiles = null;
        }


        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void RestartLevel() { SetCurrentLevel(currentLevelIndex); }
        private void StartPreviousLevel() { SetCurrentLevel(Mathf.Max(1, currentLevelIndex-1)); }
        private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            currentLevelIndex = _levelIndex;

            SetIsPaused(false);

            // Set basics!
            //Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Tell the UI!
            //ui.UpdateLevelName(currentLevelIndex);

            // Initialize everything!
            //TODO: different sentence per level.
            InitializeLevel("start with a sentence. match letters against each other to clear them.");
        }
        private void SetGameOver() {
            //gameState = GameStates.GameOver;
            //timeWhenLevelEnded = Time.time;
            //Camera.main.backgroundColor = new Color(0.6f,0.1f,0.1f);
        }

        private void OnCompleteLevel() {
            //gameState = GameStates.PostLevel;
            //timeWhenLevelEnded = Time.time;
            //Camera.main.backgroundColor = new Color(0.1f,0.8f,0.1f);
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
            //if (gameState == GameStates.GameOver) {
            //    // Make us wait a short moment so we visually register what's happened.
            //    if (Time.time>timeWhenLevelEnded+0.2f) {
            //        RestartLevel();
            //        return;
            //    }
            //}
            //else if (gameState == GameStates.PostLevel) {
            //    StartNextLevel();
            //    return;
            //}
            //else {
            //    
            //}
        }
        override protected void RegisterButtonInput() {
            base.RegisterButtonInput();
            if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPreviousLevel(); }
            if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextLevel(); }
            if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
        }



        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_WinLevel() {
            //foreach (PaintSpace space in paintSpaces) {
            //    space.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
            //}
        }



    }
}
