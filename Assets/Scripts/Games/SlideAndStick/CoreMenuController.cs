using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class CoreMenuController : MonoBehaviour {
        // References
        [SerializeField] private GameController gameController=null;
        [SerializeField] private LevSelController levSelController=null;
        
        // Getters (Private)
        private LevelsManager lm { get { return LevelsManager.Instance; } }
        // Getters (Public)
        public bool IsGameControllerBlockedByLevSel() {
            // Only accept input if our LevSel is 100% closed.
            return levSelController.OpenLoc == 0;
        }
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start() {
            bool didBeatTutorial = SaveStorage.GetInt(SaveKeys.SlideAndStick_DidCompleteTutorial) == 1;
            
            // Didn't beat tutorial? Go straight into it!
            if (!didBeatTutorial) {
                CloseLevSelController(false);
                //OpenLevel(
            }
            // We DID beat the tutorial. Open LevSel menu!
            else {
                OpenLevSelController(false);
            }
        }
        
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void OpenLevSelController(bool doAnimate) {
            levSelController.Open(doAnimate);
        }
        public void CloseLevSelController(bool doAnimate) {
            //gameController.Open();
            levSelController.Close(doAnimate);
        }
        
        public void OpenLevel(LevelAddress address) {
            gameController.SetCurrentLevel(address, false);
            levSelController.Close(true);
        }
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnToggleLevSelButtonClick() {
            bool isOpen = levSelController.OpenLoc > 0.6f;
            if (isOpen) {
                CloseLevSelController(true);
            }
            else {
                OpenLevSelController(true);
            }
        }
        
    
        // ----------------------------------------------------------------
        //  Scene Management
        // ----------------------------------------------------------------
        private void ReloadScene () { OpenScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
        private void OpenScene (string sceneName) {
            UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
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
                lm.Reset();
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