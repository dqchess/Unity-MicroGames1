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

			SetCurrentLevel(SaveStorage.GetInt(SaveKeys.BouncePaint_LastLevelPlayed, 1));
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
        public void StartPrevLevel() { SetCurrentLevel(Mathf.Max(1, currentLevelIndex-1)); }
        public void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
        private void SetCurrentLevel(int _levelIndex) {
            currentLevelIndex = _levelIndex;
			SaveStorage.SetInt(SaveKeys.BouncePaint_LastLevelPlayed, currentLevelIndex);

            SetIsPaused(false);

            // Set basics!
            timeWhenLevelEnded = -1;
            gameState = GameStates.Playing;
            Camera.main.backgroundColor = new Color(0.97f,0.97f,0.97f);

            // Tell the UI!
            ui.UpdateLevelName(currentLevelIndex);

            // Initialize level components and player!
			AddLevelComponents(currentLevelIndex);
            player.Reset(currentLevelIndex);
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
        }
        public void OnPlayerDie() {
            if (gameState == GameStates.Playing) {
                SetGameOver();
            }
        }

		public void OnPlayerSetBlockHeadingTo(Block blockHeadingTo) {
			// When the Player knows who it's going to, tell all Blocks to update their visuals!
			foreach (Block b in blocks) {
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
			if (Time.timeScale == 0) { return; }

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
                player.OnPressJumpButton();
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




        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        private void Debug_WinLevel() {
            foreach (Block block in blocks) {
                block.OnPlayerBounceOnMe(Player.GetRandomHappyColor());
            }
            OnPlayerPaintBlock();
        }
		/// Jumps *10* levels back.
        public void StartPrevLevel10() { SetCurrentLevel(Mathf.Max(1, currentLevelIndex-10)); }
		/// Jumps *10* levels forward.
        public void StartNextLevel10() { SetCurrentLevel(currentLevelIndex+10); }



		// HARDCODED Level-adding. Hardcoded for now.
		private void AddLevelComponents(int levelIndex) {
			DestroyAllBlocks(); // Just in case
			blocks = new List<Block>();

			Vector2 blockSize = new Vector2(50,50);

			// NOTE: All coordinates are based off of a 600x800 available playing space! :)

			float b = -240; // bottom.
			int i=1; // TEMP! Until we make levels into XML or Json.
			if (false) {}


			// Simple, together.
			else if (levelIndex == i++) {
				AddBlock(blockSize, 0,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -30,b);
				AddBlock(blockSize,  30,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -60,b);
				AddBlock(blockSize,   0,b);
				AddBlock(blockSize,  60,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -90,b);
				AddBlock(blockSize, -30,b);
				AddBlock(blockSize,  30,b);
				AddBlock(blockSize,  90,b);
			}

			// Larger X gaps.
			else if (levelIndex == i++) {
				AddBlock(blockSize, -100,b);
				AddBlock(blockSize,  100,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -200,b);
				AddBlock(blockSize,  100,b);
				AddBlock(blockSize,  200,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -200,b);
				AddBlock(blockSize, -140,b);
				AddBlock(blockSize,  -80,b);
				AddBlock(blockSize,  200,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -200,b);
				AddBlock(blockSize, -140,b);
				AddBlock(blockSize,    0,b+50);
				AddBlock(blockSize,  200,b);
				AddBlock(blockSize,  140,b);
			}

			// Offset Y positions
//			else if (levelIndex == i++) {
//				AddBlock(blockSize, -80,b);
//				AddBlock(blockSize,   0,b+100);
//				AddBlock(blockSize,  80,b);
//			}
//			else if (levelIndex == i++) {
//				AddBlock(blockSize, -80,b);
//				AddBlock(blockSize,  80,b+200);
//				AddBlock(blockSize,  80,b);
//			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -180,b+160);
				AddBlock(blockSize, -100,b);
				AddBlock(blockSize,  100,b);
				AddBlock(blockSize,  180,b+160);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -180,b);
				AddBlock(blockSize,  -60,b+100);
				AddBlock(blockSize,    0,b+300);
				AddBlock(blockSize,   60,b+100);
				AddBlock(blockSize,  180,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -220,b+100);
				AddBlock(blockSize,  -90,b);
				AddBlock(blockSize,  -30,b+160);
				AddBlock(blockSize,   30,b+20);
				AddBlock(blockSize,  130,b);
				AddBlock(blockSize,  180,b+80);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -240,b+300);
				AddBlock(blockSize, -120,b+200);
				AddBlock(blockSize,  -60,b);
				AddBlock(blockSize,    0,b+150);
				AddBlock(blockSize,   60,b+200);
				AddBlock(blockSize,  180,b);
				AddBlock(blockSize,  240,b+100);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -210,b);
				AddBlock(blockSize, -150,b+40);
				AddBlock(blockSize,  -90,b+80);
				AddBlock(blockSize,  -30,b+120);
				AddBlock(blockSize,   30,b+160);
				AddBlock(blockSize,   90,b+200);
				AddBlock(blockSize,  150,b+240);
				AddBlock(blockSize,  210,b+280);
            }

            // Vertical Stacks
            else if (levelIndex == i++) {
                AddBlock(blockSize, 0,b);
                AddBlock(blockSize, 0,b+60);
                AddBlock(blockSize, 0,b+120);
                AddBlock(blockSize, 0,b+180);
            }
            else if (levelIndex == i++) {
                AddBlock(blockSize, 0,b);
                AddBlock(blockSize, 0,b+80);
                AddBlock(blockSize, 0,b+160);
                AddBlock(blockSize, 0,b+240);
                AddBlock(blockSize, 0,b+320);
            }
            else if (levelIndex == i++) {
                AddBlock(blockSize, -120,b);
                AddBlock(blockSize,    0,b);
                AddBlock(blockSize,  120,b);
                AddBlock(blockSize, -120,b+180);
                AddBlock(blockSize,    0,b+180);
                AddBlock(blockSize,  120,b+180);
            }
