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

	}
}