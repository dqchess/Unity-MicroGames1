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
        // Components
        [SerializeField] private Level level;
        // References
        [SerializeField] private GameUI ui=null;

        // Getters (Public)
        public bool IsLevelComplete { get { return gameState == GameStates.PostLevel; } }
        public float PlayerGravityScale { get; set; } // Currently used for multi-ball levels! Slow down gravity to make it more reasonable.
        public List<Block> Blocks { get { return level.Blocks; } }
        // Getters (Private)
        private int LevelIndex { get { return level.LevelIndex; } }
        private List<Player> Players { get { return level.Players; } }
        private bool IsEveryBlockPainted() {
            return NumBlocksPainted() >= Blocks.Count;
        }
        private int NumBlocksPainted() {
            int total=0;
            foreach (Block block in Blocks) {
                if (block.IsPainted) { total ++; }
            }
            return total;
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start () {
            base.Start();

			SetCurrentLevel(SaveStorage.GetInt(SaveKeys.BouncePaint_LastLevelPlayed, 1));
        }



        // ----------------------------------------------------------------
        //  Game Flow
        // ----------------------------------------------------------------
        private void RestartLevel() { SetCurrentLevel(LevelIndex); }
        public void StartPrevLevel() { SetCurrentLevel(Mathf.Max(1, LevelIndex-1)); }
        public void StartNextLevel() { SetCurrentLevel(LevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            level.LevelIndex = _levelIndex;
            SaveStorage.SetInt(SaveKeys.BouncePaint_LastLevelPlayed, LevelIndex);

            // Set basics!
            SetIsPaused(false);
            timeWhenLevelEnded = -1;
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Tell the UI!
            ui.UpdateLevelName(LevelIndex);

            // Initialize level components, and reset Players!
            level.AddLevelComponents();
            // HACK! For now until I know how this is gonna work.
            if (PlayerGravityScale == 1f) { // if we didn't specify the gravity scale...
                PlayerGravityScale = Players.Count == 1 ? 1f : 0.6f;
            }
            foreach (Player p in Players) {
                p.Reset(LevelIndex);
            }
        }
        private void SetGameOver() {
            gameState = GameStates.GameOver;
            timeWhenLevelEnded = Time.time;
        }

        private void OnCompleteLevel() {
            gameState = GameStates.PostLevel;
            timeWhenLevelEnded = Time.time;
        }

        public void OnPlayerPaintBlock() {
            // We're a champion?? Win!
            if (gameState==GameStates.Playing && IsEveryBlockPainted()) {
                OnCompleteLevel();
            }
            //foreach (Block block in blocks) {
            //    block.OnPlayerPaintedABlock();
            //}
        }
        public void OnPlayerDie() {
            if (gameState == GameStates.Playing) {
                SetGameOver();
            }
        }

		public void OnPlayerSetBlockHeadingTo(Block blockHeadingTo) {
			// When the Player knows who it's going to, tell all Blocks to update their visuals!
			foreach (Block b in Blocks) {
				b.SetIntentionVisuals(b == blockHeadingTo);
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
			OnTapScreen();
		}
		private void OnTapScreen() {
			// Paused? Ignore input.
			if (Time.timeScale == 0f) { return; }

            if (gameState == GameStates.GameOver) {
                // Make us wait a short moment so we visually register what's happened.
                if (Time.time>timeWhenLevelEnded+0.2f) {
                    RestartLevel();
                    return;
                }
            }
			else if (gameState == GameStates.PostLevel) {
				// Make us wait a short moment so we visually register what's happened.
				if (Time.time>timeWhenLevelEnded+0.2f) {
	                StartNextLevel();
	                return;
				}
            }
            else {
                OnPressJumpButton();
            }
        }
        override protected void RegisterButtonInput() {
            base.RegisterButtonInput();

			if (Input.GetKeyDown(KeyCode.Space)) { OnTapScreen(); }

			// DEBUG
            if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPrevLevel(); }
            if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextLevel(); }
            if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
        }


        private void OnPressJumpButton() {
            bool didAnyPlayerBounce = false; // I'll say otherwise next.
            foreach (Player player in Players) {
                Block blockTouching = player.GetUnpaintedBlockTouching();
                // This Player IS touching a standard, do-tap Block...!
                if (blockTouching != null && blockTouching.DoTap) {
                    player.BounceOnBlock(blockTouching);
                    didAnyPlayerBounce = true;
                }
            }

            // Did NOBODY bounce? Oh, jeez. Explode some balls.
            if (!didAnyPlayerBounce) {
                foreach (Player player in Players) {
                    // Visually inform any don't-tap Blocks for user's mistake feedback.
                    Block blockTouching = player.GetUnpaintedBlockTouching();
                    if (blockTouching != null && !blockTouching.DoTap) {
                        blockTouching.OnPlayerPressJumpOnMeInappropriately();
                    }
                    // Explode the ball!
                    player.Explode();
                }
            }
        }



        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_WinLevel() {
            foreach (Block block in Blocks) {
                block.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
            }
            OnPlayerPaintBlock();
        }
		/// Jumps *10* levels back.
        public void StartPrevLevel10() { SetCurrentLevel(Mathf.Max(1, LevelIndex-10)); }
		/// Jumps *10* levels forward.
        public void StartNextLevel10() { SetCurrentLevel(LevelIndex+10); }




    }
}


/*
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
*/