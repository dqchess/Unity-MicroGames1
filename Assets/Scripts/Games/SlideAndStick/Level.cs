using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlideAndStick {
	public class Level : BaseLevel {
		// Components
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
		private TouchInputDetector touchInputDetector; // this guy handles all the mobile touch stuff.
		// Properties
//        private bool isAnimatingTiles = false; // when true, we can't make any moves, ok?
		private bool isLevelComplete = false;
		private string description; // dev's description of the level (set in Levels.txt).
		private Vector2Int mousePosBoard;
		// References
		private GameController gameController;
		private Tile tileOver; // the Tile my mouse is over.

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
		public void Initialize (GameController _gameController, Transform tf_parent, int _levelIndex) {
			gameController = _gameController;
			base.BaseInitialize(_gameController, tf_parent, _levelIndex);
			myRectTransform.offsetMax = myRectTransform.offsetMin = Vector2.zero;

			touchInputDetector = new TouchInputDetector ();

			AddLevelComponents();
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
		override protected void AddLevelComponents() {
			DestroyBoardModelAndView(); // Just in case.
			if (resourcesHandler == null) { return; } // Safety check for runtime compile.

			string levelString = gameController.LevelLoader.GetLevelString(LevelIndex);
			if (!string.IsNullOrEmpty(levelString)) {
				MakeLevelFromString(levelString);
			}
			else {
//				DestroyLevelComponents();
//				levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
				Debug.LogWarning("No level data available for level: " + LevelIndex);
			}
//			if (LevelIndex > LastLevelIndex) {
//				levelUI.t_moreLevelsComingSoon.gameObject.SetActive(true);
//			}
		}
		private void MakeLevelFromString(string _str) {
			try {
				string[] lines = TextUtils.GetStringArrayFromStringWithLineBreaks(_str);
				description = lines[0]; // Description will be the first line (what follows "LEVEL ").
				string[] boardLayout = lines.Skip(1).ToArray(); // skip the descrpition string. The rest is the board layout! :)
				BoardData boardData = new BoardData(boardLayout);
				RemakeModelAndViewFromData(boardData);
			}
			catch (System.Exception e) {
				Debug.LogError("Error reading level string! LevelIndex: " + LevelIndex + ", description: \"" + description + "\". Error: " + e);
			}
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (board==null || board.spaces == null) { return; } // To prevent errors when compiling during runtime.

			touchInputDetector.Update();
			UpdateMousePosBoard();
			UpdateTileOver();
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

		private void UpdateTileOver() {
			// If our finger isn't touching the screen, update tileOver!
			if (!inputController.IsTouchHold()) {
				Tile prevTileOver = tileOver;
				tileOver = board.GetTile(mousePosBoard);
				if (prevTileOver != tileOver) { // it's changed!
//					if (prevTileOver != null) { prevTileOver.OnMouseOut(); }
//					if (tileOver != null) { tileOver.OnMouseOver(); }
				}
			}
		}


		private void RegisterMouseInput() {
			if (inputController == null) { return; } // For compiling during runtime.
//			// Mouse UP
//			if (inputController.IsTouchUp()) {
////				OnTouchUp();
//			}
//			// Mouse DOWN
//			else if (inputController.IsTouchDown()) {
//				OnTouchDown();
//			}

			if (inputController != null) { // Safety check for runtime compile.
				boardView.UpdateSimulatedMove(tileOver, touchInputDetector.SimulatedMoveDir, touchInputDetector.SimulatedMovePercent);
				if 		(inputController.IsPlayerMove_L ())  { MoveTileOverAttempt(Vector2Int.L); }
				else if (inputController.IsPlayerMove_R ())  { MoveTileOverAttempt(Vector2Int.R); }
				else if (inputController.IsPlayerMove_D ())  { MoveTileOverAttempt(Vector2Int.B); }
				else if (inputController.IsPlayerMove_U ())  { MoveTileOverAttempt(Vector2Int.T); }
			}
		}
//		private void OnTouchDown() {
//			if (!CanMakeAnyMove()) { return; } // If the Dark Lord says not to make a move, you musn't, Cissy!
//
//			Tile tileOver = board.GetTile(mousePosBoard);
//			if (tileOver != null) {
//				TapTile(tileOver);
//			}
//		}

		private void RegisterButtonInput() {
			if (false) {}

			// TEMP DEBUG
			else if (Input.GetKeyDown(KeyCode.P)) { board.Debug_PrintBoardLayout(); }
//			else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    { MoveTileOverAttempt(Vector2Int.T); }
//			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  { MoveTileOverAttempt(Vector2Int.B); }
//			else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  { MoveTileOverAttempt(Vector2Int.L); }
//			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { MoveTileOverAttempt(Vector2Int.R); }
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void TapTile(Tile tile) {
			if (tile == null) { return; } // Safety check.

		}

		private void OnBoardMoveComplete () {
			// Update BoardView visuals!!
			boardView.UpdateAllViewsMoveStart ();
//			// If our goals are satisfied AND the player's at the exit spot, advance to the next level!!
//			if (board.AreGoalsSatisfied) {
//				CompleteLevel ();
//			}
//			// Dispatch success/not-yet-success event!
//			GameManagers.Instance.EventManager.OnSetIsLevelCompleted (isLevelComplete);
		}



		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		// TEMP!! For testing.
		private void MoveTileOverAttempt(Vector2Int dir) {
			Tile tileToMove = board.GetTile(mousePosBoard);
			MoveTileAttempt(tileToMove, dir);
		}
		public void MoveTileAttempt(Tile tileToMove, Vector2Int dir) {
			// If we can't make this specific move, also stop.
			if (!BoardUtils.CanExecuteMove(board, tileToMove, dir)) {
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
				// Complete this move!
				OnBoardMoveComplete ();
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