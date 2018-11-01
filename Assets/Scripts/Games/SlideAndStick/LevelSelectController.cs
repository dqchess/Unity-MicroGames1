using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SlideAndStick {
    public class LevelSelectController : MonoBehaviour {
        // Constants
        private static int CollIndex_E = 2; // NOTE: We will eventually improve this! It's sloppy as is but functioning and untenable.
        private static int CollIndex_M = 3;
        private static int CollIndex_H = 4;
        // Components
        [SerializeField] private TextMeshProUGUI t_deleteLayouts=null;
        [SerializeField] private TextMeshProUGUI t_progressEasy=null;
        [SerializeField] private TextMeshProUGUI t_progressMed=null;
        [SerializeField] private TextMeshProUGUI t_progressHard=null;
    
        // Getters (Private)
        private LevelAddress GetLastPlayedAddress(int collection) {
            LevelAddress collectionAdd = new LevelAddress(0,collection,0,0);
            string key = SaveKeys.SlideAndStick_LastPlayedLevelAddress(collectionAdd);
            if (SaveStorage.HasKey(key)) { // We've got it saved! Load 'er up.
                return LevelAddress.FromString(SaveStorage.GetString(key));
            }
            else { // Oh, there was no save data. Use collectionAdd to start at the first level in the collection.
                return collectionAdd;
            }
        }
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            // Update progress texts!
            LevelsManager lm = LevelsManager.Instance;
            t_progressEasy.text = lm.GetNumLevelsCompleted(CollIndex_E) + "/" + lm.GetNumLevelsPlayable(CollIndex_E);
            t_progressMed.text  = lm.GetNumLevelsCompleted(CollIndex_M) + "/" + lm.GetNumLevelsPlayable(CollIndex_M);
            t_progressHard.text = lm.GetNumLevelsCompleted(CollIndex_H) + "/" + lm.GetNumLevelsPlayable(CollIndex_H);
            UpdateDeleteLayoutsText();
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClickCollectionButton(int collectionIndex) { StartGameAtCollection(collectionIndex); }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void UpdateDeleteLayoutsText() {
            string savedLayoutsString = SaveStorage.GetString(SaveKeys.SlideAndStick_Debug_SavedLayouts);
            int numLayouts = RandLayoutHelperUI.GetNumLayouts(savedLayoutsString);
            t_deleteLayouts.text = "delete " + numLayouts + " saved layouts";
        }
        public void CopySavedLayoutsToClipboard() {
            string savedLayoutsString = SaveStorage.GetString(SaveKeys.SlideAndStick_Debug_SavedLayouts);
            GameUtils.CopyToClipboard(savedLayoutsString);
        }
        private void StartGameAtCollection(int collection) {
            LevelAddress lastPlayedAddress = GetLastPlayedAddress(collection);
            LevelsManager.Instance.selectedAddress = lastPlayedAddress; // Setting this is optional. Just keepin' it consistent.
            
            LoadLevel(lastPlayedAddress);
        }
        public void DeleteSavedLayouts() {
            SaveStorage.DeleteKey(SaveKeys.SlideAndStick_Debug_SavedLayouts);
            ReloadScene ();
        }
        public void ClearAllSaveData() {
            GameManagers.Instance.DataManager.ClearAllSaveData ();
            LevelsManager.Instance.ReloadModeDatas();
            ReloadScene ();
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