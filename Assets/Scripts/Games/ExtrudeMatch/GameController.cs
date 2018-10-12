using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ExtrudeMatch {
	public class GameController : BaseGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.ExtrudeMatch; }
        // Properties
        private int score;
		// Objects
		private BoardController boardController;
		// References
		[SerializeField] private GameUI ui=null;

		// Getters
//		public Level CurrentLevel { get { return currentLevel; } }


		// ----------------------------------------------------------------
		//  Start / Destroy
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

			// Let's a-play!
			ResetLevel();
		}


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void SetScore(int _score) {
            score = _score;
            // Update best score!
            int bestScore = SaveStorage.GetInt(SaveKeys.ExtrudeMatch_BestScore, 0);
            if (score >= bestScore) {
                SaveStorage.SetInt(SaveKeys.ExtrudeMatch_BestScore, score);
            }
            // Update UI!
            ui.UpdateScoreTexts(score);
        }
        public void AddToScore(int _value) {
            SetScore(score + _value);
        }


        // ----------------------------------------------------------------
        //  Game Events
        // ----------------------------------------------------------------
        public void GameOver() {
            ui.OnGameOver();
        }



		// ----------------------------------------------------------------
		//  Doers - Loading Level
		// ----------------------------------------------------------------
		public void ResetLevel() {
			StartCoroutine (Coroutine_ResetLevel ());
		}
		/** This actually shows "Loading" overlay FIRST, THEN next frame loads the world. */
		private IEnumerator Coroutine_ResetLevel () {
			//		// Show "Loading" overlay!
			//		ui.ShowLoadingOverlay (); Actually don't. It flashes too quickly. Kinda jarring.
			yield return null;

            // Destroy the previous level.
            DestroyCurrentLevel ();

            // Reset some values
            SetScore(0);

			// Instantiate the Level from the provided LevelData!
			boardController = ((GameObject) Instantiate (resourcesHandler.extrudeMatch_boardController)).GetComponent<BoardController>();
			boardController.Initialize (this, canvas.transform);

			// Tell the people!
            ui.OnStartLevel();
//			// Dispatch event!
//			GameManagers.Instance.EventManager.OnStartGameAtLevel (currentLevel);

			yield return null;
		}

		private void DestroyCurrentLevel () {
			if (boardController != null) {
				boardController.DestroySelf ();
				boardController = null;
			}
		}







	}


}



