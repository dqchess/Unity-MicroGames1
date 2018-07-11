using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
    public enum GameStates { Playing, GameOver, PostLevel }
    public enum LoseReasons { TapEarly, MissedTap, TappedDontTap }

    public class GameController : BaseGameController {
        // Properties
        private GameStates gameState;
        private float timeWhenLevelEnded;
        // Components
        private Level level;
        // References
        [SerializeField] private Canvas canvas=null;
        [SerializeField] private GameUI ui=null;
        [SerializeField] private FUEController fueController;

        // Getters (Public)
        public bool IsFUEPlayerFrozen { get { return fueController.IsPlayerFrozen; } }
        public bool IsLevelComplete { get { return gameState == GameStates.PostLevel; } }
        public float PlayerDiameter { get; set; }
        public float PlayerGravityScale { get; set; } // Currently used for multi-ball levels! Slow down gravity to make it more reasonable.
        public List<Block> Blocks { get { return level.Blocks; } }
        public List<Player> Players { get { return level.Players; } }
        // Getters (Private)
        private int LevelIndex { get { return level.LevelIndex; } }
        private bool IsEveryBlockSatisfied() {
            return NumBlocksSatisfied() >= Blocks.Count;
        }
        private int NumBlocksSatisfied() {
            int total=0;
            foreach (Block block in Blocks) {
                if (block.IsSatisfied) { total ++; }
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
        //private void SetCurrentLevel(int _levelIndex) {
        //    level = Instantiate(resourcesHandler.bouncePaint_level).GetComponent<Level>();
        //    level.Initialize(this,canvas.transform, _levelIndex);

        //    SaveStorage.SetInt(SaveKeys.BouncePaint_LastLevelPlayed, LevelIndex);

        //    // Set basics!
        //    SetIsPaused(false);
        //    timeWhenLevelEnded = -1;
        //    gameState = GameStates.Playing;
        //    Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

        //    // Tell the UI!
        //    ui.OnStartLevel(LevelIndex);

        //    // Initialize level components, and reset Players!
        //    level.AddLevelComponents();
        //    // HACK! For now until I know how this is gonna work.
        //    if (PlayerGravityScale == 1f) { // if we didn't specify the gravity scale...
        //        PlayerGravityScale = Players.Count == 1 ? 1f : 0.6f;
        //    }
        //    foreach (Player p in Players) {
        //        p.Reset(LevelIndex);
        //    }
        //}
        private void SetGameOver(LoseReasons reason) {
            gameState = GameStates.GameOver;
            timeWhenLevelEnded = Time.time;
            ui.OnGameOver();
            fueController.OnSetGameOver(reason);
        }

        private void OnCompleteLevel() {
            gameState = GameStates.PostLevel;
            timeWhenLevelEnded = Time.time;
            StartCoroutine(Coroutine_StartNextLevel());
        }

        public void OnPlayerPaintBlock() {
            // We're a champion?? Win!
            if (gameState==GameStates.Playing && IsEveryBlockSatisfied()) {
                OnCompleteLevel();
            }
            // Tell people!
            fueController.OnPlayerPaintBlock();
            //foreach (Block block in blocks) {
            //    block.OnPlayerPaintedABlock();
            //}
        }
        public void OnPlayerDie(LoseReasons reason) {
            if (gameState == GameStates.Playing) {
                SetGameOver(reason);
            }
        }

		public void OnPlayerSetBlockHeadingTo(Block blockHeadingTo) {
			// When the Player knows who it's going to, tell all Blocks to update their visuals!
			foreach (Block b in Blocks) {
				b.SetIntentionVisuals(b == blockHeadingTo);
			}
        }

        private IEnumerator Coroutine_StartNextLevel() {
            yield return new WaitForSecondsRealtime(1.1f);
            SetCurrentLevel(LevelIndex+1, true);
        }

        private void SetCurrentLevel(int _levelIndex, bool doAnimate=false) {
            StartCoroutine(Coroutine_SetCurrentLevel(_levelIndex, doAnimate));
        }
        private IEnumerator Coroutine_SetCurrentLevel(int _levelIndex, bool doAnimate) {
            // Make the new level!
            Level prevLevel = level;
            level = Instantiate(resourcesHandler.bouncePaint_level).GetComponent<Level>();
            level.Initialize(this,canvas.transform, _levelIndex);

            SaveStorage.SetInt(SaveKeys.BouncePaint_LastLevelPlayed, LevelIndex);

            // Set basics!
            SetIsPaused(false);
            timeWhenLevelEnded = -1;
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Initialize level components, and reset Players!
            level.AddLevelComponents();
            // HACK! For now until I know how this is gonna work.
            if (PlayerGravityScale == 1f) { // if we didn't specify the gravity scale...
                PlayerGravityScale = Players.Count == 1 ? 1f : 0.6f;
            }
            foreach (Player p in Players) {
                p.Reset(LevelIndex);
            }

            // DO animate!
            if (doAnimate) {
                float duration = 1f;

                level.IsAnimatingIn = true;
                float height = 1200;
                Vector3 levelDefaultPos = level.transform.localPosition;
                level.transform.localPosition += new Vector3(0, height, 0);
                LeanTween.moveLocal(level.gameObject, levelDefaultPos, duration).setEaseInOutQuart();
                LeanTween.moveLocal(prevLevel.gameObject, new Vector3(0, -height, 0), duration).setEaseInOutQuart();
                yield return new WaitForSeconds(duration);

                level.IsAnimatingIn = false;
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

            // Tell things!
            ui.OnStartLevel(LevelIndex);
            fueController.OnStartLevel(level);

            yield return null;
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
        public void OnRetryButtonClick() {
            RestartLevel();
        }
		private void OnTapScreen() {
			// Paused? Ignore input.
			if (Time.timeScale == 0f) { return; }

            if (gameState == GameStates.GameOver) {
                //// Make us wait a short moment so we visually register what's happened.
                //if (Time.time>timeWhenLevelEnded+0.2f) {
                //    RestartLevel();
                //    return;
                //}
            }
			else if (gameState == GameStates.PostLevel) {
				//// Make us wait a short moment so we visually register what's happened.
				//if (Time.time>timeWhenLevelEnded+0.2f) {
	   //             StartNextLevel();
	   //             return;
				//}
            }
            else {
                OnPressJumpButton();
            }

            // Tell people!
            fueController.OnTapScreen();
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
            // Ignore jumps if...
            if (gameState != GameStates.Playing) { return; }
            if (level!=null && level.IsAnimatingIn) { return; }

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
            if (!didAnyPlayerBounce && !IsFUEPlayerFrozen) { // also, don't explode if we're frozen in the FUE.
                foreach (Player player in Players) {
                    // Visually inform any don't-tap Blocks for user's mistake feedback.
                    LoseReasons reason = LoseReasons.TapEarly; // I'll say otherwise next.
                    Block blockTouching = player.GetUnpaintedBlockTouching();
                    if (blockTouching != null && !blockTouching.DoTap) {
                        blockTouching.OnPlayerPressJumpOnMeInappropriately();
                        reason = LoseReasons.TappedDontTap;
                    }
                    // Explode the ball!
                    player.Explode(reason);
                }
            }
        }



        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_WinLevel() {
            foreach (Block block in Blocks) {
                for (int i=Mathf.Max(1,block.NumHitsReq); i>0; --i) {
                    if (i==1 && !block.IsAvailable) { continue; } // Test. Comment this out if you want to win right away.
                    block.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
                }
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