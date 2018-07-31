using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    abstract public class Wall : MonoBehaviour {
        // Components
        [SerializeField] protected Image i_body;

        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        virtual public void Initialize(Transform tf_parent, Vector2 center, Vector2 size) {
            // Parent jazz!
            this.transform.SetParent(tf_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;

            // Size/position me!
            RectTransform myRectTransform = this.GetComponent<RectTransform>();
            myRectTransform.sizeDelta = size;
            myRectTransform.anchoredPosition = center;

            // Color me impressed!
            i_body.color = Grower.color_solid;
        }

    }
}
