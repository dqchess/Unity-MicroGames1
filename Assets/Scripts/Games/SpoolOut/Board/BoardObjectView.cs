using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
	public class BoardObjectView : MonoBehaviour {
		// Components
		protected RectTransform myRectTransform; // Set in Start.
        // References
        public BoardView MyBoardView { get; private set; }
        public BoardObject MyBoardObject { get; private set; }

        // Getters (Public)
        public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value;
			}
		}
		// Getters (Private)
        protected Vector2 GetPosFromBO (BoardObject _bo) {
			return new Vector2 (MyBoardView.BoardToX (_bo.Col), MyBoardView.BoardToY (_bo.Row));
		}


		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		protected void InitializeAsBoardObjectView (BoardView _myBoardView, Transform tf_parent, BoardObject _myObject) {
			MyBoardView = _myBoardView;
			MyBoardObject = _myObject;
			myRectTransform = this.GetComponent<RectTransform>();

			// Parent me!
			GameUtils.ParentAndReset(this.gameObject, tf_parent.transform);
//			myRectTransform.sizeDelta = new Vector2(MyBoardView.UnitSize, MyBoardView.UnitSize); // NOTE: There's no code pattern to follow here with this! SpoolView totally does its own thing.
//
//			// Start me in the right spot!   NOTE: Disabled! Spools don't follow this.
//            Pos = GetPosFromBO(_myObject);
		}





	}
}