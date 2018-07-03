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
		static private int temp_lastPlayedLevelIndex=1; // TEMP! For faster testing.
        // Components
        [SerializeField] private Player player=null;
        private List<Block> blocks;
        // References
		[SerializeField] private Canvas myCanvas;
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

			SetCurrentLevel(temp_lastPlayedLevelIndex);
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
			temp_lastPlayedLevelIndex = currentLevelIndex;

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



		// HARDCODED Level-adding. Hardcoded for now.
		private void AddLevelComponents(int levelIndex) {
			DestroyAllBlocks(); // Just in case
			blocks = new List<Block>();

			Vector2 blockSize = new Vector2(50,50);

			// NOTE: All coordinates are based off of a 600x800 available playing space! :)

			float b = -240; // bottom.
			int i=1; // TEMP! Until we make levels into XML or Json.
			if (false) {}
			else if (levelIndex == i++) {
//				AddBlock(blockSize, new Vector2(0,b), new Vector2(100,b), 1f);
//				AddBlock(blockSize, new Vector2(100,b), new Vector2(0,b), 1f);
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
			else {
				Debug.LogError("No level data available for level: " + levelIndex);
			}
		}
		private void AddBlock(Vector2 blockSize, float x,float y, int numHitsReq=1, bool doTap=true) {
			Vector2 pos = new Vector2(x,y);
			AddBlock(blockSize, pos,pos, 0, numHitsReq, doTap);
		}
		private void AddBlock(Vector2 blockSize, Vector2 posA,Vector2 posB, float travelSpeed, int numHitsReq=1, bool doTap=true) {
			Block newBlock = Instantiate(resourcesHandler.bouncePaint_block).GetComponent<Block>();
			newBlock.Initialize(this,rt_blocks, blockSize, posA,posB, travelSpeed, numHitsReq, doTap);
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