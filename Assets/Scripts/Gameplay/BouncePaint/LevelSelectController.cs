using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BouncePaint {
    public class LevelSelectController : MonoBehaviour {
        // Constants
        private readonly Vector2 tileSize = new Vector2(110,110);
        private readonly Vector2 tileGap = new Vector2(24,24);
        // Components
        [SerializeField] private TextMeshProUGUI t_headerA=null;
        [SerializeField] private TextMeshProUGUI t_headerB=null;
        [SerializeField] private RectTransform rt_scrollContent=null;
        private List<LevelTile> levelTiles;
        // Properties
        private float oscLoc;

        // Getters (Public)
        public float ScrollY { get { return rt_scrollContent.anchoredPosition.y; } }
        // Getters (Private)
        private DataManager dataManager { get { return GameManagers.Instance.DataManager; } }
        private float GetScrollStartingPos() {
            return -GetLastPlayedTilePosY();// - 100; // move it down a little bit more so the last played tile isn't the topmost one.
        }
        private float GetLastPlayedTilePosY() {
            int lastPlayedLevelIndex = SaveStorage.GetInt(SaveKeys.BouncePaint_LastLevelPlayed);
            foreach (LevelTile tile in levelTiles) {
                if (tile.LevelIndex == lastPlayedLevelIndex) {
                    return tile.transform.localPosition.y;
                }
            }
            return 0;
        }


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start () {
            oscLoc = Random.Range(0f, 100f);

            MakeLevelTiles ();

            // Add event listeners!
            GameManagers.Instance.EventManager.ScreenSizeChangedEvent += OnScreenSizeChanged;
        }
        private void OnDestroy () {
            // Remove event listeners!
            GameManagers.Instance.EventManager.ScreenSizeChangedEvent -= OnScreenSizeChanged;
        }
        private void MakeLevelTiles () {
            GameObject go_prefab = ResourcesHandler.Instance.bouncePaint_levelTile;
            int highestLevelUnlocked = SaveStorage.GetInt(SaveKeys.BouncePaint_HighestLevelUnlocked);

            levelTiles = new List<LevelTile>();
            for (int levelIndex=Level.FirstLevelIndex; levelIndex<=Level.LastLevelIndex; levelIndex++) {
                LevelTile newTile = Instantiate(go_prefab).GetComponent<LevelTile>();
                bool isLocked = levelIndex > highestLevelUnlocked;
                if (levelIndex==Level.FirstLevelIndex) { isLocked = false; } // Force the first level to be unlocked, of course.
                newTile.Initialize (this, rt_scrollContent, levelIndex, isLocked);
                levelTiles.Add (newTile);
            }

            PositionLevelTiles ();
        }

        private void PositionLevelTiles () {
            float containerWidth = 600;//HACKy hardcoded. rt_scrollContent.rect.width; // let's use this value to determine how much horz. space I've got for the tiles.

            float tempX = tileGap.x; // where we're putting things! Added to as we go along.
            float tempY = -tileGap.y; // where we're putting things! Added to as we go along.

            for (int i=0; i<levelTiles.Count; i++) {
                LevelTile tile = levelTiles[i];

                tile.SetPosSize (new Vector2(tempX,tempY), tileSize);
                tempX += tileSize.x+tileGap.x;
                if (tempX+tileSize.x > containerWidth) { // wrap to the next row
                    tempX = tileGap.x;
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
        private void Update () {
            UpdateHeaderColors();

            AcceptButtonInput();
        }
        private void UpdateHeaderColors() {
            float h;
            float s = 1f;
            float b = 0.92f;

            oscLoc += Time.deltaTime;

            h = (oscLoc*0.07f) % 1f;
            t_headerA.color = new ColorHSB(h,s,b).ToColor();
            h = (oscLoc*0.03f+0.3f) % 1f;
            t_headerB.color = new ColorHSB(h,s,b).ToColor();
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
            foreach (LevelTile tile in levelTiles) {
                tile.Debug_UnlockMe ();
            }
        }


    }
}