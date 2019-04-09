using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class TileMergeBurst : MonoBehaviour {
    
        // Doers
        public void DestroySelf() {
            Destroy(this.gameObject);
        }
        
    }
}
