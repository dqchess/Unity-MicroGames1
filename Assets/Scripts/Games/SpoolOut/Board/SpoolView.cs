using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpoolOut {
	public class SpoolView : BoardObjectView {
		// Components
        [SerializeField] private Image i_core=null;
        [SerializeField] private TextMeshProUGUI t_numSpacesLeft=null;
        // References
        public Spool MySpool { get; private set; }

        // Getters (Static)
        static public Color GetBodyColor(int colorID) {
            switch (colorID) {
                case -1: return new Color(128/255f,128/255f,128/255f, 20/255f);
                case  0: return new Color(128/255f,220/255f,200/255f);
                case  1: return new Color( 58/255f,220/255f,200/255f);
                case  2: return new Color( 28/255f,200/255f,245/255f);
                case  3: return new Color(190/255f,150/255f,245/255f);
                case  4: return new Color(230/255f,150/255f,245/255f);
                case  5: return new Color( 80/255f,170/255f,245/255f);
                case  6: return new Color( 12/255f,170/255f,245/255f);
                default: return new Color(Random.Range(0,1f),150/255f,245/255f); // Hmm.
            }
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (BoardView _myBoardView, Transform tf_parent, Spool _myObj) {
			MySpool = _myObj;
			base.InitializeAsBoardObjectView(_myBoardView, tf_parent, _myObj);
            
            // TODO: Thissss.
		}


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
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
            //body.SetHighlightAlpha(alpha);TEMP! Disabled highlighting!
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