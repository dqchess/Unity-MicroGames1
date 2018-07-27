using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	public class Level : MonoBehaviour {
		// Components
		[SerializeField] private RectTransform myRectTransform;
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
		// Variable Properties
		private bool isLevelOver = false;
//		private Vector2Int mousePosBoard=new Vector2Int();
		// References
		private GameController gameController;

		// Getters
		public bool IsLevelOver { get { return isLevelOver; } }
		public Board Board { get { return board; } }
		public BoardView BoardView { get { return boardView; } }
		private InputController inputController { get { return InputController.Instance; } }


		private bool CanMakeAnyMove () {
			if (isLevelOver) { return false; } // If the level's over, don't allow further movement. :)
			return true;
		}

		public void SetIsLevelOver(bool isLevelOver) {
			this.isLevelOver = isLevelOver;
		}



		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		public void Initialize (GameController _gameController, Transform tf_parent) {
			gameController = _gameController;
			this.transform.SetParent (tf_parent);
			this.transform.localScale = Vector3.one;
			myRectTransform.offsetMax = myRectTransform.offsetMin = Vector2.zero;

			// Reset!
			BoardData bd = new BoardData(5,5);
			RemakeModelAndViewFromData (bd);
			// Add initial tiles!
			int numStartingTiles = Mathf.CeilToInt(board.NumCols*board.NumRows * 0.2f);
			board.AddRandomTiles(numStartingTiles);
			board.MatchCongruentTiles();
//			board.CalculateScore();

			// Update BoardView visuals!!
            boardView.AnimateInNewTilesFromSource(null); // TODO: Clean this up!
		}
		public void DestroySelf () {
			// Tell my boardView it's toast!
			DestroyBoardModelAndView ();
			// Destroy my whole GO.
			Destroy (this.gameObject);
		}


		private void RemakeModelAndViewFromData (BoardData bd) {
			// Destroy them first!
			DestroyBoardModelAndView ();
			// Make them afresh!
			board = new Board (bd);
			boardView = Instantiate (ResourcesHandler.Instance.extrudeMatch_boardView).GetComponent<BoardView>();
			boardView.Initialize (this, board);
		}
		private void DestroyBoardModelAndView () {
			// Nullify the model (there's nothing to destroy).
			board = null;
			// Destroy view.
			if (boardView != null) {
				boardView.DestroySelf ();
				boardView = null;
			}
		}

		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		private void TapTile(Tile tile) {
			if (tile == null) { return; } // Safety check.
			// If we CAN'T execute this move, do nothing.
			if (!board.CanExtrudeTile(tile)) { return; }

			StartCoroutine(Coroutine_ExtrudeTileSeq(tile));
		}
		private IEnumerator Coroutine_ExtrudeTileSeq(Tile tile) {
			// Extrude the tile!
			board.ExtrudeTile(tile);
            // Remove the removed view and animate in the new guys!
            boardView.AnimateOutRemovedTiles();
            boardView.AnimateInNewTilesFromSource(tile);

			// Wait a moment, then add a new random tile!
			yield return new WaitForSeconds(0.2f);
			board.AddRandomTiles(1);
            boardView.AnimateInNewTilesFromSource(null);

			// Wait another moment, then match all congruent tiles!
			yield return new WaitForSeconds(0.2f);
			board.MatchCongruentTiles();
			boardView.AnimateOutRemovedTiles();

			// TODO: Update score
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
//		private void OnMoveComplete() {
//			// Update BoardView visuals!!
//			boardView.UpdateViewsPostMove();
//		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (board==null || board.spaces == null) { return; } // To prevent errors when compiling during runtime.

			//		UpdateMousePos();
			//		UpdateMousePosBoard();
			RegisterMouseInput();
            //GetMousePosBoard();//
		}

		private Vector2Int GetMousePosBoard() {
			Vector2 mousePosScaled = Input.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
			mousePosScaled += boardView.Pos;
			int col = Mathf.FloorToInt(mousePosScaled.x / (float)boardView.UnitSize);
			int row = Mathf.FloorToInt(mousePosScaled.y / (float)boardView.UnitSize);
			//print("mousePosBoard: " + col+","+row + "   mousePosScaled: " + mousePosScaled);
			return new Vector2Int(col,row);
		}

		private void RegisterMouseInput() {
			if (inputController == null) { return; } // For compiling during runtime.
			// Mouse UP
			if (inputController.IsTouchUp()) {
//				OnTouchUp();
			}
			// Mouse DOWN
			else if (inputController.IsTouchDown()) {
				OnTouchDown();
			}
		}
		private void OnTouchDown() {
			if (isLevelOver) { return; } // If the level's over, do nothing.

			Vector2Int mousePosBoard = GetMousePosBoard();
			Tile tileOver = board.GetTile(mousePosBoard);
			if (tileOver != null) {
				TapTile(tileOver);
			}
		}


	}


}