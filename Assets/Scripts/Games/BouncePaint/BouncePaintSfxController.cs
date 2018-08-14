using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BouncePaint {
    public class BouncePaintSfxController : SfxController {
        // References
        [SerializeField] private AudioClip ac_bounce=null;
        [SerializeField] private AudioClip ac_playerDie=null;
        [SerializeField] private AudioClip ac_completeLevel=null;


        // ----------------------------------------------------------------
        //  Game Events
        // ----------------------------------------------------------------
        public void OnPlayerBounceOnBlock() {
            PlaySound(ac_bounce);
        }
        public void OnSetGameOver() {
            PlaySound(ac_playerDie);
        }
        public void OnCompleteLevel() {
            PlaySound(ac_completeLevel);
        }

    }
}