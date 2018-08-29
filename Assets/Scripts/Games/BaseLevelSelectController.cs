using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseLevelSelectController : MonoBehaviour {
	// Overrideables
	abstract public    string MyGameName();
	abstract protected GameObject LevelTilePrefab();
	abstract protected int FirstLevelIndex();
	abstract protected int LastLevelIndex();
	protected Vector2 tileSize = new Vector2(50,50); // to override, set these values in Start() before calling base.Start().
	protected Vector2 tileGap = new Vector2(20,20);
	// Components
	[SerializeField] private RectTransform rt_scrollContent=null;
	private List<BaseLevelTile> levelTiles;

	// Getters (Public)
	public float ScrollY { get { return rt_scrollContent.anchoredPosition.y; } }
	// Getters (Private)
	private DataManager dataManager { get { return GameManagers.Instance.DataManager; } }
	private float GetScrollStartingPos() {
		return -GetLastPlayedTilePosY();// - 100; // move it down a little bit more so the last played tile isn't the topmost one.
	}
	private float GetLastPlayedTilePosY() {
		int lastPlayedLevelIndex = SaveStorage.GetInt(SaveKeys.LastLevelPlayed(MyGameName()));
		foreach (BaseLevelTile tile in levelTiles) {
			if (tile.LevelIndex == lastPlayedLevelIndex) {
				return tile.transform.localPosition.y;
			}
		}
		return 0;
	}



	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	virtual protected void Start () {
		MakeLevelTiles ();

		// Add event listeners!
		GameManagers.Instance.EventManager.ScreenSizeChangedEvent += OnScreenSizeChanged;
	}
	private void OnDestroy () {
		// Remove event listeners!
		GameManagers.Instance.EventManager.ScreenSizeChangedEvent -= OnScreenSizeChanged;
	}
	private void MakeLevelTiles () {
		GameObject go_prefab = LevelTilePrefab();
		int highestLevelUnlocked = SaveStorage.GetInt(SaveKeys.HighestLevelUnlocked(MyGameName()));

		levelTiles = new List<BaseLevelTile>();
		for (int levelIndex=FirstLevelIndex(); levelIndex<=LastLevelIndex(); levelIndex++) {
			BaseLevelTile newTile = Instantiate(go_prefab).GetComponent<BaseLevelTile>();
			bool isLocked = levelIndex > highestLevelUnlocked+1;
			if (levelIndex==FirstLevelIndex()) { isLocked = false; } // Force the first level to be unlocked, of course.
			newTile.Initialize (this, rt_scrollContent, levelIndex, isLocked);
			levelTiles.Add (newTile);
		}

		PositionLevelTiles ();
	}

	private void PositionLevelTiles () {
		float containerWidth = 600;//HACKy hardcoded. rt_scrollContent.rect.width; // let's use this value to determine how much horz. space I've got for the tiles.

		int numCols = (int)(containerWidth / (tileSize.x+tileGap.x));
		float xOffset = (containerWidth-(tileSize.x+tileGap.x)*numCols) * 0.5f;
		float tempX = xOffset; // where we're putting things! Added to as we go along.
		float tempY = -tileGap.y; // where we're putting things! Added to as we go along.

		for (int i=0; i<levelTiles.Count; i++) {
			BaseLevelTile tile = levelTiles[i];

			tile.SetPosSize (new Vector2(tempX,tempY), tileSize);
			tempX += tileSize.x+tileGap.x;
			if (tempX+tileSize.x > containerWidth) { // wrap to the next row
				tempX = xOffset;
				tempY -= tileSize.y+tileGap.y;
			}
		}

		// Set scroll layer's height, now that we know the bottommost element!
		tempY -= 300; // Give us some extra scroll room.
		rt_scrollContent.sizeDelta = new Vector2(rt_scrollContent.sizeDelta.x, -tempY);
		// Reset the scroll layer pos.
		rt_scrollContent.anchoredPosition = new Vector2(rt_scrollContent.anchoredPosition.x, GetScrollStartingPos());
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void OpenScene (string sceneName) { UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName); }
	private void ReloadScene () { OpenScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
	//public void LoadLevel (int worldIndex, int levelIndex) {
	//    SaveStorage.SetInt (SaveKeys.LAST_PLAYED_LEVEL_INDEX, levelIndex);
	//    UnityEngine.SceneManagement.SceneManager.LoadScene (SceneNames.Gameplay);
	//}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnScreenSizeChanged () {
		PositionLevelTiles ();
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	virtual protected void Update() {
		AcceptButtonInput();
	}
	private void AcceptButtonInput() {
		//bool isKey_control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		//bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		// DEBUG
		if (Input.GetKeyDown(KeyCode.U)) {
			Debug_UnlockAllLevelTiles ();
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
			ReloadScene ();
			return;
		}
		//if (isKey_control && isKey_shift && Input.GetKeyDown(KeyCode.Delete)) {
		//    ClearAllSaveDataAndReloadScene ();
		//    return;
		//}
	}
	public void Debug_UnlockAllLevelTiles () {
		foreach (BaseLevelTile tile in levelTiles) {
			tile.Debug_UnlockMe ();
		}
	}



}
