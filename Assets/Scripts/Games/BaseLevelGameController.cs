using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameStates { Playing, GameOver, PostLevel }

abstract public class BaseLevelGameController : BaseGameController {
	// Properties
	private GameStates gameState;
	protected float timeWhenLevelStarted; // in UNSCALED SECONDS.
	private   float timeWhenLevelEnded; // in UNSCALED SECONDS.
	// Components
	protected BaseLevel baseLevel;
	private LevelLoader levelLoader; // this is added in Start! :)
	// References
	[SerializeField] private LevelGameUI levelGameUI=null; // All BaseLevelGames come with a boilerplate LevelGameUI. Retry, Quit, and Debug buttons.

	// Getters (Public)
	public bool IsGameStatePlaying { get { return gameState==GameStates.Playing; } } // NOTE: This can be improved! This'll cause confusion when animating lvls in/out, as we say we're playing, but not WHICH lvl we're playing. :P
	public bool IsLevelComplete { get { return gameState == GameStates.PostLevel; } }
	public GameStates GameState { get { return gameState; } }
	public LevelLoader LevelLoader { get { return levelLoader; } }
	// Getters (Protected)
	protected int LevelIndex { get { return baseLevel.LevelIndex; } }
	// Getters (Private)
	private Vector2 GetMousePosLevel() {
		Vector2 mousePos = InputController.Instance.TouchPosScaled;
//		mousePos -= new Vector2(baseLevel.transform.localPosition.x,baseLevel.transform.localPosition.y);
		mousePos -= new Vector2(300,400); // HACK! This is hardcoded. I'm only using this function for debugging so nbd atm.
		return mousePos;
	}


