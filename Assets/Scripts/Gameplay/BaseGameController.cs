using UnityEngine;
using System.Collections;
using System.Collections.Generic;


abstract public class BaseGameController : MonoBehaviour {
	// Properties
	private bool isPaused = false;
	// References
//	[SerializeField] private GameplayUI ui=null;
//	[SerializeField] private Transform tf_world=null;

	// Getters / Setters
	protected DataManager dataManager { get { return GameManagers.Instance.DataManager; } }
	protected InputController inputController { get { return InputController.Instance; } }
	protected ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }



	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	virtual protected void Start () {
		// Set application values
		Application.targetFrameRate = GameVisualProperties.TARGET_FRAME_RATE;

		UpdateTimeScale();

		// Add event listeners!
//		GameManagers.Instance.EventManager.UserSaidTargetPuzzleWordEvent += OnUserSaidTargetPuzzleWord;
	}
	virtual protected void OnDestroy() {
		// Remove event listeners!
//		GameManagers.Instance.EventManager.UserSaidTargetPuzzleWordEvent -= OnUserSaidTargetPuzzleWord;
	}




	// ----------------------------------------------------------------
	//  Doers - Loading Level
	// ----------------------------------------------------------------
	public void ReloadScene () { OpenScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
	protected void OpenScene (string sceneName) {
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
	}

	public void QuitToGameSelect() {
		OpenScene(SceneNames.GameSelect);
	}




	// ----------------------------------------------------------------
	//  Doers - Gameplay
	// ----------------------------------------------------------------
	protected void SetIsPaused(bool _isPaused) {
		isPaused = _isPaused;
		UpdateTimeScale ();
	}
	protected void TogglePause () {
		SetIsPaused(!isPaused);
	}
	private void UpdateTimeScale () {
		if (isPaused) { Time.timeScale = 0; }
		else { Time.timeScale = 1; }
	}



	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	virtual protected void Update () {
		RegisterButtonInput ();
//		RegisterMouseInput();
	}
	private void RegisterButtonInput () {
		bool isKey_alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		bool isKey_control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);


		// P = Toggle pause
		if (Input.GetKeyDown (KeyCode.P)) {
			TogglePause ();
		}


		// ~~~~ DEBUG ~~~~
		// ENTER = Reload scene!
		if (Input.GetKeyDown(KeyCode.Return)) {
			ReloadScene();
			return;
		}
		// ALT + ___
		if (isKey_alt) {
			
		}
		// CONTROL + ___
		if (isKey_control) {
			
		}

	}




}






