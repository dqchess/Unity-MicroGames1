using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtrudeMatch {
	public class BoardSpaceView : MonoBehaviour {
		// Components
		[SerializeField] private Image i_backing=null;
		[SerializeField] private Image i_border=null;
		// Properties
		private Color fillColor;
		// References
//		private BoardSpace mySpace;

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (BoardView _boardView, BoardSpace mySpace) {
//			this.mySpace = mySpace;
			int col = mySpace.Col;
			int row = mySpace.Row;

			// Parent me to my boooard!
			this.transform.SetParent (_boardView.tf_BoardSpaces);
			this.gameObject.name = "BoardSpace_" + col + "," + row;

			// Size/position me right!
			this.transform.localPosition = new Vector3 (_boardView.BoardToX(col), _boardView.BoardToY(row), 0);
			this.transform.localScale = Vector3.one;
			this.transform.localEulerAngles = Vector3.zero;

			// YES playable? Make visuals nice!
			if (mySpace.IsPlayable) {
				const float spaceGap = 0f; // make this something between 0 and 5, to taste.
				float diameter = _boardView.UnitSize-spaceGap;
				GameUtils.SizeUIGraphic (i_border, _boardView.UnitSize+4,_boardView.UnitSize+4);
				GameUtils.SizeUIGraphic (i_backing, diameter,diameter);

                float fillS = Random.Range(0.2f, 0.25f);
                float fillB = Random.Range(0.9f, 0.98f);
                //float fillB = Random.Range(0.01f, 0.06f);
				fillColor = new ColorHSB(30/360f, fillS, fillB).ToColor();
				i_backing.color = fillColor;
				i_border.color = new ColorHSB(30/360f, 0.2f, 0.75f).ToColor();
                i_border.enabled = false; // note: DISABLED borders!
			}
			// NOT playable? Destroy my sprites and do nothing.
			else {
				GameObject.Destroy(i_border.gameObject);
				GameObject.Destroy(i_backing.gameObject);
			}
		}


	}
}