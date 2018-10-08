using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class TileView : BoardOccupantView {
		// Components
//		[SerializeField] private Image i_highlight;
		private List<Image> bodyImages;
		// References
		private Tile myTile;
		// Properties
		private Color bodyColor;

		// Getters
		public static Color GetBodyColor(int _colorID) {
			switch (_colorID) {
			case 0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
			case 1: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
			case 2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
            case 3: return new ColorHSB(180/255f,250/255f,200/255f).ToColor();
            case 4: return new ColorHSB(220/255f,150/255f,245/255f).ToColor();
            case 5: return new ColorHSB(  5/255f,250/255f,245/255f).ToColor();
			default: return Color.red; // Oops! Too many colors.
			}
		}

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (BoardView _myBoardView, Tile _myObj) {
			base.InitializeAsBoardOccupantView (_myBoardView, _myObj);
			myTile = _myObj;
			bodyColor = GetBodyColor(myTile.ColorID);

			// Set size!
			myRectTransform.sizeDelta = new Vector2(_myBoardView.UnitSize*0.92f, _myBoardView.UnitSize*0.92f);

			bodyImages = new List<Image>(); // Note: We add our first image in UpdateVisualsPostMove.
		}
		private void ApplyColor(Color color) {
			for (int i=0; i<bodyImages.Count; i++) { bodyImages[i].color = color; }
		}

		private void AddBodyImage(Vector2Int footPos) {
			float unitSize = MyBoardView.UnitSize;
			Image newImage = new GameObject().AddComponent<Image>();
			GameUtils.ParentAndReset(newImage.gameObject, this.transform);
			GameUtils.SizeUIGraphic(newImage, unitSize,unitSize);
			newImage.color = bodyColor;
			newImage.rectTransform.anchoredPosition = new Vector2(footPos.x*unitSize, -footPos.y*unitSize);
			newImage.transform.SetAsFirstSibling(); // put behind everything else.
			newImage.name = "i_FootprintUnit";
			bodyImages.Add(newImage);
		}


//		public void OnGroupIDToMoveChanged(int _groupIDToMove) {
//			i_highlight.enabled = myTile.GroupID == _groupIDToMove;
//		}

		override public void UpdateVisualsPostMove() {
			base.UpdateVisualsPostMove();
			// Add images to fit my footprint!
			if (bodyImages.Count != myTile.FootprintLocal.Count) {
				for (int i=bodyImages.Count; i<myTile.FootprintLocal.Count; i++) {
					AddBodyImage(myTile.FootprintLocal[i]);
				}
			}
		}



		public void OnMouseOut() {
			SetHighlightAlpha(0);
		}
		public void OnMouseOver() {
			SetHighlightAlpha(0.15f);
		}
		public void OnStopGrabbing() {
			SetHighlightAlpha(0);
		}
		public void OnStartGrabbing() {
			SetHighlightAlpha(0.3f);
		}
		private void SetHighlightAlpha(float alpha) {
			// FOR NOW, just color my body sprites instead of showing separate image(s).
			ApplyColor(Color.Lerp(bodyColor, Color.white, alpha));
		}


        // ----------------------------------------------------------------
        //  Animating
        // ----------------------------------------------------------------
//        public void AnimateIn(Tile sourceTile) {
//            StartCoroutine(Coroutine_AnimateIn(sourceTile));
//        }
//        private IEnumerator Coroutine_AnimateIn(Tile sourceTile) {
//            Vector2 targetPos = Pos;
//            float targetScale = 1;
//
//            Scale = 0f; // shrink me down
//
//            // If I came from a source, then start me there and animate me to my actual position!
//            if (sourceTile != null) {
//                Vector2 sourcePos = GetPosFromBO(sourceTile);
//                Pos = sourcePos;
//            }
//            // Animate!
//            float easing = 0.5f; // higher is faster.
//            while (Pos!=targetPos || Scale!=targetScale) {
//                Pos += new Vector2((targetPos.x-Pos.x)*easing, (targetPos.y-Pos.y)*easing);
//                Scale += (targetScale-Scale) * easing;
//                if (Vector2.Distance(Pos,targetPos)<0.5f) { Pos = targetPos; } // Almost there? Get it!
//                if (Mathf.Approximately(Scale,targetScale)) { Scale = targetScale; } // Almost there? Get it!
//                yield return null;
//            }
//        }

	}
}