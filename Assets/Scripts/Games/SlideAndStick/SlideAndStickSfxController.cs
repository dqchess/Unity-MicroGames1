using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class SlideAndStickSfxController : SfxController {
        // References
        [SerializeField] private AudioClip ac_grabTile=null;
        [SerializeField] private AudioClip ac_releaseTile=null;
        [SerializeField] private AudioClip ac_tilesMerge=null;
        [SerializeField] private AudioClip ac_completeLevel=null;


        // ----------------------------------------------------------------
        //  Game Events
        // ----------------------------------------------------------------
        public void OnGrabTile() {
            PlaySound(ac_grabTile);
        }
        public void OnReleaseTile() {
            PlaySound(ac_releaseTile);
        }
        public void OnTilesMerged() {
            PlaySound(ac_tilesMerge);
        }
        public void OnCompleteLevel() {
            PlaySound(ac_completeLevel);
        }

    }
}