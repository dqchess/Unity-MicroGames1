using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectController : MonoBehaviour {


	// ----------------------------------------------------------------
	//  Doers - Loading Scenes
	// ----------------------------------------------------------------
	private void OpenScene (string sceneName) {
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
	}

	public void OpenScene_AlphaTapMatch() { OpenScene(SceneNames.AlphaTapMatch); }
	public void OpenScene_AlphaTapMost() { OpenScene(SceneNames.AlphaTapMost); }
    public void OpenScene_AlphaTapOrder() { OpenScene(SceneNames.AlphaTapOrder); }

	public void OpenScene_BouncePaint() { OpenScene(SceneNames.MainMenu(GameNames.BouncePaint)); }

	public void OpenScene_CircleGrow() { OpenScene(SceneNames.MainMenu(GameNames.CircleGrow)); }

    public void OpenScene_ExtrudeMatch() { OpenScene(SceneNames.ExtrudeMatch); }

    public void OpenScene_LetterClear() { OpenScene(SceneNames.LetterClear); }

    public void OpenScene_WaveTap() { OpenScene(SceneNames.WaveTap); }


}
