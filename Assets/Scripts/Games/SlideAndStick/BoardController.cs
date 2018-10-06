using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardController : MonoBehaviour {
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
		// Properties
//        private bool isAnimatingTiles = false; // when true, we can't make any moves, ok?
		private bool isLevelComplete = false;
		private int groupIDToMove; // which Tile group our mouse is over, and we're gonna move.
		private Vector2Int mousePosBoard;
		// References
		private GameController gameController;

		// Getters
		public bool IsLevelComplete { get { return isLevelComplete; } }
		public Board Board { get { return board; } }
		public BoardView BoardView { get { return boardView; } }
		private InputController inputController { get { return InputController.Instance; } }


		private bool CanMakeAnyMove () {
//            if (isAnimatingTiles) { return false; } // No moves allowed while animating, ok?
			if (isLevelComplete) { return false; } // If the level's over, don't allow further movement. :)
			return true;
		}

		public void SetIsLevelOver(bool isLevelOver) {
			this.isLevelComplete = isLevelOver;
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
			boardView = Instantiate (ResourcesHandler.Instance.slideAndStick_boardView).GetComponent<BoardView>();
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
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (board==null || board.spaces == null) { return; } // To prevent errors when compiling during runtime.

			UpdateMousePosBoard();
			UpdateGroupIDToMove();
			RegisterMouseInput();
			RegisterButtonInput();
		}

		private void UpdateMousePosBoard() {
			Vector2 mousePosScaled = Input.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
			mousePosScaled += new Vector2(-boardView.Pos.x, boardView.Pos.y); // Note: Idk why negative...
			int col = Mathf.FloorToInt(mousePosScaled.x / (float)boardView.UnitSize);
			int row = Mathf.FloorToInt(mousePosScaled.y / (float)boardView.UnitSize);
			mousePosBoard = new Vector2Int(col,row);
		}

		private void UpdateGroupIDToMove() {
			// If our finger isn't on the screen, update groupIDToMove!
			if (!inputController.IsTouchHold()) {
				int prevGroupIDToMove = groupIDToMove;
				Tile tileOver = board.GetTile(mousePosBoard);
				groupIDToMove = tileOver!=null ? tileOver.GroupID : -1;
				if (groupIDToMove != prevGroupIDToMove) {
					// Tell boardView!
					boardView.OnGroupIDToMoveChanged(groupIDToMove);
				}
			}
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
			if (!CanMakeAnyMove()) { return; } // If the Dark Lord says not to make a move, you musn't, Cissy!

			Tile tileOver = board.GetTile(mousePosBoard);
			if (tileOver != null) {
				TapTile(tileOver);
			}
		}

		private void RegisterButtonInput() {
			if (false) {}

			// TEMP DEBUG
			else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    { MoveTileAttempt(Vector2Int.T); }
			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  { MoveTileAttempt(Vector2Int.B); }
			else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  { MoveTileAttempt(Vector2Int.L); }
			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { MoveTileAttempt(Vector2Int.R); }
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void TapTile(Tile tile) {
			if (tile == null) { return; } // Safety check.


		}



		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		// TEMP!! For testing.
		private void MoveTileAttempt(Vector2Int dir) {
			Tile tileToMove = board.GetTile(mousePosBoard);
			MoveTileAttempt(tileToMove, dir);
		}
		public void MoveTileAttempt(Tile tileToMove, Vector2Int dir) {
			// If we can't make this specific move, also stop.
			if (!BoardUtils.CanExecuteMove(board, tileToMove.BoardPos, dir)) {
				return;
			}
			// We CAN make this move!
			else {
//				// Take a snapshot and add it to our list!
//				BoardData preMoveSnapshot = board.SerializeAsData();
//				boardSnapshots.Add (preMoveSnapshot);
				// Move it, move it! :D
				board.ExecuteMove(tileToMove.BoardPos, dir); // This will always return success, because we already asked if this move was possible.
//				// We make moves.
//				NumMovesMade ++;
//				// Complete this move!
//				OnBoardMoveComplete ();
			}
		}

//		public void UndoLastMove () {
//			if (!CanUndoMove ()) { return; }
//			// Get the snapshot to restore to, restore, and decrement moves made!
//			BoardData boardSnapshotData = boardSnapshots[boardSnapshots.Count-1];
//			// Remake my model and view from scratch!
//			RemakeModelAndViewFromData (boardSnapshotData);
//			boardSnapshots.Remove (boardSnapshotData);
//			NumMovesMade --; // decrement this here!
//			// No, the level is definitely not complete anymore.
//			isLevelComplete = false;
//			// Tie up loose ends by "completing" this move!
//			OnBoardMoveComplete ();
//		}


	}


}