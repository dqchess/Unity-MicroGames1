using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlideAndStick {
	public class GameController : BaseGameController {
		// Objects
		private BoardController boardController;
		// References
		[SerializeField] private Canvas canvas=null;
		[SerializeField] private GameUI ui=null;

		// Getters
		public Canvas Canvas { get { return canvas; } }


		// ----------------------------------------------------------------
		//  Start / Destroy
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

			// Let's a-play!
			ResetLevel();
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
            // Destroy the previous level.
            DestroyCurrentLevel ();

			// Instantiate the Level from the provided LevelData!
			boardController = Instantiate(resourcesHandler.slideAndStick_boardController).GetComponent<BoardController>();
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



