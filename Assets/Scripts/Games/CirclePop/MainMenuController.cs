using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CirclePop {
//	public class Game {
//		static public string Name = GameNames.CirclePop;
//	}

	public class MainMenuController : MonoBehaviour {


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		protected void OpenScene (string sceneName) {
			UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
		}
		public void OpenScene_LevelSelect() {
//			OpenScene(SceneNames.LevelSelect(Game.Name));
			OpenScene(SceneNames.LevelSelect(GameNames.CirclePop));
		}



	}
}