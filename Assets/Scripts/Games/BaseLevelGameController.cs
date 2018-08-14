using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { Playing, GameOver, PostLevel }

abstract public class BaseLevelGameController : BaseGameController {
	// Overrideables
	abstract public string MyGameName();
	// Properties
	private GameStates gameState;
	private float timeWhenLevelEnded;
	// Components
	protected BaseLevel baseLevel;
	// References
	[SerializeField] protected Canvas canvas=null;
	[SerializeField] private LevelGameUI levelGameUI=null; // All BaseLevelGames come with a boilerplate LevelGameUI. Retry, Quit, and Debug buttons.

	// Getters (Public)
	public bool IsLevelComplete { get { return gameState == GameStates.PostLevel; } }
	public Canvas Canvas { get { return canvas; } }
	// Getters (Protected)
	public bool IsGameStatePlaying { get { return gameState==GameStates.Playing; } }
	protected int LevelIndex { get { return baseLevel.LevelIndex; } }


	// ----------------------------------------------------------------
	//  Abstract Functions
	// ----------------------------------------------------------------
	/** Initialize our game-specific Level class AND set baseLevel ref.
	 * This is a required function to make SURE we set baseLevel for each extension of this class. */
	abstract protected void InitializeLevel(GameObject _levelGO, int _levelIndex);
	abstract protected void SetCurrentLevel(int _levelIndex, bool doAnimate=false);
	abstract protected void OnTapScreen();
	abstract protected void Debug_WinLevel();


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	override protected void Start () {
		base.Start();

		SetCurrentLevel(SaveStorage.GetInt(SaveKeys.LastLevelPlayed(MyGameName()), 1));
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void OpenScene_LevelSelect() {
		OpenScene(SceneNames.LevelSelect(MyGameName()));
	}
	private void UpdateHighestLevelUnlocked(int _levelIndex) {
		int highestRecord = SaveStorage.GetInt(SaveKeys.HighestLevelUnlocked(MyGameName()));
		if (_levelIndex > highestRecord) {
			SaveStorage.SetInt(SaveKeys.HighestLevelUnlocked(MyGameName()), _levelIndex);
		}
	}


	// ----------------------------------------------------------------
	//  Game Flow
	// ----------------------------------------------------------------
	private void RestartLevel() { SetCurrentLevel(LevelIndex); }
	public void StartPrevLevel() { SetCurrentLevel(Mathf.Max(1, LevelIndex-1)); }
	public void StartNextLevel() { SetCurrentLevel(LevelIndex+1); }
	/// Jumps *10* levels back.
	public void StartPrevLevel10() { SetCurrentLevel(Mathf.Max(1, LevelIndex-10)); }
	/// Jumps *10* levels forward.
	public void StartNextLevel10() { SetCurrentLevel(LevelIndex+10); }

	/** This function handles a bunch of random paperwork.
	 * For consistency, call this within InitializeLevel. */
	protected void OnInitializeLevel(BaseLevel _baseLevel) {
		// Assign baseLevel reference here!
		baseLevel = _baseLevel;
		// Reset basic stuff
		SetIsPaused(false);
		timeWhenLevelEnded = -1;
		gameState = GameStates.Playing;
		// Save values!
		SaveStorage.SetInt(SaveKeys.LastLevelPlayed(MyGameName()), LevelIndex);
		// Tell people!
		levelGameUI.OnStartLevel(LevelIndex);
	}


	virtual public void LoseLevel() {
		gameState = GameStates.GameOver;
		timeWhenLevelEnded = Time.time;
		// Increment losses on this level.
		string saveKey = SaveKeys.NumLosses(MyGameName(), LevelIndex);
		int numLosses = SaveStorage.GetInt(saveKey,0);
		SaveStorage.SetInt(saveKey, numLosses + 1);
		// Tell people!
		levelGameUI.OnGameOver();
	}
	virtual protected void WinLevel() {
		FBAnalyticsController.Instance.OnWinLevel(MyGameName(), LevelIndex); // Analytics call!
		UpdateHighestLevelUnlocked(LevelIndex);
		gameState = GameStates.PostLevel;
		timeWhenLevelEnded = Time.time;
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
			OnTapScreen();
		}
	}
	override protected void RegisterButtonInput() {
		base.RegisterButtonInput();

//		bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		if (Input.GetKeyDown(KeyCode.Space)) { OnTapScreen(); }

		// DEBUG
		if (Input.GetKeyDown(KeyCode.P))			{ StartPrevLevel10(); return; } // P = Back 10 levels.
		if (Input.GetKeyDown(KeyCode.LeftBracket))	{ StartPrevLevel(); return; } // [ = Back 1 level.
		if (Input.GetKeyDown(KeyCode.RightBracket))	{ StartNextLevel(); return; } // ] = Ahead 1 level.
		if (Input.GetKeyDown(KeyCode.Backslash))	{ StartNextLevel10(); return; } // \ = Ahead 10 levels.
		if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
	}

	public void OnRetryButtonClick() {
		RestartLevel();
	}



}
