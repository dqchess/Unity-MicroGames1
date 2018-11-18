using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    public class LevelDebugUI : MonoBehaviour {
        // Components
        [SerializeField] private GameObject go_customBoardGenUI=null;
        // References
        [SerializeField] private Level level=null;
        
        
        
        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
        private void Start () {
            go_customBoardGenUI.SetActive(level.Board.DidRandGen); // only show rand-gen UI for rand-gen boards.
        }
        
        
        
        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            // Debug!
            // D = Toggle RandLayoutHelper visible!
            if (Input.GetKeyDown(KeyCode.D)) {
                go_customBoardGenUI.SetActive(!go_customBoardGenUI.activeSelf);
            }
        }
        
    
    }
}