//          else if (levelIndex == i++) {
//              AddBlock(blockSize, -80,b);
//              AddBlock(blockSize,   0,b);
//              AddBlock(blockSize,   0,b+80);
//              AddBlock(blockSize,   0,b+160);
//              AddBlock(blockSize,   0,b+240);
//              AddBlock(blockSize,  80,b);
//          }
            else if (levelIndex == i++) { // 8 plus
                AddBlock(blockSize,    0,b);
                AddBlock(blockSize,    0,b+80);
                AddBlock(blockSize,    0,b+240);
                AddBlock(blockSize,    0,b+320);
                AddBlock(blockSize, -160,b+160);
                AddBlock(blockSize,  -80,b+160);
                AddBlock(blockSize,   80,b+160);
                AddBlock(blockSize,  160,b+160);
            }



			// Traveling Blocks
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-100,b), new Vector2(100,b), 1f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-60,b), new Vector2(-160,b), 1f);
				AddBlock(blockSize, new Vector2(60,b), new Vector2(160,b), 1f);
				AddBlock(blockSize, 0,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-100,b), new Vector2(-160,b), 1f);
				AddBlock(blockSize, new Vector2(140,b), new Vector2(200,b), 1f);
				AddBlock(blockSize, new Vector2(-100,b+100), new Vector2(100,b+100), 1f);
				AddBlock(blockSize, -200,b+140);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-120,b), new Vector2(-240,b), 1f);
				AddBlock(blockSize, new Vector2(-60,b), new Vector2(-120,b), 1f);
				AddBlock(blockSize, 0,b);
				AddBlock(blockSize, new Vector2( 120,b), new Vector2(240,b), 1f);
				AddBlock(blockSize, new Vector2( 60,b), new Vector2(120,b), 1f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-120,b), new Vector2(-240,b), 1f);
				AddBlock(blockSize, new Vector2(-60,b), new Vector2(-120,b), 1f);
				AddBlock(blockSize, 0,b);
				AddBlock(blockSize, new Vector2(-200,b+250), new Vector2(200,b+250), 1f);
				AddBlock(blockSize, new Vector2( 240,b), new Vector2(120,b), 1f);
				AddBlock(blockSize, new Vector2( 120,b), new Vector2(60,b), 1f);
            }
            // Faster Traveling Blocks
            else if (levelIndex == i++) {
                float w = 120;
                AddBlock(blockSize, new Vector2(-240,b), new Vector2(-240+w,b), 1.4f);
                AddBlock(blockSize, new Vector2(-180,b), new Vector2(-180+w,b), 1.4f);
                AddBlock(blockSize, new Vector2(-120,b), new Vector2(-120+w,b), 1.4f);
                AddBlock(blockSize, new Vector2( -60,b), new Vector2( -60+w,b), 1.4f);
                AddBlock(blockSize, new Vector2(   0,b), new Vector2(   0+w,b), 1.4f);
                AddBlock(blockSize, new Vector2(  60,b), new Vector2(  60+w,b), 1.4f);
                AddBlock(blockSize, new Vector2( 120,b), new Vector2( 120+w,b), 1.4f);
            }
            else if (levelIndex == i++) {
                float w = 160;
                AddBlock(blockSize, -260,b+240);
                AddBlock(blockSize, new Vector2(-200,b+160), new Vector2(-200+w,b+160), 2f);
                AddBlock(blockSize, new Vector2(-140,b+ 80), new Vector2(-140+w,b+ 80), 2f);
                AddBlock(blockSize, new Vector2( -80,b    ), new Vector2( -80+w,b    ), 2f);
                AddBlock(blockSize, new Vector2( -20,b+ 80), new Vector2( -20+w,b+ 80), 2f);
                AddBlock(blockSize, new Vector2(  40,b+160), new Vector2(  40+w,b+160), 2f);
                AddBlock(blockSize,  260,b+240);
            }
            else if (levelIndex == i++) {
                float w = 120;
                AddBlock(blockSize, new Vector2(-240,b), new Vector2(-240+w,b), 2f, 0f);
                AddBlock(blockSize, new Vector2(-180,b), new Vector2(-180+w,b), 2f, 0.2f);
                AddBlock(blockSize, new Vector2(-120,b), new Vector2(-120+w,b), 2f, 0.4f);
                AddBlock(blockSize, new Vector2( -60,b), new Vector2( -60+w,b), 2f, 0.6f);
                AddBlock(blockSize, new Vector2(   0,b), new Vector2(   0+w,b), 2f, 0.8f);
                AddBlock(blockSize, new Vector2(  60,b), new Vector2(  60+w,b), 2f, 1.0f);
                AddBlock(blockSize, new Vector2( 120,b), new Vector2( 120+w,b), 2f, 1.2f);
            }
            //else if (levelIndex == i++) {
            //    float w = 120;
            //    AddBlock(blockSize, new Vector2(-240,b), new Vector2(-240+w,b), 2f, 0f);
            //    AddBlock(blockSize, new Vector2(-180,b), new Vector2(-180+w,b), 2f, 0.3f);
            //    AddBlock(blockSize, new Vector2(-120,b), new Vector2(-120+w,b), 2f, 0.6f);
            //    AddBlock(blockSize, new Vector2( -60,b), new Vector2( -60+w,b), 2f, 0.9f);
            //    AddBlock(blockSize, new Vector2(   0,b), new Vector2(   0+w,b), 2f, 1.2f);
            //    AddBlock(blockSize, new Vector2(  60,b), new Vector2(  60+w,b), 2f, 1.5f);
            //    AddBlock(blockSize, new Vector2( 120,b), new Vector2( 120+w,b), 2f, 1.8f);
            //}

			// Varying-Speed Traveling Blocks
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-150,b), new Vector2(-240,b), 1f);
				AddBlock(blockSize, new Vector2(-100,b+50), new Vector2(100,b+50), 2f);
				AddBlock(blockSize, new Vector2( 150,b), new Vector2(240,b), 1f);
			}
			else if (levelIndex == i++) {
                AddBlock(blockSize, new Vector2(-240,b+100), new Vector2(-150,b+100), 2.5f);
                AddBlock(blockSize, new Vector2(-120,b), new Vector2(40,b), 2.5f);
                AddBlock(blockSize, new Vector2( -40,b+200), new Vector2(120,b+200), 2.5f);
				AddBlock(blockSize, new Vector2( 150,b+100), new Vector2(240,b+100), 2.5f);
            }
            else if (levelIndex == i++) {
                AddBlock(blockSize, new Vector2( 160,b+200), new Vector2(-160,b+200), 3f);
                AddBlock(blockSize, new Vector2(-150,b), new Vector2(-240,b), 4f);
                AddBlock(blockSize, new Vector2(-160,b+100), new Vector2( 160,b+100), 3f);
                AddBlock(blockSize, new Vector2( 150,b), new Vector2(240,b), 4f);
            }
            //else if (levelIndex == i++) {
            //    AddBlock(blockSize, new Vector2(-150,b+100), new Vector2(-240,b+100), 2f);
            //    AddBlock(blockSize, new Vector2(-160,b), new Vector2(160,b), 2f);
            //    AddBlock(blockSize, new Vector2(-160,b+200), new Vector2(160,b+200), 2f, 0.5f);
            //    AddBlock(blockSize, new Vector2( 240,b+100), new Vector2(150,b+100), 2f);
            //}
            else if (levelIndex == i++) {
                AddBlock(blockSize, new Vector2(-240,b    ), new Vector2( -80,b    ), 5f);
                AddBlock(blockSize, new Vector2( 240,b    ), new Vector2(  80,b    ), 5f);
                AddBlock(blockSize, new Vector2( -80,b+160), new Vector2(-240,b+160), 5f);
                AddBlock(blockSize, new Vector2(  80,b+160), new Vector2( 240,b+160), 5f);
                AddBlock(blockSize, new Vector2(-240,b+320), new Vector2( -80,b+320), 5f);
                AddBlock(blockSize, new Vector2( 240,b+320), new Vector2(  80,b+320), 5f);
            }

            else if (levelIndex == i++) {
                AddBlock(blockSize, new Vector2(-200,b    ), new Vector2( 200,b    ), 3f);
                AddBlock(blockSize, new Vector2(-200,b+100), new Vector2( 200,b+100), 3f, 3.142f);
                AddBlock(blockSize, new Vector2(-200,b+200), new Vector2( 200,b+200), 3f);
                AddBlock(blockSize, new Vector2(-200,b+300), new Vector2( 200,b+300), 3f, 3.142f);
            }



            // Weird-Shapes Interlude
