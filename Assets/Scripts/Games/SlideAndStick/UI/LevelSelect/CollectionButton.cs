using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    public class CollectionButton : MonoBehaviour {
        // Properties
        [SerializeField] private int collectionIndex;
        // References
        [SerializeField] private LevSelController levSelController;
        
        
        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnClick() {
            levSelController.OnCollectionButtonClick(collectionIndex);
        }
    }
}
