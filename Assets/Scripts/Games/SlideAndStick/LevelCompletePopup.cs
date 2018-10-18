using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class LevelCompletePopup : MonoBehaviour {
        // Components
        [SerializeField] private Button b_nextLevel;
        [SerializeField] private TextMeshProUGUI t_levelName=null;
        // Properties
        public bool DidPressNextButton { get; private set; }
    
    
        // ----------------------------------------------------------------
        //  Appear / Disappear!
        // ----------------------------------------------------------------
        public void Hide() {
            this.gameObject.SetActive(false);
        }
        public void Appear() {
            this.gameObject.SetActive(true);
            
            DidPressNextButton = false; // We'll set to true when we press Next button!
    
            this.transform.localScale = new Vector3(1, 0, 1);
            LeanTween.scale (this.gameObject, Vector3.one, 0.5f).setEaseOutBack();
            this.transform.localEulerAngles = new Vector3(0, 0, -16);
            LeanTween.rotateZ(this.gameObject, 0f, 2.0f).setEaseOutElastic();
        }
    
    
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnPressNextButton() {
            DidPressNextButton = true;
        }
    }
}