//			else if (levelIndex == i++) { // 3x3 larger grid
//				AddBlock(blockSize, -120,b);
//				AddBlock(blockSize,    0,b);
//				AddBlock(blockSize,  120,b);
//				AddBlock(blockSize, -120,b+120);
//				AddBlock(blockSize,    0,b+120);
//				AddBlock(blockSize,  120,b+120);
//				AddBlock(blockSize, -120,b+240);
//				AddBlock(blockSize,    0,b+240);
//				AddBlock(blockSize,  120,b+240);
//			}
            else if (levelIndex == i++) { // 3x3 tight grid
				AddBlock(blockSize, -60,b);
				AddBlock(blockSize,   0,b);
				AddBlock(blockSize,  60,b);
				AddBlock(blockSize, -60,b+60);
				AddBlock(blockSize,   0,b+60);
				AddBlock(blockSize,  60,b+60);
				AddBlock(blockSize, -60,b+120);
				AddBlock(blockSize,   0,b+120);
				AddBlock(blockSize,  60,b+120);
			}



			// Weird-Shapes Traveling Blocks
			else if (levelIndex == i++) {
				AddBlock(blockSize, -220,b);
				AddBlock(blockSize, new Vector2( -220,b+60), new Vector2(-140,b+60), 0.7f);
				AddBlock(blockSize, new Vector2( -220,b+120), new Vector2(-60,b+120), 0.7f);
				AddBlock(blockSize, new Vector2( -220,b+180), new Vector2( 20,b+180), 0.7f);
				AddBlock(blockSize, new Vector2( -220,b+240), new Vector2( 100,b+240), 0.7f);
				AddBlock(blockSize, new Vector2( -220,b+300), new Vector2( 180,b+300), 0.7f);
				AddBlock(blockSize, new Vector2( -220,b+360), new Vector2( 260,b+360), 0.7f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2( -100,b    ), new Vector2(100,b), 1f, 0f);
				AddBlock(blockSize, new Vector2( -100,b+60 ), new Vector2(100,b+60), 1f, 0.5f);
				AddBlock(blockSize, new Vector2( -100,b+120), new Vector2(100,b+120), 1f, 1f);
				AddBlock(blockSize, new Vector2( -100,b+180), new Vector2(100,b+180), 1f, 1.5f);
				AddBlock(blockSize, new Vector2( -100,b+240), new Vector2(100,b+240), 1f, 2f);
				AddBlock(blockSize, new Vector2( -100,b+300), new Vector2(100,b+300), 1f, 2.5f);
				AddBlock(blockSize, new Vector2( -100,b+360), new Vector2(100,b+360), 1f, 3f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2( -160,b    ), new Vector2( 160,b), 1f, 0f);
				AddBlock(blockSize, new Vector2( -160,b+60 ), new Vector2( 160,b+60), 1f, -0.8f);
				AddBlock(blockSize, new Vector2( -160,b+120), new Vector2( 160,b+120), 1f, -1.6f);
				AddBlock(blockSize, new Vector2( -160,b+180), new Vector2( 160,b+180), 1f, -2.4f);
				AddBlock(blockSize, new Vector2( -160,b+240), new Vector2( 160,b+240), 1f, -3.2f);
				AddBlock(blockSize, new Vector2( -160,b+300), new Vector2( 160,b+300), 1f, -4f);
				AddBlock(blockSize, new Vector2( -160,b+360), new Vector2( 160,b+360), 1f, -4.8f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize,  220,b);
				AddBlock(blockSize, new Vector2(220,b+50), new Vector2( 140,b+50), 1f, -0.4f);
				AddBlock(blockSize, new Vector2(220,b+100), new Vector2( 60,b+100), 1f, -0.8f);
				AddBlock(blockSize, new Vector2(220,b+150), new Vector2(-20,b+150), 1f, -1.2f);
				AddBlock(blockSize, new Vector2(220,b+200), new Vector2(-100,b+200), 1f, -1.6f);
				AddBlock(blockSize, new Vector2(220,b+250), new Vector2(-180,b+250), 1f, -2f);
				AddBlock(blockSize, new Vector2(220,b+300), new Vector2(-260,b+300), 1f, -2.4f);
				AddBlock(blockSize, new Vector2(220,b+350), new Vector2(-260,b+350), 1f, -2.8f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -160,b);
				AddBlock(blockSize, new Vector2(-160,b+60), new Vector2(160,b+60), 0.4f, Mathf.PI);
				AddBlock(blockSize, new Vector2(-160,b+120), new Vector2(160,b+120), 0.8f);
				AddBlock(blockSize, new Vector2(-160,b+180), new Vector2(160,b+180), 1.2f, Mathf.PI);
				AddBlock(blockSize, new Vector2(-160,b+240), new Vector2(160,b+240), 1.6f);
				AddBlock(blockSize, new Vector2(-160,b+300), new Vector2(160,b+300), 2f, Mathf.PI);
				AddBlock(blockSize, new Vector2(-160,b+360), new Vector2(160,b+360), 2.4f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-160,b    ), new Vector2( 160,b   ), 1f, 0f);
				AddBlock(blockSize, new Vector2( 160,b+50 ), new Vector2(-160,b+50), 1f, 0.2f);
				AddBlock(blockSize, new Vector2(-160,b+100), new Vector2( 160,b+100), 1f, 0.4f);
				AddBlock(blockSize, new Vector2( 160,b+150), new Vector2(-160,b+150), 1f, 0.6f);
				AddBlock(blockSize, new Vector2(-160,b+200), new Vector2( 160,b+200), 1f, 0.8f);
				AddBlock(blockSize, new Vector2( 160,b+250), new Vector2(-160,b+250), 1f, 1f);
				AddBlock(blockSize, new Vector2(-160,b+300), new Vector2( 160,b+300), 1f, 1.2f);
				AddBlock(blockSize, new Vector2( 160,b+350), new Vector2(-160,b+350), 1f, 1.4f);
			}
