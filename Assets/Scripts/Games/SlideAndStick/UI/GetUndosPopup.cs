using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
    public class GetUndosPopup : MonoBehaviour {
        // Components
        [SerializeField] private Animator myAnimator=null;
        [SerializeField] private TextMeshProUGUI t_numUndosToGet=null;
        // Properties
        public bool IsOpen { get; private set; }


        //// ----------------------------------------------------------------
        ////  Awake / Destroy
        //// ----------------------------------------------------------------
        //private void Awake() {
        //    // Add event listeners!
        //    IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        //}
        //private void OnDestroy() {
        //    // Remove event listeners!
        //    IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        //}


        // ----------------------------------------------------------------
        //  Open / Close!
        // ----------------------------------------------------------------
        public void Close() {
            IsOpen = false;
            myAnimator.Play("GetUndosPopupClose");
        }
        public void Open() {
            IsOpen = true;
            t_numUndosToGet.text = GameController.NumUndosFromRewardVideo.ToString();
            myAnimator.Play("GetUndosPopupOpen");
            
            // HACKY release the Undo button, so we're not spamming it.
            UndoMoveButton undoMoveButton = FindObjectOfType<UndoMoveButton>();
            if (undoMoveButton != null) {
                undoMoveButton.OnTouchUp();
            }
            else { Debug.LogError("Whoa! Can't find an UndoMoveButton."); }
        }
    
    
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick_WatchAd() {
            AdManager.instance.showRewardVideo();
        }
        public void OnClick_Close() {
            if (IsOpen) { // Don't register close clicks if I'm not open.
                Close();
            }
        }
        
        
        
    }
}

