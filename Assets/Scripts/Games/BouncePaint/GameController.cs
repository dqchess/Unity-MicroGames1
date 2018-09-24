using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BouncePaint {
    public enum LoseReasons { TapEarly, MissedTap, TappedDontTap }

    public class GameController : BaseLevelGameController {
		// Overrideables
		override public string MyGameName() { return GameNames.BouncePaint; }
		// Properties
		private LoseReasons loseReason;
        // References
        [SerializeField] private FUEController fueController=null;
        [SerializeField] private BouncePaintSfxController sfxController=null;
		private Level level; // MY game-specific Level class.
        public Player WinningPlayer; // the Player whose bounce won the level!

        // Getters (Public)
        public bool IsFUEPlayerFrozen { get { return fueController.IsPlayerFrozen; } }
        public float PlayerDiameter { get; set; }
        public float PlayerGravityScale { get; set; } // Currently used for multi-ball levels! Slow down gravity to make it more reasonable.
        public List<Block> Blocks { get { return level.Blocks; } }
        public List<Player> Players { get { return level.Players; } }
        // Getters (Private)
        private bool IsFUECantLose { get { return fueController.IsCantLose; } }
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
		//  Game Flow Events
		// ----------------------------------------------------------------
		override public void LoseLevel() {
			base.LoseLevel();
			// Tell people!
			fueController.OnSetGameOver(loseReason);
			sfxController.OnSetGameOver();
		}
		override protected void WinLevel() {
			base.WinLevel();
			StartCoroutine(Coroutine_SetTimeScaleFromWinLevel());
			StartCoroutine(Coroutine_StartNextLevel());
			// Tell people!
			level.OnWinLevel(WinningPlayer);
			sfxController.OnCompleteLevel();
		}

        private IEnumerator Coroutine_StartNextLevel() {
            yield return new WaitForSecondsRealtime(2.6f);
            SetCurrentLevel(LevelIndex+1, true);
        }

		override protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false) {
            StartCoroutine(Coroutine_SetCurrentLevel(_levelIndex, doAnimate));
        }
		override protected void InitializeLevel(GameObject _levelGO, int _levelIndex) {
			level = _levelGO.GetComponent<Level>();
			level.Initialize(this, canvas.transform, _levelIndex);
			base.OnInitializeLevel(level);
		}
        private IEnumerator Coroutine_SetCurrentLevel(int _levelIndex, bool doAnimate) {
			Level prevLevel = level;

            // Make the new level!
			InitializeLevel(Instantiate(resourcesHandler.bouncePaint_level), _levelIndex);

            // Set basics!
            WinningPlayer = null;

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

                level.IsAnimating = true;
                float height = 1200;
                Vector3 levelDefaultPos = level.transform.localPosition;
                level.transform.localPosition += new Vector3(0, height, 0);
                LeanTween.moveLocal(level.gameObject, levelDefaultPos, duration).setEaseInOutQuart();
                LeanTween.moveLocal(prevLevel.gameObject, new Vector3(0, -height, 0), duration).setEaseInOutQuart();
                yield return new WaitForSeconds(duration);

                level.IsAnimating = false;
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
            fueController.OnStartLevel(level);

            yield return null;
        }

        private IEnumerator Coroutine_SetTimeScaleFromWinLevel() {
            // Wait a tiny moment in fast motion.
            //Time.timeScale = 2f;
            yield return new WaitForSecondsRealtime(0.12f);
            // Slow down quick!
            float nextTimeStep = Time.unscaledTime + 1f;
            float targetTimeScale = 0.1f;
            while (Time.unscaledTime < nextTimeStep) {
                Time.timeScale += (targetTimeScale-Time.timeScale) / 7f;
                yield return null;
            }
            // Speed back up slowly.
            targetTimeScale = 1f;
            while (Time.timeScale < targetTimeScale) {
                Time.timeScale += 0.05f;
                yield return null;
            }
            Time.timeScale = 1f;
		}


		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		public void OnPlayerBounceOnBlock(Player player, bool didPaintBlock) {
			// Tell people!
			sfxController.OnPlayerBounceOnBlock();
			// We DID paint it!
			if (didPaintBlock) {
				// We're a champion?? Win!
				if (IsGameStatePlaying && IsEveryBlockSatisfied()) {
					WinningPlayer = player; // Assign the winning Player!
					WinLevel();
				}
				// Tell people!
				fueController.OnPlayerPaintBlock();
			}
			//foreach (Block block in blocks) {
			//    block.OnPlayerPaintedABlock();
			//}
		}
		public void OnPlayerSetBlockHeadingTo(Block blockHeadingTo) {
			// When the Player knows who it's going to, tell all Blocks to update their visuals!
			foreach (Block b in Blocks) {
				b.SetIntentionVisuals(b == blockHeadingTo);
			}
		}
		public void OnPlayerDie(LoseReasons reason) {
			if (IsGameStatePlaying) {
				loseReason = reason;
				LoseLevel();
			}
		}




        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        override protected void OnTapUp() { }
		override protected void OnTapDown() {
			// Paused? Ignore input.
			if (Time.timeScale == 0f) { return; }

            if (IsGameStatePlaying) {
                OnPressJumpButton();
            }

            // Tell people!
            fueController.OnTapDown();
        }


        private void OnPressJumpButton() {
            // Ignore jumps if...
			if (!IsGameStatePlaying) { return; }
            if (level!=null && level.IsAnimating) { return; }

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
            if (!didAnyPlayerBounce && !IsFUECantLose) { // also, don't explode if the FUE won't let us fail.
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
		override protected void Debug_WinLevel() {
            foreach (Block block in Blocks) {
                for (int i=Mathf.Max(1,block.NumHitsReq); i>0; --i) {
                    if (i==1 && !block.IsAvailable) { continue; } // Test. Comment this out if you want to win right away.
                    block.OnPlayerBounceOnMe(Player.GetRandomHappyColor(), Vector2.zero);
                }
            }
            OnPlayerBounceOnBlock(Players[0], true);
        }




    }
}


/*

    if (gameState == GameStates.Playing) {
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