	// ----------------------------------------------------------------
	//  Abstract Functions
	// ----------------------------------------------------------------
	/** Initialize our game-specific Level class AND set baseLevel ref.
	 * This is a required function to make SURE we set baseLevel for each extension of this class. */
	abstract protected void InitializeLevel(GameObject _levelGO, int _levelIndex);
    abstract protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false);
    abstract protected void OnTapDown();
    abstract protected void OnTapUp();
	abstract protected void Debug_WinLevel();


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	override protected void Start () {
        // In the editor? Reload our levels file!
        #if UNITY_EDITOR
        AssetDatabase.ImportAsset("Assets/" + LevelLoader.LevelsFilePath(MyGameName()) + ".txt");
        #endif

		// Initialize LevelLoader!
		levelLoader = gameObject.AddComponent<LevelLoader>();
		levelLoader.ReloadLevelsFile(MyGameName());

		base.Start();

		SetCurrentLevel(SaveStorage.GetInt(SaveKeys.LastLevelPlayed(MyGameName()), 1));

		// Add event listeners!
		GameManagers.Instance.EventManager.LevelJumpButtonClickEvent += OnLevelJumpButtonClick;
		GameManagers.Instance.EventManager.RetryButtonClickEvent += OnRetryButtonClick;
		GameManagers.Instance.EventManager.QuitGameplayButtonClickEvent += OnQuitGameplayButtonClick;
	}
	override protected void OnDestroy() {
		// Remove event listeners!
		GameManagers.Instance.EventManager.LevelJumpButtonClickEvent -= OnLevelJumpButtonClick;
		GameManagers.Instance.EventManager.RetryButtonClickEvent -= OnRetryButtonClick;
		GameManagers.Instance.EventManager.QuitGameplayButtonClickEvent -= OnQuitGameplayButtonClick;

		base.OnDestroy();
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void OpenScene_LevelSelect() {
		OpenScene(SceneNames.LevelSelect(MyGameName()));
	}
	private void UpdateHighestLevelUnlocked(int _levelIndex) {
		int highestRecord = SaveStorage.GetInt(SaveKeys.HighestLevelUnlocked(MyGameName()));
		if (_levelIndex > highestRecord) {
			SaveStorage.SetInt(SaveKeys.HighestLevelUnlocked(MyGameName()), _levelIndex);
		}
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnLevelJumpButtonClick(int levelIndexChange) {
		ChangeLevel(levelIndexChange);
	}
	protected void OnRetryButtonClick() {
		RestartLevel();
	}
	private void OnQuitGameplayButtonClick() {
		OpenScene_LevelSelect();
	}


	// ----------------------------------------------------------------
	//  Game Flow
	// ----------------------------------------------------------------
	private void RestartLevel() { SetCurrentLevel(LevelIndex); }
	private void ChangeLevel(int levelIndexChange) {
		SetCurrentLevel(Mathf.Max(1, LevelIndex+levelIndexChange));
	}
//	public void StartPrevLevel() { SetCurrentLevel(Mathf.Max(1, LevelIndex-1)); }
//	public void StartNextLevel() { SetCurrentLevel(LevelIndex+1); }
//	/// Jumps *10* levels back.
//	public void StartPrevLevel10() { SetCurrentLevel(Mathf.Max(1, LevelIndex-10)); }
//	/// Jumps *10* levels forward.
//	public void StartNextLevel10() { SetCurrentLevel(LevelIndex+10); }

	/** This function handles a bunch of random paperwork.
	 * For consistency, call this within InitializeLevel. */
	protected void OnInitializeLevel(BaseLevel _baseLevel) {
		// Assign baseLevel reference here!
		baseLevel = _baseLevel;
		// Reset basic stuff
		SetIsPaused(false);
		timeWhenLevelStarted = Time.unscaledTime;
		timeWhenLevelEnded = -1;
		gameState = GameStates.Playing;
		// Save values!
		SaveStorage.SetInt(SaveKeys.LastLevelPlayed(MyGameName()), LevelIndex);
		// Tell people!
		levelGameUI.OnStartLevel(LevelIndex);
	}


	virtual public void LoseLevel() {
		gameState = GameStates.GameOver;
		timeWhenLevelEnded = Time.unscaledTime;
		IncrementTimeSpentTotal();
		IncrementNumLosses();
		// Tell people!
		FBAnalyticsController.Instance.OnLoseLevel(MyGameName(), LevelIndex, GetTimeSpentThisPlay());
		levelGameUI.OnGameOver();
	}
	virtual protected void WinLevel() {
		gameState = GameStates.PostLevel;
		timeWhenLevelEnded = Time.unscaledTime;
		IncrementTimeSpentTotal();
		IncrementNumWins();
		UpdateHighestLevelUnlocked(LevelIndex);
		// Tell people!
		FBAnalyticsController.Instance.OnWinLevel(MyGameName(), LevelIndex);
	}
	private void IncrementNumLosses() {
		string saveKey = SaveKeys.NumLosses(MyGameName(), LevelIndex);
		int numLosses = SaveStorage.GetInt(saveKey,0);
		SaveStorage.SetInt(saveKey, numLosses + 1);
	}
	private void IncrementNumWins() {
		string saveKey = SaveKeys.NumWins(MyGameName(), LevelIndex);
		int numWins = SaveStorage.GetInt(saveKey,0);
		SaveStorage.SetInt(saveKey, numWins + 1);
	}
	private void IncrementTimeSpentTotal() {
		float timeSpentThisPlay = GetTimeSpentThisPlay();
		string saveKey = SaveKeys.TimeSpentTotal(MyGameName(), LevelIndex);
		float timeSpentTotal = SaveStorage.GetFloat(saveKey,0);
		SaveStorage.SetFloat(saveKey, timeSpentTotal+timeSpentThisPlay);
	}
	/// in UNSCALED SECONDS. How long the player was playing this level.
	private float GetTimeSpentThisPlay() {
		if (gameState == GameStates.Playing) { Debug.LogWarning("Whoa! We can't ask for timeSpentThisPlay if we're still playing!"); }
		return timeWhenLevelEnded - timeWhenLevelStarted;
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
            OnTapDown();
        }
        else if (Input.GetMouseButtonUp(0)) {
            OnTapUp();
        }

		// Debug print mouse position.
		if (Input.GetMouseButtonDown(1)) { Debug.Log("Mouse pos in level: " + GetMousePosLevel()); }
	}
	override protected void RegisterButtonInput() {
		base.RegisterButtonInput();

//		bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.Space)) { OnTapDown(); }
        else if (Input.GetKeyUp(KeyCode.Space)) { OnTapUp(); }

		// DEBUG
		if (Input.GetKeyDown(KeyCode.P))			{ ChangeLevel(-10); return; } // P = Back 10 levels.
		if (Input.GetKeyDown(KeyCode.LeftBracket))	{ ChangeLevel( -1); return; } // [ = Back 1 level.
		if (Input.GetKeyDown(KeyCode.RightBracket))	{ ChangeLevel(  1); return; } // ] = Ahead 1 level.
		if (Input.GetKeyDown(KeyCode.Backslash))	{ ChangeLevel( 10); return; } // \ = Ahead 10 levels.
		if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
	}




}
