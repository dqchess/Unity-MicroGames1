using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlideAndStick {
	public class GameController : BaseGameController {
		// Overrideables
		override public string MyGameName() { return GameNames.SlideAndStick; }
		// Components
		[SerializeField] public RandGenParams randGenParams=null;
		private Level level;
		// References
		[SerializeField] private FUEController fueController=null;

		// Getters (Public)
		public FUEController FUEController { get { return fueController; } }
        // Getters (Private)
        private LevelAddress currAddress { get { return level==null?LevelAddress.zero : level.MyAddress; } }
        private PackData CurrentPackData { get { return levelsManager.GetPackData(currAddress); } }
        private LevelsManager levelsManager { get { return LevelsManager.Instance; } }


        

        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        override protected void Start() {
            base.Start();
            
            // In the editor? Reload levels!
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
    //      AssetDatabase.ImportAsset(LevelLoader.LevelsFilePath(MyGameName(), true));
            #endif
    
            // Start at the level we've most recently played!
            SetCurrentLevel(levelsManager.GetLastPlayedLevelAddress());
            
            // Add event listeners!
            GameManagers.Instance.EventManager.LevelJumpButtonClickEvent += OnLevelJumpButtonClick;
            GameManagers.Instance.EventManager.QuitGameplayButtonClickEvent += OnQuitGameplayButtonClick;
        }
        override protected void OnDestroy() {
            base.OnDestroy();
            
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
            
            // Show LevelCompletePopup, and wait until we press its Next button!
            LevelCompletePopup popup = level.LevelUI.LevelCompletePopup;
            popup.Appear();
            while (!popup.DidPressNextButton) { yield return null; }
            
            //yield return new WaitForSecondsRealtime(0.1f);
            SetCurrentLevel(currAddress.NextLevel, true);
        }
        private bool didPressLevelCompletePopupNextButton;

        private void RestartLevel() { SetCurrentLevel(currAddress, false); }
        private void StartPrevLevel() {
            LevelData data = levelsManager.GetLevelData(currAddress.PreviousLevel);
            if (data != null) { SetCurrentLevel(data); }
        }
        private void StartNextLevel () {
            LevelData data = levelsManager.GetLevelData (currAddress.NextLevel);
            if (data != null) { SetCurrentLevel(data); }
            else { OnCompleteLastLevelInPack(); } // If we've reached the end of this world...
        }
        private void ChangeCollection(int change) {
            //SetCurrentLevel(currentAddress + new LevelAddress(0, change, 0, 0));
            SetCurrentLevel(new LevelAddress(currAddress.mode, currAddress.collection+change, 0, 0));
        }
        private void ChangePack(int change) {
            //SetCurrentLevel(currentAddress + new LevelAddress(0, 0, change, 0));
            SetCurrentLevel(new LevelAddress(currAddress.mode, currAddress.collection, currAddress.pack+change, 0));
        }
        private void ChangeLevel(int change) {
            SetCurrentLevel(currAddress + new LevelAddress(0, 0, 0, change));
        }
        private void SetCurrentLevel(LevelAddress address, bool doAnimate=false) {
            if (Application.isEditor) { // In editor? Noice. Reload all levels from file so we can update during runtime!
                levelsManager.Reset();
            }
            address = address.NoNegatives();
            LevelData ld = levelsManager.GetLevelData (address);
            if (ld == null) { Debug.LogError("Requested LevelData doesn't exist! Address: " + address.ToString()); } // Useful feedback for dev.
            SetCurrentLevel(ld, doAnimate);
        }
    
        private void SetCurrentLevel(LevelData levelData, bool doAnimate=false) {
		//	StopCoroutine("Coroutine_SetCurrentLevel");
		//	StartCoroutine(Coroutine_SetCurrentLevel(levelData, doAnimate));
		//}
		//private IEnumerator Coroutine_SetCurrentLevel(LevelData levelData, bool doAnimate) {
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
            level = Instantiate(resourcesHandler.slideAndStick_level).GetComponent<Level>();
            level.Initialize(this, canvas.transform, ld);
            
            // Reset basic stuff
            SetIsPaused(false);
            //gameState = GameStates.Playing;
            // Tell people!
            fueController.OnStartLevel(level);
            // Save values!
            SaveStorage.SetString(SaveKeys.SlideAndStick_LastPlayedLevelAddress(currAddress), currAddress.ToString());
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
            levelsManager.OnCompleteLevel(currAddress);
            level.OnWinLevel();
            fueController.OnCompleteLevel();
            FBAnalyticsController.Instance.OnWinLevel(MyGameName(), currAddress);
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

            // R = Reload Level (without reloading scene)
            if (Input.GetKeyDown(KeyCode.R)) {
                RestartLevel();
                return;
            }
    
            // DEBUG
            bool isKey_alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool isKey_ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (isKey_ctrl) {
                if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangeCollection(-1); return; }
                if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangeCollection( 1); return; }
            }
            else if (isKey_alt) {
                if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangePack(-1); return; }
                if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangePack( 1); return; }
            }
            if (Input.GetKeyDown(KeyCode.P))            { ChangeLevel(-10); return; } // P = Back 10 levels.
            if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangeLevel( -1); return; } // [ = Back 1 level.
            if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangeLevel(  1); return; } // ] = Ahead 1 level.
            if (Input.GetKeyDown(KeyCode.Backslash))    { ChangeLevel( 10); return; } // \ = Ahead 10 levels.
            
        }


	}


}



