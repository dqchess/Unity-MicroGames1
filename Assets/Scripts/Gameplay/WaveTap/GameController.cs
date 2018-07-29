using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public enum GameStates { Playing, Lost, Won }
	public enum LoseReasons { TapEarly, MissedTap }

	public class GameController : BaseGameController {
		// Components
		[SerializeField] private Bar bar=null;
		[SerializeField] private Player player=null;
		// Properties
		private GameStates gameState;
		private int currentLevel;
//		private Rect r_levelBounds;
		// References
//		[SerializeField] private Canvas canvas=null;
		[SerializeField] private GameUI ui=null;

		// Getters (Public)
		public bool IsLevelWon { get { return gameState == GameStates.Won; } }
		// Getters (Private)
		private bool IsPlayerTouchingBar() {
			return Mathf.Abs(player.PosY-bar.PosY) <= player.Radius;
		}


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

//			r_levelBounds = i_levelBounds.rectTransform.rect;

//			SetCurrentLevel(SaveStorage.GetInt(SaveKeys.WaveTap_LastLevelPlayed, 1));TODO: Convert this to level-based system
		}



		// ----------------------------------------------------------------
		//  Game Flow
		// ----------------------------------------------------------------
		private void RestartLevel() { SetCurrentLevel(currentLevel); }
		public void StartPrevLevel() { SetCurrentLevel(Mathf.Max(1, currentLevel-1)); }
		public void StartNextLevel() { SetCurrentLevel(currentLevel+1); }
		private void SetCurrentLevel(int _currentLevel) {
			currentLevel = _currentLevel;

			// Set basics!
			SetIsPaused(false);
			gameState = GameStates.Playing;

			bar.Reset(currentLevel);
			player.Reset(currentLevel);

			// Tell the people!
			ui.OnStartLevel(currentLevel);
		}

		private void LoseLevel(LoseReasons reason) {
			gameState = GameStates.Lost;
			// Tell people!
			ui.OnGameOver();
			// Increment losses on this level.
//			string saveKey = SaveKeys.WaveTap_NumLosses(currentLevel);TODO: Convert this to level-based system
//			int numLosses = SaveStorage.GetInt(saveKey,0);
//			SaveStorage.SetInt(saveKey, numLosses + 1);
		}

		private void WinLevel() {
//			FBAnalyticsController.Instance.WaveTap_OnWinLevel(LevelIndex); // Analytics call!TODO: Convert this to level-based system
			UpdateHighestLevelUnlocked(currentLevel);
			gameState = GameStates.Won;
			//			// Tell people!
			//			level.OnWinLevel(WinningPlayer);
			//			sfxController.OnCompleteLevel();
			Invoke("StartNextLevel", 0.6f);
		}
		private void UpdateHighestLevelUnlocked(int _levelIndex) {
//			int highestRecord = SaveStorage.GetInt(SaveKeys.BouncePaint_HighestLevelUnlocked);TODO: Convert this to level-based system
//			if (_levelIndex > highestRecord) {
//				SaveStorage.SetInt(SaveKeys.BouncePaint_HighestLevelUnlocked, _levelIndex);
//			}
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

			if (gameState == GameStates.Playing) {
				OnPressHitButton();
			}
		}
		override protected void RegisterButtonInput() {
			base.RegisterButtonInput();

			if (Input.GetKeyDown(KeyCode.Space)) { OnTapScreen(); }

			// DEBUG
			if (Input.GetKeyDown(KeyCode.LeftBracket)) { StartPrevLevel(); }
			if (Input.GetKeyDown(KeyCode.RightBracket)) { StartNextLevel(); }
//			if (Input.GetKeyDown(KeyCode.W)) { Debug_WinLevel(); }
		}


		private void OnPressHitButton() {
			// Ignore hits if...
			if (gameState != GameStates.Playing) { return; }
//			if (level!=null && level.IsAnimatingIn) { return; }

			if (IsPlayerTouchingBar()) {
				OnPlayerHitBar();
			}
			else {
				player.Die(LoseReasons.TapEarly);
			}
		}




		private void OnPlayerHitBar() {
			bar.HitMe();
			player.OnHitBar();
			if (bar.NumHitsLeft <= 0) {
				WinLevel();
			}
		}
		public void OnPlayerDie(LoseReasons reason) {
			if (gameState == GameStates.Playing) {
				LoseLevel(reason);
			}
		}







	}
}

