using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlideAndStick {
    /** Note that this GameController /manually/ trickles down Update calls. To ensure no accidental Update calls when we shouldn't have any (i.e. when blocked by LevSel). */
	public class GameController : BaseGameController {
        // Constants
        private const int NumPlaysBetweenAds = 5;
		// Overrideables
		override public string MyGameName() { return GameNames.SlideAndStick; }
		// Components
		[SerializeField] public RandGenParams randGenParams=null;
		private Level level;
        // Properties
        private List<BoardData> debug_prevBoardDatas=new List<BoardData>(); // for making rand lvls. Press E to restore the last level, in case we pressed R accidentally and lost it.
        // References
        [SerializeField] private GameObject go_levelContainer=null;
        [SerializeField] private GameBackground background=null;
        [SerializeField] private CoreMenuController coreMenuController=null;
        [SerializeField] private FUEController fueController=null;
        [SerializeField] private GameObject go_toggleLevSelButton=null;
        [SerializeField] private SlideAndStickSfxController sfxController=null;

		// Getters (Public)
        public FUEController FUEController { get { return fueController; } }
        public SlideAndStickSfxController SFXController { get { return sfxController; } }
        // Getters (Private)
        private bool IsBlockedByLevSel { get { return coreMenuController.IsGameControllerBlockedByLevSel(); } }
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
            bool debug_supressAnimation = Input.GetKey(KeyCode.A); // DEBUG for TESTING!
            LevelData ld = levelsManager.GetLastPlayedLevelData();
            SetCurrentLevel(ld, !debug_supressAnimation);
            
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
            
            // NOT last level in pack? Start next level!
            if (!levelsManager.IsLastLevelInPack(currAddress)) {
                StartNextLevel();
            }
            else {
				bool wasTutorial = levelsManager.IsTutorial(currAddress);
                // What's the next level we can play? Start it!
                RolloverPackStartNextLevel();
				// WAS the tutorial?? Tell FUEController!
				if (wasTutorial) {
					fueController.ForcePlayerToOpenLevSel();
				}
				// Was NOT tutorial. Open LevSel!
				else {
                	coreMenuController.OpenLevSelController(false);
				}
            }
        }

        public void RestartLevel() { SetCurrentLevel(currAddress, false); }
        //private void StartPrevLevel() {
        //    LevelData data = levelsManager.GetLevelData(currAddress.PreviousLevel);
        //    if (data != null) { SetCurrentLevel(data); }
        //}
        private void StartNextLevel() {
            LevelData data = levelsManager.GetLevelData(currAddress.NextLevel);
            if (data != null) { SetCurrentLevel(data, true); }
        }
        private void RolloverPackStartNextLevel() {
            LevelData data = levelsManager.GetRolloverPackNextLevelData(currAddress);
            if (data != null) { SetCurrentLevel(data, false); } // Note: Don't animate; LevSel's covering us now.
        }
        private void ChangeCollection(int change) {
            SetCurrentLevel(new LevelAddress(currAddress.mode, currAddress.collection+change, 0, 0));
        }
        private void ChangePack(int change) {
            SetCurrentLevel(new LevelAddress(currAddress.mode, currAddress.collection, currAddress.pack+change, 0));
        }
        private void ChangeLevel(int change) {
            SetCurrentLevel(currAddress + new LevelAddress(0, 0, 0, change));
        }
        public void SetCurrentLevel(LevelAddress address, bool doAnimate=false) {
            if (Application.isEditor) { // In editor? Noice. Reload all levels from file so we can update during runtime!
                levelsManager.Reset();
            }
            address = address.NoNegatives();
            LevelData ld = levelsManager.GetLevelData(address);
            if (ld == null) { Debug.LogError("Requested LevelData doesn't exist! Address: " + address.ToString()); } // Useful feedback for dev.
            SetCurrentLevel(ld, doAnimate);
        }
    
        private void SetCurrentLevel(LevelData levelData, bool doAnimate=false) {
			Level oldLevel = level;

			InitializeLevel(levelData);
            
            // Animate in/out!
			if (doAnimate) {
                level.AnimateIn();
                if (oldLevel != null) {
                    oldLevel.AnimateOut();
                }
                background.SpeedUpParticlesforLevelTrans();
            }
            // No animating.
			else {
                level.AnimateInBoard();
                if (oldLevel != null) {
                    oldLevel.DestroySelf();
                }
            }
		}
        
        //private IEnumerator Coroutine_AnimateLevelsInOut(Level l, Level oldLevel) {
        //    // Animate the dudes in/out.
        //    // Speed up/slow down the particles.
        //        StartCoroutine(Coroutine_AnimateLevelsInOut(level, oldLevel));
        //}
        
        private void InitializeLevel(LevelData ld) {
            if (ld == null) {
                Debug.LogError ("Can't load the requested level! Can't find its LevelData.");
                if (level == null) { // If there's no currentLevel, yikes! Default us to something.
                    ld = levelsManager.GetFallbackEmptyLevelData();
                }
                else { return; } // If there IS a currentLevel, let's just stay there!
            }
    
            // Instantiate the Level from the provided LevelData!
            level = Instantiate(resourcesHandler.slideAndStick_level).GetComponent<Level>();
            level.Initialize(this, go_levelContainer.transform, ld);
            debug_prevBoardDatas.Add(level.Board.SerializeAsData());
            
            // Reset basic stuff
            SetIsPaused(false);
            // Tell people!
            fueController.OnStartLevel(level);
            levelsManager.selectedAddress = currAddress; // for consistency.
            bool isTutorial = levelsManager.IsTutorial(currAddress);
            go_toggleLevSelButton.SetActive(!isTutorial); // hide menu button in tutorial!
            // Save values!
            //SaveStorage.SetString(SaveKeys.SlideAndStick_LastPlayedLevelLocal(currAddress), currAddress.ToString());
            SaveStorage.SetString(SaveKeys.SlideAndStick_LastPlayedLevelGlobal, currAddress.ToString());
            
            // Maybe show ad!
            int playsUntilAd = SaveStorage.GetInt(SaveKeys.SlideAndStick_PlaysUntilAd, NumPlaysBetweenAds);
            playsUntilAd --;
            SaveStorage.SetInt(SaveKeys.SlideAndStick_PlaysUntilAd, playsUntilAd);
            if (playsUntilAd <= 0) {
                ShowAd();
            }
        }
        
        private void ShowAd() {
            // Show an ad!
            AdManager.instance.showInterstitial();
            // Reset PlaysUntilAd.
            SaveStorage.SetInt(SaveKeys.SlideAndStick_PlaysUntilAd, NumPlaysBetweenAds);
        }
    
        
        

		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
        public void OnBoardGoalsSatisfied() {
            WinLevel();
        }
        private void WinLevel() {
            // Tell people!
            levelsManager.OnCompleteLevel(currAddress);
            level.OnWinLevel();
            fueController.OnCompleteLevel();
            sfxController.OnCompleteLevel();
            FBAnalyticsController.Instance.OnWinLevel(MyGameName(), currAddress);
            
            // #forchristian Here's where WinLevel is called! Put what you need here.
            // Note: currAddress has mode (not used), collection (difficulty), pack (board size), and level.
            //ChristianFunAnalyticsHandler.DispatchOneGroovyEvent(currAddress.collection, currAddress.pack, currAddress.level);
            
            if (levelsManager.IsLastLevelInPack(currAddress)) {
                OnCompleteLastLevelInPack();
            }
            // Preemptively save the NEXT level as the last one played! In case we quit now and reopen the game (we don't wanna open to the lvl we just beat).
            SaveStorage.SetString(SaveKeys.SlideAndStick_LastPlayedLevelGlobal, currAddress.NextLevel.ToString());
            // Start next level business!
            StartCoroutine(Coroutine_JustWonLevel());
        }
        private void OnCompleteLastLevelInPack() {
            // We just completed the tutorial?? Save that!!
            if (levelsManager.IsTutorial(currAddress)) {
                SaveStorage.SetInt(SaveKeys.SlideAndStick_DidCompleteTutorial, 1);
            }
        }



        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        override protected void Update () {
            // DEBUG! S = Save screenshot.
            if (Input.GetKeyDown(KeyCode.S)) { ScreenCapture.CaptureScreenshot("screenshot.png"); }
            
            if (!IsBlockedByLevSel) { return; }
            base.Update();
            // Update my dependencies!
            level.DependentUpdate();
            fueController.DependentUpdate();
        }
        private void FixedUpdate() {
            if (!IsBlockedByLevSel) { return; }
            // Update my dependencies!
            level.DependentFixedUpdate();
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
            bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (isKey_shift) {
                //if (Input.GetKeyDown(KeyCode.S)) { levelsManager.Debug_SaveReplaceXMLLayout(level); }
            }
            else if (isKey_ctrl) {
                if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangeCollection(-1); return; }
                if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangeCollection( 1); return; }
            }
            else if (isKey_alt) {
                if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangePack(-1); return; }
                if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangePack( 1); return; }
            }
            else {
                if (Input.GetKeyDown(KeyCode.P))            { ChangeLevel(-10); return; } // P = Back 10 levels.
                if (Input.GetKeyDown(KeyCode.LeftBracket))  { ChangeLevel( -1); return; } // [ = Back 1 level.
                if (Input.GetKeyDown(KeyCode.RightBracket)) { ChangeLevel(  1); return; } // ] = Ahead 1 level.
                if (Input.GetKeyDown(KeyCode.Backslash))    { ChangeLevel( 10); return; } // \ = Ahead 10 levels.
                
                // W = Win!
                if (Input.GetKeyDown(KeyCode.W)) {
                    WinLevel();
                }
                // E = Restore prev Board! In case we had one we liked but accidentally made a new one.
                if (Input.GetKeyDown(KeyCode.E)) {
                    Debug_RestorePrevBoard();
                    return;
                }
            }
            
        }
        
        
        public void RecedeIntoBackground() {
            LeanTween.cancel(go_levelContainer);
            float duration = 0.7f;
            Vector3 posTo = new Vector3(100, 0,0);//200
            LeanTween.scale(go_levelContainer, Vector3.one*0.74f, duration).setEaseOutQuart();
            LeanTween.moveLocal(go_levelContainer, posTo, duration).setEaseOutQuart();
			// Tell people.
			fueController.OnGameControllerRecedeIntoBackground();
        }
        public void ReturnToForeground() {
            LeanTween.cancel(go_levelContainer.gameObject);
            float duration = 0.5f;
            Vector3 posTo = new Vector3(0,0,0);
            LeanTween.scale(go_levelContainer, Vector3.one, duration).setEaseOutQuart();
            LeanTween.moveLocal(go_levelContainer, posTo, duration).setEaseOutQuart();
        }
        
        

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Debug
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        if (UnityEditor.EditorApplication.isPlaying)  {
            CoreMenuController.Debug_HideMenuOnStart = true; // Tell CoreMenuController NOT to show the Combi menu when we reload the scene.
            ReloadScene();
        }
    }
#endif
        private void Debug_RestorePrevBoard() {
            if (debug_prevBoardDatas.Count > 1) {
                BoardData snapshot = debug_prevBoardDatas[debug_prevBoardDatas.Count-2];
                debug_prevBoardDatas.RemoveAt(debug_prevBoardDatas.Count-1);
                level.Debug_RemakeBoardAndViewFromArbitrarySnapshot(snapshot);
            }
        }


	}


}