//			else if (levelIndex == i++) {
//				AddBlock(blockSize, new Vector2(-160,b    ), new Vector2( 160,b   ), 1f, 0f);
//				AddBlock(blockSize, new Vector2( 160,b+55 ), new Vector2(-160,b+55), 1f, 0.2f);
//				AddBlock(blockSize, new Vector2(-160,b+110), new Vector2( 160,b+110), 1f, 0.4f);
//				AddBlock(blockSize, new Vector2( 160,b+165), new Vector2(-160,b+165), 1f, 0.6f);
//				AddBlock(blockSize, new Vector2(-160,b+220), new Vector2( 160,b+220), 1f, 0.8f);
//				AddBlock(blockSize, new Vector2( 160,b+275), new Vector2(-160,b+275), 1f, 1f);
//				AddBlock(blockSize, new Vector2(-160,b+330), new Vector2( 160,b+330), 1f, 1.2f);
//			}
//			else if (levelIndex == i++) {
//				AddBlock(blockSize, -220,b);
//				AddBlock(blockSize, new Vector2( -220,b+60), new Vector2(-140,b+60), 0.4f);
//				AddBlock(blockSize, new Vector2( -220,b+120), new Vector2(-60,b+120), 0.8f);
//				AddBlock(blockSize, new Vector2( -220,b+180), new Vector2( 20,b+180), 1.2f);
//				AddBlock(blockSize, new Vector2( -220,b+240), new Vector2( 100,b+240), 1.6f);
//				AddBlock(blockSize, new Vector2( -220,b+300), new Vector2( 180,b+300), 2f);
//				AddBlock(blockSize, new Vector2( -220,b+360), new Vector2( 260,b+360), 2.4f);
//			}



			// Don't-Tap Blocks
			else if (levelIndex == i++) {
				AddBlock(blockSize,    0,b, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -120,b);
				AddBlock(blockSize,    0,b);
				AddBlock(blockSize,  120,b, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -120,b);
				AddBlock(blockSize,  -60,b, false);
				AddBlock(blockSize,    0,b);
				AddBlock(blockSize,   60,b);
				AddBlock(blockSize,  120,b, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -150,b, false);
				AddBlock(blockSize,  -90,b, false);
				AddBlock(blockSize,  -30,b);
				AddBlock(blockSize,   30,b);
				AddBlock(blockSize,   90,b, false);
				AddBlock(blockSize,  150,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -220,b);
				AddBlock(blockSize,  -90,b);
				AddBlock(blockSize,  -30,b);
				AddBlock(blockSize,   30,b, false);
				AddBlock(blockSize,  160,b);
				AddBlock(blockSize,  220,b, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -210,b+10, false);
				AddBlock(blockSize, -150,b+80);
				AddBlock(blockSize,  -90,b+15, false);
				AddBlock(blockSize,  -30,b+40);
				AddBlock(blockSize,   30,b+20);
				AddBlock(blockSize,   90,b, false);
				AddBlock(blockSize,  150,b+50);
				AddBlock(blockSize,  210,b+45);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -210,b+180);
				AddBlock(blockSize, -150,b+120);
				AddBlock(blockSize,  -90,b+100, false);
				AddBlock(blockSize,  -30,b);
				AddBlock(blockSize,   30,b+300);
				AddBlock(blockSize,   90,b+40, false);
				AddBlock(blockSize,  150,b+160);
				AddBlock(blockSize,  210,b+80);
			}
			else if (levelIndex == i++) {
//				AddBlock(blockSize, -240,b+160);
				AddBlock(blockSize, -180,b+140);
				AddBlock(blockSize, -120,b+80);
				AddBlock(blockSize,  -60,b+40);
				AddBlock(blockSize,    0,b+360, false);
				AddBlock(blockSize,   60,b+40);
				AddBlock(blockSize,  120,b+80);
				AddBlock(blockSize,  180,b+140);
//				AddBlock(blockSize,  240,b+160);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -200,b+120, false);
				AddBlock(blockSize, -200,b+180, false);
				AddBlock(blockSize, -140,b+120, false);
				AddBlock(blockSize, -140,b+180, false);
				AddBlock(blockSize,   0,b);
				AddBlock(blockSize,  60,b+60);
				AddBlock(blockSize,  120,b+120);
				AddBlock(blockSize,  180,b+180);
				AddBlock(blockSize,  240,b+240);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -120,b);
				AddBlock(blockSize,  120,b);
				AddBlock(blockSize, -120,b+280, false);
				AddBlock(blockSize,  120,b+280, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -120,b, false);
				AddBlock(blockSize,  -60,b);
				AddBlock(blockSize,   60,b, false);
				AddBlock(blockSize,  120,b);
				AddBlock(blockSize, -120,b+250);
				AddBlock(blockSize,  -60,b+250, false);
				AddBlock(blockSize,   60,b+250);
				AddBlock(blockSize,  120,b+250, false);
			}
