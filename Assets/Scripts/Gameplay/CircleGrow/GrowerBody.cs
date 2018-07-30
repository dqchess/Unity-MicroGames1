using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    abstract public class GrowerBody : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
        // References
        private Grower myGrower;

        // Getters (Private)
        public Color color {
            get { return i_body.color; }
            set { i_body.color = value; }
        }


        // ----------------------------------------------------------------
        //  Abstract Methods
        // ----------------------------------------------------------------
        abstract public void SetRadius(float radius);


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(Grower _myGrower) {
            myGrower = _myGrower;
            // Parent jazz!
            RectTransform myRectTransform = GetComponent<RectTransform>();
            transform.SetParent(myGrower.transform);
            transform.SetAsFirstSibling(); // put me BEHIND the text.
            myRectTransform.localScale = Vector2.one;
            myRectTransform.localEulerAngles = Vector3.zero;
            // Make me fit flush within my parent!
            myRectTransform.anchorMin = Vector2.zero;
            myRectTransform.anchorMax = Vector2.one;
            myRectTransform.offsetMin = myRectTransform.offsetMax = Vector2.zero;
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        private void OnTriggerEnter2D(Collider2D collision) {
            myGrower.OnBodyTriggerEnter();
        }

    }
}