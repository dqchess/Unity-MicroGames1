using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class Level : MonoBehaviour {
		// Components
		[SerializeField] private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		[SerializeField] private RectTransform myRectTransform;
		// References
		private GameController gameController;

		// Getters (Private)
		private InputController inputController { get { return InputController.Instance; } }
		// Getters (Public)
		public Canvas Canvas { get { return gameController.Canvas; } }



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

	//		UpdateMousePos();
	//		UpdateMousePosBoard();
			RegisterMouseInput();
            //GetMousePosBoard();
		}

		private Vector2Int GetMousePosBoard() {
			Vector2 mousePosScaled = Input.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
			mousePosScaled += new Vector2(-board.Pos.x, board.Pos.y); // Note: Idk why negative...
			int col = Mathf.FloorToInt(mousePosScaled.x / (float)board.UnitSize);
			int row = Mathf.FloorToInt(mousePosScaled.y / (float)board.UnitSize);
			print("mousePosBoard: " + col+","+row + "   mousePosScaled: " + mousePosScaled);
			return new Vector2Int(col,row);
		}

		private void RegisterMouseInput() {
			if (inputController == null) { return; } // For compiling during runtime.
			// Mouse UP
			if (inputController.IsTouchUp()) {
				OnTouchUp();
			}
			// Mouse DOWN
			else if (inputController.IsTouchDown()) {
				OnTouchDown();
			}
		}
		private void OnTouchDown() {
			Vector2Int mousePosBoard = GetMousePosBoard();
			// TODO: This
		}
		private void OnTouchUp() {

		}


	}


}