//			else if (levelIndex == i++) {
//				AddBlock(blockSize, -100,b, false);
//				AddBlock(blockSize, -100,b+60, false);
//				AddBlock(blockSize, -100,b+120, false);
//				AddBlock(blockSize, -100,b+180, false);
//				AddBlock(blockSize,  100,b);
//				AddBlock(blockSize,  100,b+60);
//				AddBlock(blockSize,  100,b+120);
//				AddBlock(blockSize,  100,b+180);
//			}
			else if (levelIndex == i++) {
				AddBlock(blockSize,  160,b+60, false);
				AddBlock(blockSize,  160,b+120, false);
				AddBlock(blockSize,  160,b+180, false);
				AddBlock(blockSize,  160,b+240, false);
				AddBlock(blockSize,  160,b+300, false);
				AddBlock(blockSize,  100,b);
				AddBlock(blockSize,   40,b);
				AddBlock(blockSize,  -20,b);
				AddBlock(blockSize,  -80,b);
				AddBlock(blockSize, -140,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -180,b);
				AddBlock(blockSize, -120,b);
				AddBlock(blockSize,  -60,b);
				AddBlock(blockSize,    0,b);
				AddBlock(blockSize,   60,b);
				AddBlock(blockSize,  120,b);
				AddBlock(blockSize,  180,b);
				AddBlock(blockSize, -180,b+200, false);
				AddBlock(blockSize, -120,b+200, false);
				AddBlock(blockSize,  -60,b+200, false);
				AddBlock(blockSize,    0,b+200, false);
				AddBlock(blockSize,   60,b+200, false);
				AddBlock(blockSize,  120,b+200, false);
				AddBlock(blockSize,  180,b+200, false);
			}





			// Traveling, No-Tap Blocks
			//TODO: These, son
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-160,b    ), new Vector2( 160,b   ), 1f, 0f, 1, false);
				AddBlock(blockSize, new Vector2( 160,b+50 ), new Vector2(-160,b+50), 1f, 0.2f, 1, false);
				AddBlock(blockSize, new Vector2(-160,b+100), new Vector2( 160,b+100), 1f, 0.4f);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(-160,b    ), new Vector2( 160,b   ), 1f, 0f, 1, false);
				AddBlock(blockSize, new Vector2( 160,b+50 ), new Vector2(-160,b+50), 1f, 0.2f, 1, false);
				AddBlock(blockSize, new Vector2(-160,b+100), new Vector2( 160,b+100), 1f, 0.4f);
				AddBlock(blockSize, new Vector2( 160,b+150), new Vector2(-160,b+150), 1f, 0.6f);
				AddBlock(blockSize, new Vector2(-160,b+200), new Vector2( 160,b+200), 1f, 0.8f);
				AddBlock(blockSize, new Vector2( 160,b+250), new Vector2(-160,b+250), 1f, 1f);
				AddBlock(blockSize, new Vector2(-160,b+300), new Vector2( 160,b+300), 1f, 1.2f);
				AddBlock(blockSize, new Vector2( 160,b+350), new Vector2(-160,b+350), 1f, 1.4f);
			}





			// Multi-Hit Blocks
			else if (levelIndex == i++) {
				AddBlock(blockSize,    0,b, 4);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -80,b+120, 3);
				AddBlock(blockSize,  80,b, 3);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -140,b, 2);
				AddBlock(blockSize,    0,b+100, 2);
				AddBlock(blockSize,  140,b+200, 2);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -140,b, 2);
				AddBlock(blockSize,    0,b+100, 2);
				AddBlock(blockSize,  140,b+200, 2);
			}







			// Even grids for level-making reference
			else if (levelIndex == i++) {
				AddBlock(blockSize, -120,b);
				AddBlock(blockSize,  -60,b);
				AddBlock(blockSize,    0,b);
				AddBlock(blockSize,   60,b);
				AddBlock(blockSize,  120,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -150,b);
				AddBlock(blockSize,  -90,b);
				AddBlock(blockSize,  -30,b);
				AddBlock(blockSize,   30,b);
				AddBlock(blockSize,   90,b);
				AddBlock(blockSize,  150,b);
			}

			// TEST LEVELS
			else if (levelIndex == i++) {
				AddBlock(blockSize, new Vector2(0,b), new Vector2(100,b), 1f);
				AddBlock(blockSize, new Vector2(100,b), new Vector2(0,b), 1f);
				AddBlock(blockSize, new Vector2(-100,b), new Vector2(100,b), 1f);
				AddBlock(blockSize,  -200,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, 0,b, 1, false);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, -30,b, 1, false);
				AddBlock(blockSize,  30,b);
			}
			else if (levelIndex == i++) {
				AddBlock(blockSize, 0,b, 2);
			}
			else {
				AddBlock(new Vector2(200,200), 0,b);
				Debug.LogError("No level data available for level: " + levelIndex);
			}
		}
		private void AddBlock(Vector2 blockSize, float x,float y, bool doTap) {
			Vector2 pos = new Vector2(x,y);
			AddBlock(blockSize, pos,pos, 0,0, 1, doTap);
		}
		private void AddBlock(Vector2 blockSize, float x,float y, int numHitsReq=1, bool doTap=true) {
			Vector2 pos = new Vector2(x,y);
			AddBlock(blockSize, pos,pos, 0,0, numHitsReq, doTap);
		}
		private void AddBlock(Vector2 blockSize, Vector2 posA,Vector2 posB, float travelSpeed, float startLocOffset=0, int numHitsReq=1, bool doTap=true) {
			if (resourcesHandler == null) { return; } // Safety check for runtime compile.
			Block newBlock = Instantiate(resourcesHandler.bouncePaint_block).GetComponent<Block>();
			newBlock.Initialize(this,rt_blocks, blockSize, posA,posB, travelSpeed, startLocOffset, numHitsReq, doTap);
			blocks.Add(newBlock);
		}


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