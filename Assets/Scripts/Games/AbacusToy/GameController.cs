using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AbacusToy {
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



