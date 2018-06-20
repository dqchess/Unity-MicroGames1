using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericGameUI : MonoBehaviour {

	// ----------------------------------------------------------------
	//  Doers - Loading Scenes
	// ----------------------------------------------------------------
	private void OpenScene (string sceneName) {
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
	}

	public void ReloadScene () { OpenScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
	public void OpenScene_GameSelect() { OpenScene(SceneNames.GameSelect); }

}
