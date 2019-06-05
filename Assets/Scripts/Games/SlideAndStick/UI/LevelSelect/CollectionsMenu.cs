using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {

    public class CollectionsMenu : BaseLevSelMenu {
        // Components
        [SerializeField] private GameObject go_collButtonsEasies=null;
        [SerializeField] private GameObject go_collButtonsNoEasies=null;
        
        private void Awake() {
            // Destroy one of the coll-buttons GOs, for AB test!
            if (ABTestsManager.Instance.IsEasies) { Destroy(go_collButtonsNoEasies); }
            else { Destroy(go_collButtonsEasies); }
        }
    }
    
}