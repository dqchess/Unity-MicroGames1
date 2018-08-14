using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public class LevelBounds : MonoBehaviour {
        // Components
//        [SerializeField] private BoxCollider2D colL=null;
//        [SerializeField] private BoxCollider2D colR=null;
//        [SerializeField] private BoxCollider2D colB=null;
//        [SerializeField] private BoxCollider2D colT=null;
        [SerializeField] private Image i_border=null;
		[SerializeField] private Image i_bounds=null; // THIS is exactly what my actual bounds are, mmkay
		private WallRect[] walls; // index is by Side. (See Side.cs for more info.)
        // Properties
        private Rect r_bounds;
		// References
		[SerializeField] private Level myLevel=null;


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize() {
            i_border.color = Grower.color_solid;
            r_bounds = new Rect();
			// Add walls!
			GameObject prefabGO = ResourcesHandler.Instance.circleGrow_wallRect;
			walls = new WallRect[4];
			for (int i=0; i<walls.Length; i++) {
				walls[i] = Instantiate(prefabGO).GetComponent<WallRect>();
                walls[i].Initialize(myLevel, myLevel.rt_GameComponents, new WallData());
				walls[i].name = "BoundsWall_" + i;
			}
        }

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetSize(Vector2 _size) { SetSize(_size.x, _size.y); }
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

			float wt = 100; // wallThickness. TODO: Set this individually for each wall, math-correctly!
			walls[Sides.L].SetPoses(r_bounds.xMin-wt*0.5f, r_bounds.center.y);
			walls[Sides.R].SetPoses(r_bounds.xMax+wt*0.5f, r_bounds.center.y);
			walls[Sides.B].SetPoses(r_bounds.center.x, r_bounds.yMin-wt*0.5f);
			walls[Sides.T].SetPoses(r_bounds.center.x, r_bounds.yMax+wt*0.5f);

			walls[Sides.L].SetSize(new Vector2(wt, r_bounds.height+wt));
			walls[Sides.R].SetSize(new Vector2(wt, r_bounds.height+wt));
			walls[Sides.B].SetSize(new Vector2(r_bounds.width+wt, wt));
			walls[Sides.T].SetSize(new Vector2(r_bounds.width+wt, wt));

//            float ct = 50; // colliderThickness. Doesn't matter, as long as it's greater than 1.
//            colL.size = new Vector2(ct, r_bounds.height);
//            colR.size = new Vector2(ct, r_bounds.height);
//            colB.size = new Vector2(r_bounds.width, ct);
//            colT.size = new Vector2(r_bounds.width, ct);
//
//            colL.transform.localPosition = new Vector2(r_bounds.xMin-ct*0.5f, r_bounds.center.y);
//            colR.transform.localPosition = new Vector2(r_bounds.xMax+ct*0.5f, r_bounds.center.y);
//            colB.transform.localPosition = new Vector2(r_bounds.center.x, r_bounds.yMin-ct*0.5f);
//            colT.transform.localPosition = new Vector2(r_bounds.center.x, r_bounds.yMax+ct*0.5f);
        }



    }
}