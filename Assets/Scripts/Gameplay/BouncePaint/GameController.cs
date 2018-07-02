using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public enum GameStates { PreLevel, Playing, PostLevel, GameOver }

    public class GameController : BaseGameController {
        // Properties
        private GameStates gameState;
        private float timeWhenLevelEnded;
        private int currentLevelIndex;
        // Components
        [SerializeField] private Player player=null;
        private List<Block> blocks;
        // References
        [SerializeField] private GameUI ui=null;
		[SerializeField] private Image i_blocksAvailableRect=null;
        [SerializeField] private RectTransform rt_blocks=null;

        // Getters (Public)
        public bool IsLevelComplete { get { return gameState == GameStates.PostLevel; } }
        public List<Block> Blocks { get { return blocks; } }
        // Getters (Private)
        private bool IsEveryBlockPainted() {
            return NumBlocksPainted() >= blocks.Count;
        }
        private int NumBlocksPainted() {
            int total=0;
            foreach (Block block in blocks) {
                if (block.IsPainted) { total ++; }
            }
            return total;
        }



        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

            SetCurrentLevel(1);
        }


        private void AddLevelComponents(int numBlocks) {
            DestroyAllBlocks(); // Just in case

            Rect availableRect = new Rect();// i_spacesAvailableRect.rectTransform.rect;
            availableRect.size = i_blocksAvailableRect.rectTransform.rect.size;
            availableRect.position = new Vector2(0,200);
            Vector2 slotSize = new Vector2(availableRect.width/(float)numBlocks, availableRect.height);
            Vector2 blockSize = new Vector2(availableRect.height, availableRect.height);//spaceSlotSize.x-spaceGapX, spaceSlotSize.y);
            float gapX = slotSize.x-blockSize.x;//4; // how many pixels between each block.

            blocks = new List<Block>();
            for (int i=0; i<numBlocks; i++) {
                Block newBlock = Instantiate(resourcesHandler.bouncePaint_block).GetComponent<Block>();
                Vector2 pos = new Vector2(gapX*0.5f + slotSize.x*i, 0);
                pos += availableRect.position;
                pos += new Vector2(0, Random.Range(0, numBlocks*20f));
                newBlock.Initialize(this, rt_blocks, pos, blockSize);
                blocks.Add(newBlock);
            }
        }
        private void DestroyAllBlocks() {
            if (blocks!=null) {
                foreach (Block block in blocks) {
                    Destroy(block.gameObject);
                }
            }
            blocks = null;
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
            timeWhenLevelEnded = -1;
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Tell the UI!
            ui.UpdateLevelName(currentLevelIndex);

            // Initialize level components and player!
            int numBlocks = currentLevelIndex;
            AddLevelComponents(numBlocks);
            player.Reset(currentLevelIndex);
        }
        private void SetGameOver() {
            gameState = GameStates.GameOver;
            timeWhenLevelEnded = Time.time;
            //Camera.main.backgroundColor = new Color(0.6f,0.1f,0.1f);
        }

        private void OnCompleteLevel() {
            gameState = GameStates.PostLevel;
            timeWhenLevelEnded = Time.time;
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

        public void OnPlayerPaintBlock() {
            // We're a champion?? Win!
            if (gameState==GameStates.Playing && IsEveryBlockPainted()) {
                OnCompleteLevel();
            }
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
                // Make us wait a short moment so we visually register what's happened.
                if (Time.time>timeWhenLevelEnded+0.2f) {
                    RestartLevel();
                    return;
                }
            }
            else if (gameState == GameStates.PostLevel) {
                StartNextLevel();
                return;
            }
            else {
                player.OnPressJumpButton();
            }
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
            foreach (Block block in blocks) {
                block.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
            }
            OnPlayerPaintBlock();
        }



    }
}
