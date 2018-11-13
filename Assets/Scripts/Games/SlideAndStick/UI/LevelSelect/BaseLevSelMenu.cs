using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class BaseLevSelMenu : MonoBehaviour {
        // Components
        [SerializeField] private CanvasGroup myCanvasGroup=null;
    
        // Getters (Protected)
        protected LevelsManager lm { get { return LevelsManager.Instance; } }
        protected LevelAddress selectedAddress { get { return lm.selectedAddress; } }
        
        
        
    }
}