using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class SlideAndStickSfxController : SfxController {
        // References
        [SerializeField] private AudioClip ac_grabTile=null;
        [SerializeField] private AudioClip ac_releaseTile=null;
        [SerializeField] private AudioClip ac_tilesMerge=null;
        [SerializeField] private AudioClip ac_undoMove=null;
        [SerializeField] private AudioClip ac_completeLevel=null;
        [SerializeField] private AudioClip ac_levelAnimateIn=null;


        // ----------------------------------------------------------------
        //  Play Sounds
        // ----------------------------------------------------------------
        public void OnLevelAnimateIn() {
            PlaySound(ac_levelAnimateIn);
        }
        public void OnCompleteLevel() {
            PlaySound(ac_completeLevel);
        }
        public void OnGrabTile() {
            PlaySound(ac_grabTile);
        }
        public void OnReleaseTile() {
            PlaySound(ac_releaseTile);
        }
        public void OnTilesMerged() {
            float pitch = Random.Range(0.95f, 1.05f); // slight randomness to the pitch.
            PlaySound(ac_tilesMerge, 1, 0, pitch);
        }
        public void OnUndoMove() {
            PlaySound(ac_undoMove);
        }
        

    }
}