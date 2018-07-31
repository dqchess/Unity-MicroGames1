using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class LevelBounds : MonoBehaviour {
        // Components
        [SerializeField] private BoxCollider2D colL=null;
        [SerializeField] private BoxCollider2D colR=null;
        [SerializeField] private BoxCollider2D colB=null;
        [SerializeField] private BoxCollider2D colT=null;
        [SerializeField] private Image i_border=null;
        [SerializeField] private Image i_bounds=null; // THIS is exactly what my actual bounds are, mmkay
        // Properties
        private Rect r_bounds;


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
            i_border.color = Grower.color_solid;
            r_bounds = new Rect();
        }

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        /// Note that this function AUTOMATICALLY updates our position to keep us centered! (We can THEN set our offset, in case we want the level off-centered.)
        public void SetSize(float w,float h) {
            r_bounds.size = new Vector2(w,h);
            // Keep me centered, eh?
            r_bounds.center = new Vector2(0,0);
            UpdateComponents();
        }
        private void UpdateComponents() {
            i_bounds.rectTransform.sizeDelta = r_bounds.size;
            i_bounds.rectTransform.anchoredPosition = r_bounds.center;

            float ct = 50; // colliderThickness. Doesn't matter, as long as it's greater than 1.
            colL.size = new Vector2(ct, r_bounds.height);
            colR.size = new Vector2(ct, r_bounds.height);
            colB.size = new Vector2(r_bounds.width, ct);
            colT.size = new Vector2(r_bounds.width, ct);

            colL.transform.localPosition = new Vector2(r_bounds.xMin-ct*0.5f, r_bounds.center.y);
            colR.transform.localPosition = new Vector2(r_bounds.xMax+ct*0.5f, r_bounds.center.y);
            colB.transform.localPosition = new Vector2(r_bounds.center.x, r_bounds.yMin-ct*0.5f);
            colT.transform.localPosition = new Vector2(r_bounds.center.x, r_bounds.yMax+ct*0.5f);
        }



    }
}