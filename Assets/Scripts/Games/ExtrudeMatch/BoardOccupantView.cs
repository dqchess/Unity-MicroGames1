using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtrudeMatch {
	public class BoardOccupantView : BoardObjectView {
		// Components
		[SerializeField] protected Image i_body; // everyone has a primary body sprite for simplicity! Less code.
		// References
		protected BoardOccupant myOccupant; // a direct reference to my model. Doesn't change.

		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		protected void InitializeAsBoardOccupantView (BoardView _myBoardView, BoardOccupant _myOccupant) {
			base.InitializeAsBoardObjectView (_myBoardView, _myOccupant);
			myOccupant = _myOccupant;
		}



	}
}
