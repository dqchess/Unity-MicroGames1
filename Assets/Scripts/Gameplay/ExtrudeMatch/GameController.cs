using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ExtrudeMatch {
	public class GameController : BaseGameController {
		// Objects
		private Level currentLevel;
		// References
		[SerializeField] private Canvas canvas=null;
		[SerializeField] private GameUI ui=null;

		// Getters
		public Canvas Canvas { get { return canvas; } }
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

			// Reset some values
			DestroyCurrentLevel ();

			// Instantiate the Level from the provided LevelData!
			currentLevel = ((GameObject) Instantiate (resourcesHandler.extrudeMatch_level)).GetComponent<Level>();
			currentLevel.Initialize (this, canvas.transform);

//			// Reset camera!
//			cameraController.Reset ();
//			// Dispatch event!
//			GameManagers.Instance.EventManager.OnStartGameAtLevel (currentLevel);

			yield return null;
		}

		private void DestroyCurrentLevel () {
			if (currentLevel != null) {
				currentLevel.DestroySelf ();
				currentLevel = null;
			}
		}







	}


}



