using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
    public class TileMergeBurst : MonoBehaviour {
        // Components
        [SerializeField] private Transform tf_shadows=null;
        [SerializeField] private Transform tf_tops=null;

        public void Initialize(BoardView boardView, MergeSpot mergeSpot) {
            // Parent jazz.
            GameUtils.ParentAndReset(this.gameObject, boardView.transform);
            // Position/rotate/scale.
            Vector3 rotation = new Vector3(0, 0, MathUtils.DirToRotation(mergeSpot.dir));
            this.transform.localScale = Vector3.one * boardView.UnitSize/100f; // Hardcoded.
            tf_shadows.localEulerAngles = rotation;
            tf_tops.localEulerAngles = rotation;
            Vector2 boardPos = mergeSpot.pos + mergeSpot.dir*0.5f; // the exact center of the merge, flat on the Board.
            boardPos += new Vector2(0, -0.08f); // push up a bit to align with Tiles' thiccness
            this.transform.localPosition = boardView.BoardToLocal(boardPos);
            
            // Tint all my dashes right!
            Color tileColor = TileViewBody.GetBodyColor(mergeSpot.colorID);
            ColorHSB topColorHSB = new ColorHSB(tileColor) {
                s = 1,
                b = 1
            };
            Color topColor = topColorHSB.ToColor();
            Color shadowColor = Color.Lerp(tileColor,Color.black, 0.3f);
            foreach (Image image in tf_shadows.GetComponentsInChildren<Image>()) {
                image.color = shadowColor;
            }
            foreach (Image image in tf_tops.GetComponentsInChildren<Image>()) {
                image.color = topColor;
            }
            
            // Destroy me in a little bit!
            Invoke("DestroySelf", 3f);
        }

        // Doers
        private void DestroySelf() {
            Destroy(this.gameObject);
        }
        
    }
}
