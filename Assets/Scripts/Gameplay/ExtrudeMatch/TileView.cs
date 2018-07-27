using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	public class TileView : BoardOccupantView {

		// Getters
		public static Color GetBodyColor(int _colorID) {
			switch (_colorID) {
			case 0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
			case 1: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
			case 2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
			case 3: return new ColorHSB(190/255f,150/255f,245/255f).ToColor();
			case 4: return new ColorHSB(220/255f,150/255f,245/255f).ToColor();
			default: return Color.red; // Oops! Too many colors.
			}
		}

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (BoardView _myBoardView, Tile _myObj) {
			base.InitializeAsBoardOccupantView (_myBoardView, _myObj);

			// Set size!
			myRectTransform.sizeDelta = new Vector2(_myBoardView.UnitSize*0.92f, _myBoardView.UnitSize*0.92f);

			// Color me impressed!
			i_body.color = GetBodyColor(_myObj.ColorID);
		}


        // ----------------------------------------------------------------
        //  Animating
        // ----------------------------------------------------------------
        public void AnimateIn(Tile sourceTile) {
            StartCoroutine(Coroutine_AnimateIn(sourceTile));
        }
        private IEnumerator Coroutine_AnimateIn(Tile sourceTile) {
            Vector2 targetPos = Pos;
            float targetScale = 1;

            Scale = 0.2f; // shrink me down

            // If I came from a source, then start me there and animate me to my actual position!
            if (sourceTile != null) {
                Vector2 sourcePos = GetPosFromBO(sourceTile);
                Pos = sourcePos;
            }
            // Animate!
            float easing = 0.4f; // higher is faster.
            while (Pos!=targetPos || Scale!=targetScale) {
                Pos += new Vector2((targetPos.x-Pos.x)*easing, (targetPos.y-Pos.y)*easing);
                Scale += (targetScale-Scale) * easing;
                if (Vector2.Distance(Pos,targetPos)<0.5f) { Pos = targetPos; } // Almost there? Get it!
                if (Mathf.Approximately(Scale,targetScale)) { Scale = targetScale; } // Almost there? Get it!
                yield return null;
            }
        }
	}
}