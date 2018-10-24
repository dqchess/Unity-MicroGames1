using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AbacusToy {
    public class GameController : BaseGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.AbacusToy; }
        // Objects
        private Level level;
        // References
        //[SerializeField] private FUEController fueController=null;
        // Editing Levels Stuff
        [Header ("Random Tile Params")]
        public float PercentTiles = 0.8f;
        public int NumColors = 3;
        public int Stickiness = 3;

        // Getters (Public)
        //public FUEController FUEController { get { return fueController; } }
        // Getters (Private)
        private LevelAddress currentAddress { get { return level==null?LevelAddress.zero : level.MyAddress; } }
        private PackData CurrentPackData { get { return levelsManager.GetPackData(currentAddress); } }
        private LevelsManager levelsManager { get { return LevelsManager.Instance; } }


        

        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start() {
            base.Start();
            
            // Load rand tile placement properties!
            PercentTiles = SaveStorage.GetFloat(SaveKeys.AbacusToy_RandGenPercentTiles, 0.8f);
            NumColors = SaveStorage.GetInt(SaveKeys.AbacusToy_NumColors, 3);
            Stickiness = SaveStorage.GetInt(SaveKeys.AbacusToy_RandGenStickiness, 1);
            
            // In the editor? Reload levels!
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
    
            // Start at the level we've most recently played!
            SetCurrentLevel(levelsManager.GetLastPlayedLevelAddress());
            
            // Add event listeners!
            GameManagers.Instance.EventManager.LevelJumpButtonClickEvent += OnLevelJumpButtonClick;
            GameManagers.Instance.EventManager.QuitGameplayButtonClickEvent += OnQuitGameplayButtonClick;
        }
        override protected void OnDestroy() {
            base.OnDestroy();
            
            // Save tile placement properties!
            SaveStorage.SetFloat(SaveKeys.AbacusToy_RandGenPercentTiles, PercentTiles);
            SaveStorage.SetInt(SaveKeys.AbacusToy_NumColors, NumColors);
            SaveStorage.SetInt(SaveKeys.AbacusToy_RandGenStickiness, Stickiness);
            
            // Remove event listeners!
            GameManagers.Instance.EventManager.LevelJumpButtonClickEvent -= OnLevelJumpButtonClick;
            GameManagers.Instance.EventManager.QuitGameplayButtonClickEvent -= OnQuitGameplayButtonClick;
        }
        
        private void OnQuitGameplayButtonClick() {
            OpenScene(SceneNames.LevelSelect(MyGameName()));
        }
        private void OnLevelJumpButtonClick(int levelIndexChange) {
            ChangeLevel(levelIndexChange);
        }
        
        

        // ----------------------------------------------------------------
        //  Doers - Loading Level
        // ----------------------------------------------------------------
        private IEnumerator Coroutine_JustWonLevel() {
            yield return new WaitForSecondsRealtime(0.32f);

            // Wait until there's no touch on the screen.
            while (inputController.IsTouchHold()) { yield return null; }
            
            //// Show LevelCompletePopup, and wait until we press its Next button!
            //LevelCompletePopup popup = level.LevelUI.LevelCompletePopup;
            //popup.Appear();
            //while (!popup.DidPressNextButton) { yield return null; }
            
            SetCurrentLevel(currentAddress.NextLevel, true);
        }
        private bool didPressLevelCompletePopupNextButton;

        public void RestartLevel() { SetCurrentLevel(currentAddress, false); }
        private void StartPrevLevel() {
            LevelData data = levelsManager.GetLevelData(currentAddress.PreviousLevel);
            if (data != null) { SetCurrentLevel(data); }
        }
        private void StartNextLevel () {
            LevelData data = levelsManager.GetLevelData (currentAddress.NextLevel);
            if (data != null) { SetCurrentLevel(data); }
            else { OnCompleteLastLevelInPack(); } // If we've reached the end of this world...
        }
        private void ChangeLevel(int levelIndexChange) {
            LevelAddress newAddress = currentAddress;
            newAddress.level = Mathf.Max(0, newAddress.level+levelIndexChange);
            SetCurrentLevel(newAddress);
        }
        private void SetCurrentLevel(LevelAddress address, bool doAnimate=false) {
            if (Application.isEditor) { // In editor? Noice. Reload all levels from file so we can update during runtime!
                levelsManager.ReloadModeDatas();
            }
            LevelData ld = levelsManager.GetLevelData (address);
            if (ld == null) { Debug.LogError("Requested LevelData doesn't exist! Address: " + address.ToString()); } // Useful feedback for dev.
            SetCurrentLevel(ld, doAnimate);
        }
    
        private void SetCurrentLevel(LevelData levelData, bool doAnimate=false) {
            Level oldLevel = level;

            // Make the new level!
            InitializeLevel(levelData);

            if (doAnimate) { StartCoroutine(Coroutine_AnimateLevelTransition(oldLevel)); }
            else { DestroyOldLevel(oldLevel); }
        }
        private IEnumerator Coroutine_AnimateLevelTransition(Level oldLevel) {
            level.IsAnimating = true;
            oldLevel.IsAnimating = true;
    
            float duration = 1.2f;
            float height = 1200;
            Vector3 levelDefaultPos = level.transform.localPosition;
            level.transform.localPosition += new Vector3(0, height, 0);
            LeanTween.moveLocal(level.gameObject, levelDefaultPos, duration).setEaseInOutQuart();
            LeanTween.moveLocal(oldLevel.gameObject, new Vector3(0, -height, 0), duration).setEaseInOutQuart();
            yield return new WaitForSeconds(duration);
    
            level.IsAnimating = false;
            DestroyOldLevel(oldLevel);
        }
        private void DestroyOldLevel(Level oldLevel) {
            if (oldLevel!=null) { Destroy(oldLevel.gameObject); }
        }
        
        private void InitializeLevel(LevelData ld) {
            if (ld == null) {
                Debug.LogError ("Can't load the requested level! Can't find its LevelData.");
                if (level == null) { // If there's no currentLevel, yikes! Default us to something.
                    ld = levelsManager.GetLevelData(0,0,0,0);
                }
                else { return; } // If there IS a currentLevel, let's just stay there!
            }
    
            // Instantiate the Level from the provided LevelData!
            level = Instantiate(resourcesHandler.abacusToy_level).GetComponent<Level>();
            level.Initialize(this, canvas.transform, ld);
            
            // Reset basic stuff
            SetIsPaused(false);
            // Tell people!
            //fueController.OnStartLevel(level);
            // Save values!
            SaveStorage.SetString(SaveKeys.AbacusToy_LastPlayedLevelAddress(currentAddress), currentAddress.ToString());
        }
        
        
        

        private void OnCompleteLastLevelInPack() {
            //// We just completed the tutorial?? Save that!!
            //if (currentAddress.IsTutorial) {
            //    SaveStorage.SetInt(SaveKeys.DidCompleteTutorial, 1);
            //    // TEMP jump straight into the first level of Lettersmash!
            //    SetCurrentLevel(new LevelAddress(GameModes.LetterSmashIndex, 0,0,0));
            //    return;
            //}
            //// Quit outta this level back to some menu (the menu'll depend on our mode).
            //QuitToMenu();
        }
    
    
        public void QuitToMenu() {
            //// We just completed the tutorial??
            //if (currentAddress.IsTutorial) {
            //    MainMenuController.startingMenuName = MenuNames.ModeSelect;
            //}
            //// Otherwise, just boot us back to LevelSelect!
            //else {
            //    MainMenuController.startingMenuName = MenuNames.LevelSelect;
            //}
            //OpenScene(SceneNames.MainMenu);
        }
        
        

        // ----------------------------------------------------------------
        //  Game Events
        // ----------------------------------------------------------------
        private void WinLevel() {
            // Tell people!
            levelsManager.OnCompleteLevel(currentAddress);
            level.OnWinLevel();
            //fueController.OnCompleteLevel();
            FBAnalyticsController.Instance.OnWinLevel(MyGameName(), currentAddress);
            StartCoroutine(Coroutine_JustWonLevel());
        }

        public void OnBoardGoalsSatisfied() {
            WinLevel();
        }

        // ----------------------------------------------------------------
        //  Input
        // ----------------------------------------------------------------
        override protected void RegisterButtonInput() {
            base.RegisterButtonInput();

            // R = Restart Level (without reloading scene)
            if (Input.GetKeyDown(KeyCode.R)) {
                RestartLevel();
                return;
            }
    
            // DEBUG
            if (Input.GetKeyDown(KeyCode.P))            { ChangeLevel(-10); return; } // P = Back 10 levels.
            if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangeLevel( -1); return; } // [ = Back 1 level.
            if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangeLevel(  1); return; } // ] = Ahead 1 level.
            if (Input.GetKeyDown(KeyCode.Backslash))    { ChangeLevel( 10); return; } // \ = Ahead 10 levels.
        }


    }


}




