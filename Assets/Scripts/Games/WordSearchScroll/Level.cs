using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class Level : MonoBehaviour {
		// Components
		[SerializeField] private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		[SerializeField] private RectTransform myRectTransform;
//		[SerializeField] private GameUI ui;
		// Properties
		private Vector2 mousePosRelative;
		private Vector2Int mouseBoardPos;
		// References
		private GameController gameController;

		// Getters (Private)
		private InputController inputController { get { return InputController.Instance; } }
		// Getters (Public)
		public Canvas Canvas { get { return gameController.Canvas; } }
		public Vector2 MousePosRelative { get { return mousePosRelative; } }
		public Vector2Int MouseBoardPos { get { return mouseBoardPos; } }



		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		public void Initialize (GameController _gameController, Transform tf_parent) {
			gameController = _gameController;

			gameObject.name = "Level";
			GameUtils.ParentAndReset(this.gameObject, tf_parent);
			myRectTransform.SetAsFirstSibling(); // put me behind all other UI.
			myRectTransform.anchoredPosition = Vector2.zero;

//			GameUtils.FlushRectTransform(myRectTransform);
			myRectTransform.offsetMax = myRectTransform.offsetMin = Vector2.zero;

			// Initialize Board!
			board.Initialize();
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (board==null) { return; } // To prevent errors when compiling during runtime.

			UpdateMousePoses();
			RegisterMouseInput();
		}

		private void UpdateMousePoses() {
			// mousePosRelative!
			mousePosRelative = Input.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosRelative = new Vector2(mousePosRelative.x, canvasHeight-mousePosRelative.y); // convert to top-left space.
			mousePosRelative += new Vector2(-board.Pos.x, board.Pos.y); // Note: Idk why negative...
			// mousePosBoard!
			int col = Mathf.FloorToInt(mousePosRelative.x / (float)board.UnitSize);
			int row = Mathf.FloorToInt(mousePosRelative.y / (float)board.UnitSize);
			mouseBoardPos = new Vector2Int(col,row);
		}

		private void RegisterMouseInput() {
			if (inputController == null) { return; } // For compiling during runtime.
			// Mouse UP
			if (inputController.IsTouchUp()) {
				board.OnTouchUp();
			}
			// Mouse DOWN
			else if (inputController.IsTouchDown()) {
				board.OnTouchDown();
			}
		}


	}


}