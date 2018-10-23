using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class LevelSelectController : MonoBehaviour {
    
        // events
        public void OnClick_Easy() { StartGameAtCollection(0); }
        public void OnClick_Med() { StartGameAtCollection(1); }
        public void OnClick_Hard() { StartGameAtCollection(2); }
        
        
        private void StartGameAtCollection(int collection) {
            LevelAddress collectionAdd = new LevelAddress(0,collection,0,0);
            string key = SaveKeys.SlideAndStick_LastPlayedLevelAddress(collectionAdd);
            LevelAddress lastPlayedAdd;
            if (SaveStorage.HasKey(key)) { // We've got it saved! Load 'er up.
                lastPlayedAdd = LevelAddress.FromString(SaveStorage.GetString(key));
            }
            else { // Oh, there was no save data. Use collectionAdd to start at the first level in the collection.
                lastPlayedAdd = collectionAdd;
            }
            LevelsManager.Instance.selectedAddress = lastPlayedAdd; // Setting this is optional. Just keepin' it consistent.
            
            LoadLevel(lastPlayedAdd);
        }
        
        
        // ----------------------------------------------------------------
        //  Scene Management
        // ----------------------------------------------------------------
        private void ReloadScene () { OpenScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
        private void OpenScene (string sceneName) { StartCoroutine (OpenSceneCoroutine (sceneName)); }
        private IEnumerator OpenSceneCoroutine (string sceneName) {
    //      // Show "Loading" overlay!
    //      ui.ShowLoadingOverlay ();
            yield return null;
            // Second frame: Load up that business.
            UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
        }
    
    
        public void LoadLevel (LevelAddress address) {
            LevelsManager.Instance.selectedAddress = address; // Setting this is optional. Just keepin' it consistent.
            SaveStorage.SetString (SaveKeys.SlideAndStick_LastPlayedLevelAddress(address), address.ToString()); // Actually save the value! That's what GameController pulls in.
            OpenScene (SceneNames.Gameplay(GameNames.SlideAndStick));
        }
        private void OpenTutorial() {
            LoadLevel(new LevelAddress(GameModes.TutorialIndex, 0,0, -1));
        }
    
        public void ClearAllSaveData() {
            GameManagers.Instance.DataManager.ClearAllSaveData ();
            ReloadScene ();
        }
    
    
        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update () {
            bool isKey_control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    
            // DEBUG
            if (Input.GetKeyDown(KeyCode.Return)) {
                LevelsManager.Instance.ReloadModeDatas();
                ReloadScene ();
                return;
            }
            if (isKey_control && isKey_shift && Input.GetKeyDown(KeyCode.Delete)) {
                ClearAllSaveData();
                return;
            }
        }

    }
}