/*
	public class GameController : BaseLevelGameController {
		// Overrideables
		override public string MyGameName() { return GameNames.AbacusToy; }
		// Objects
		private Level level; // MY game-specific Level class.


		override protected void WinLevel() {
			base.WinLevel();
			StartCoroutine(Coroutine_StartNextLevel());
//			// Tell people!
//			level.OnWinLevel();
//			// Update best score!
//			int bestScore = SaveStorage.GetInt(SaveKeys.BestScore(MyGameName(), LevelIndex));
//			if (scoreSolidified > bestScore) {
//				SaveStorage.SetInt(SaveKeys.BestScore(MyGameName(),LevelIndex), scoreSolidified);
//			}
		}

		private IEnumerator Coroutine_StartNextLevel() {
			yield return new WaitForSecondsRealtime(0.5f);//1.2f);
			SetCurrentLevel(LevelIndex+1, true);
		}

		override protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false) {
			StopCoroutine("Coroutine_SetCurrentLevel");
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
			InitializeLevel(Instantiate(resourcesHandler.abacusToy_level), _levelIndex);

			// DO animate!
			if (doAnimate) {
				float duration = 1.2f;

				level.IsAnimating = true;
				prevLevel.IsAnimating = true;
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

			yield return null;
		}

		// game events
		public void OnBoardGoalsSatisfied() {
			WinLevel();
		}


		// ----------------------------------------------------------------
		//  Input
		// ----------------------------------------------------------------
		override protected void OnTapDown() { }
		override protected void OnTapUp() { }


		// ----------------------------------------------------------------
		//  Debug
		// ----------------------------------------------------------------
		override protected void Debug_WinLevel() {
			WinLevel();
		}





	}


}
*/


