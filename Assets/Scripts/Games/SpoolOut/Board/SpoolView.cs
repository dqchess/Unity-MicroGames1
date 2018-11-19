using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpoolOut {
	public class SpoolView : BoardObjectView {
		// Components
		[SerializeField] private Image i_core=null;
		[SerializeField] private RectTransform rt_core=null;
		[SerializeField] private SpoolPathView pathView=null;
		[SerializeField] private TextMeshProUGUI t_numSpacesLeft=null;
        // References
        public Spool MySpool { get; private set; }
        // Properties
		public Color PathColor { get; private set; }
		private Color coreColor;

        // Getters (Static)
        static public Color GetPathColor(int colorID) {
            switch (colorID) {
                case -1: return new Color(128/255f,128/255f,128/255f, 20/255f);
                case  0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
                case  1: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
                case  2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
                case  3: return new ColorHSB(190/255f,150/255f,245/255f).ToColor();
                case  4: return new ColorHSB(230/255f,150/255f,245/255f).ToColor();
                case  5: return new ColorHSB( 80/255f,170/255f,245/255f).ToColor();
                case  6: return new ColorHSB( 12/255f,170/255f,245/255f).ToColor();
                default: return new ColorHSB(Random.Range(0,1f),150/255f,245/255f).ToColor(); // Hmm.
            }
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (BoardView _myBoardView, Transform tf_parent, Spool _myObj) {
			MySpool = _myObj;
			base.InitializeAsBoardObjectView(_myBoardView, tf_parent, _myObj);
			myRectTransform.anchoredPosition = Vector2.zero; // Put me at 0,0. My components are what I position!
			PathColor = GetPathColor(MySpool.ColorID);
			coreColor = Color.Lerp(PathColor,Color.black, 0.08f);
            
			i_core.color = coreColor;
            
			rt_core.anchoredPosition = GetPosFromBO(MySpool);
			rt_core.sizeDelta = new Vector2(MyBoardView.UnitSize,MyBoardView.UnitSize)*0.95f;

			pathView.Initialize();
			WholesaleRemakeVisuals(); // Look fresh from the get-go.
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void WholesaleRemakeVisuals() {
			// Text!
			int numSpacesLeft = MySpool.NumSpacesLeft;
			t_numSpacesLeft.enabled = numSpacesLeft != 0; // don't show text if we're satisfied. ;)
			t_numSpacesLeft.text = MySpool.NumSpacesLeft.ToString();
			// PathView!
			pathView.WholesaleRemakeVisuals();
		}


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
		private void OnMySpoolPathChanged() {
			WholesaleRemakeVisuals();
		}
		public void OnMouseOut() {
			SetHighlightAlpha(0);
		}
		public void OnMouseOver() {
			SetHighlightAlpha(0.25f);
		}
		public void OnStopGrabbing() {
			SetHighlightAlpha(0);
		}
		public void OnStartGrabbing() {
			SetHighlightAlpha(0.35f);
		}
        private void SetHighlightAlpha(float alpha) {
			// Update pathView.
			pathView.SetEndHighlightAlpha(alpha);
			// Update my core.
			float coreHighlight = MySpool.PathSpaces.Count<2 ? alpha*0.5f : 0; // ONLY highlight core when there's no path.
			i_core.color = Color.Lerp(coreColor,Color.black, coreHighlight);
        }

/*
  void Update() {
    UpdateHighlightValues();
    cardThickness = numSpacesLeft*1.3;//6 - (highlightLoc*5);
  }
  private void UpdateHighlightValues() {
    isHighlighted = wellOver==this || wellGrabbing==this;
    float highlightLocTarget = isHighlighted ? 1 : 0;
    if (highlightLoc != highlightLocTarget) {
      highlightLoc += (highlightLocTarget-highlightLoc) * 0.4;
    }
  }
  
  void Draw() {
    DrawPath();
    DrawCore();
    DrawEndHighlight();
  }
  private void DrawPath() {
    // Path!
    noFill();
    stroke(bodyColor);
    strokeWeight(unitSize.x*0.6);
    beginShape();
    for (int i=0; i<pathSpaces.length; i++) {
      vertex(GridToX(pathSpaces[i].x), GridToY(pathSpaces[i].y));
    }
    endShape();
  }
  private void DrawCore() {
    pushMatrix();
    translate(corePos.x,corePos.y);
    
    // Base
    fill(sideColor);
    noStroke();
    rectMode(CENTER);
    rect(0,0, unitSize.x,unitSize.y);
    // Body
    fill(bodyColor);
    noStroke();
    rectMode(CENTER);
    rect(0,-cardThickness, unitSize.x,unitSize.y-cardThickness*0.5);
    // Text
    if (numSpacesLeft > 0) {
      fill(0, 120);
      textAlign(CENTER, CENTER);
      textSize(textSize);
      text(numSpacesLeft, 0,-cardThickness);
    }
    
    popMatrix();
  }
  private void DrawEndHighlight() {
    if (isHighlighted) {
      fill(sideColor, highlightLoc*80);
      PVector lastPos = GridToPos(lastSpacePos());
//      rect(lastPos.x,lastPos.y-cardThickness*0.5, unitSize.x,unitSize.y+cardThickness);
      ellipse(lastPos.x,lastPos.y, unitSize.x*0.7,unitSize.y*0.7);
    }
  }
  */


	}
}