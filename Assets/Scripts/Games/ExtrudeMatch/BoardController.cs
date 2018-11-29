using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	public class BoardController : MonoBehaviour {
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
		// Properties
        private bool isAnimatingTiles = false; // when true, we can't make any moves, ok?
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
            if (isAnimatingTiles) { return false; } // No moves allowed while animating, ok?
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
			board.ClearCongruentTiles();
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
            isAnimatingTiles = true;

			// Extrude the tile!
			board.ExtrudeTile(tile);
            // Remove the removed view and animate in the new guys!
            boardView.AnimateOutRemovedTiles(RemovalTypes.ExtrudeSource);
            boardView.AnimateInNewTilesFromSource(tile);
            yield return new WaitForSeconds(0.2f);

            // Match all congruent tiles! If we DID match, animate the view and wait for the animation.
            if (ClearCongruentTiles()) {
                yield return new WaitForSeconds(0.6f);
            }

            // Add a new random tile!
            board.AddRandomTiles(1);
            boardView.AnimateInNewTilesFromSource(null);
            yield return new WaitForSeconds(0.2f);

            // Match all congruent tiles AGAIN! If we DID match, animate the view and wait for the animation.
            if (ClearCongruentTiles()) {
                yield return new WaitForSeconds(0.6f);
            }

            isAnimatingTiles = false;

            // Is it game over, brah?
            if (board.AreAllSpacesFilled()) {
                gameController.GameOver();
            }
		}
        private bool ClearCongruentTiles() {
            int numCleared = board.ClearCongruentTiles();
            boardView.AnimateOutRemovedTiles(RemovalTypes.Matched);
            // Increase our score!
            gameController.AddToScore(numCleared*10); // HARDCODED score scale. 10 points per tile cleared.
            return numCleared>0;
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
			Vector2 mousePosScaled = InputController.Instance.TouchPosScaled;//.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
            mousePosScaled += new Vector2(-boardView.Pos.x, boardView.Pos.y); // Note: Idk why negative...
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
            if (!CanMakeAnyMove()) { return; } // If the Dark Lord says not to make a move, you musn't, Cissy!

			Vector2Int mousePosBoard = GetMousePosBoard();
			Tile tileOver = board.GetTile(mousePosBoard);
			if (tileOver != null) {
				TapTile(tileOver);
			}
		}


	}


}