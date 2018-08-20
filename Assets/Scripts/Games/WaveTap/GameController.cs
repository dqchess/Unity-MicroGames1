using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	//public enum GameStates { Playing, Lost, Won }
	public enum LoseReasons { Undefined, TapEarly, MissedTap }

	public class GameController : BaseLevelGameController {
		// Overrideables
		override public string MyGameName() { return GameNames.WaveTap; }
		// Components
		private Level level; // MY game-specific Level class.
		// Properties
		private LoseReasons loseReason;



		// ----------------------------------------------------------------
		//  Game Flow Events
		// ----------------------------------------------------------------
		override public void LoseLevel() {
			base.LoseLevel();
			// Tell people!
			level.OnLoseLevel(loseReason);
		}
		override protected void WinLevel() {
			base.WinLevel();
			StartCoroutine(Coroutine_StartNextLevel());
			// Tell people!
			level.OnWinLevel();
		}

		private IEnumerator Coroutine_StartNextLevel() {
			yield return new WaitForSecondsRealtime(1.2f);
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

			// Reset basics.
			loseReason = LoseReasons.Undefined;

			// Make the new level!
			InitializeLevel(Instantiate(resourcesHandler.waveTap_level), _levelIndex);

			// DO animate!
			if (doAnimate) {
				float duration = 1f;

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



		// ----------------------------------------------------------------
		//  Input
		// ----------------------------------------------------------------
		override protected void OnTapScreen() {
			if (Time.timeScale == 0f) { return; } // Paused? Ignore input.
			if (!IsGameStatePlaying) { return; } // Not playing? Ignore input.
            if (level.Player.IsFrozen) { return; } // Player's frozen? Okay, ignore input.

			if (level.IsPlayerTouchingABar()) {
				level.PlayerKnockBarsTouching();
			}
			else {
				OnTapEarly();
			}
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnKnockLastBar() {
			WinLevel();
		}
		private void OnTapEarly() {
			loseReason = LoseReasons.TapEarly;
			LoseLevel();
		}
		public void OnMissedNextBar() {
			loseReason = LoseReasons.MissedTap;
			LoseLevel();
		}



		// ----------------------------------------------------------------
		//  Debug
		// ----------------------------------------------------------------
		override protected void Debug_WinLevel() {
			WinLevel();
		}









	}
}

