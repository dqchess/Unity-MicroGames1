using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlideAndStick {
	public class Level : MonoBehaviour {
		// Components
        [SerializeField] protected RectTransform myRectTransform=null;
		[SerializeField] private UndoMoveInputController undoMoveInputController=null;
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
        private SimMoveController simMoveController; // this guy handles all the mobile touch stuff.
		// Properties
        [HideInInspector] public bool IsAnimating; // set this to true if we're animating in OR out.
        private bool IsLevelOver;
		private int numMovesMade; // reset to 0 at the start of each level. Undoing a move will decrement this.
        private LevelAddress myAddress;
		private Vector2Int mousePosBoard;
		private List<BoardData> boardSnapshots; // for undoing moves! Before each move, we add a snapshot of the board to this list (and remove from list when we undo).
		// References
		private GameController gameController;
		private Tile tileOver; // the Tile my mouse is over.
		private Tile tileGrabbing; // the Tile we're holding and simulating a move for.

		// Getters (Public)
		public Board Board { get { return board; } }
		public BoardView BoardView { get { return boardView; } }
		public GameController GameController { get { return gameController; } }
        public LevelAddress MyAddress { get { return myAddress; } }
		public UndoMoveInputController UndoMoveInputController { get { return undoMoveInputController; } }
        public bool IsPlaying { get { return !IsAnimating && !IsLevelOver; } }
		// Getters (Private)
		private InputController inputController { get { return InputController.Instance; } }
		private bool IsPlayerMove_L() { return Input.GetButtonDown("MoveL") || simMoveController.IsSwipe_L; }
		private bool IsPlayerMove_R() { return Input.GetButtonDown("MoveR") || simMoveController.IsSwipe_R; }
		private bool IsPlayerMove_D() { return Input.GetButtonDown("MoveD") || simMoveController.IsSwipe_D; }
		private bool IsPlayerMove_U() { return Input.GetButtonDown("MoveU") || simMoveController.IsSwipe_U; }
		private bool CanMakeAnyMove() {
            if (!IsPlaying) { return false; } // Not playing? Don't allow further movement. :)
            if (!gameController.FUEController.CanTouchBoard) { return false; } // FUE's locked us out? No movin'.
            if (board!=null && board.IsInKnownFailState) { return false; } // In a known fail state? Force us to undo.
			return true;
		}
		private TileView Temp_GetTileView(Tile _tile) {
			foreach (BoardOccupantView bov in boardView.allOccupantViews) {
				if (bov.MyBoardOccupant == _tile) {
					return bov as TileView;
				}
			}
			return null;
		}
		private bool CanUndoMove () {
			if (!IsPlaying) { return false; } // Not playing? No undos. ;)
			if (NumMovesMade <= 0) { return false; } // Can't go before time started, duhh.
			return true;
		}

		public int NumMovesMade {
			get { return numMovesMade; }
			private set {
				numMovesMade = value;
				undoMoveInputController.OnNumMovesMadeChanged(numMovesMade);
				// Dispatch event!
//				GameManagers.Instance.EventManager.OnNumMovesMadeChanged(numMovesMade);
			}
		
        }

		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		public void Initialize (GameController _gameController, Transform tf_parent, LevelData _levelData) {
			gameController = _gameController;
            myAddress = _levelData.myAddress;
            IsLevelOver = false;
    
            gameObject.name = "Level " + myAddress.level;
            GameUtils.ParentAndReset(this.gameObject, tf_parent);
            myRectTransform.SetAsFirstSibling(); // put me behind all other UI.
            myRectTransform.anchoredPosition = Vector2.zero;
			myRectTransform.offsetMax = myRectTransform.offsetMin = Vector2.zero;

			// Reset easy stuff
			boardSnapshots = new List<BoardData>();
			NumMovesMade = 0;

			// Send in the clowns!
            RemakeModelAndViewFromData(_levelData.boardData);
            simMoveController = new SimMoveController(this);
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
			// Nullify tileOver and tileGrabbing (the refs don't exist anymore).
			tileOver = null;
			tileGrabbing = null;
		}
        /*
		override protected void AddLevelComponents() {
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
        */



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (board==null || board.spaces == null) { return; } // To prevent errors when compiling during runtime.

            simMoveController.Update();

            UpdateMousePosBoard();
            UpdateTileOver();

            RegisterTouchInput();
            RegisterButtonInput();

            boardView.UpdateSimMove(tileGrabbing, simMoveController.SimMoveDir, simMoveController.SimMovePercent);
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
			// A) Can't make a move? Then no highlighting.
			if (!CanMakeAnyMove()) { SetTileOver(null); }
			// B) If we're GRABBING a Tile already, FORCE tileOver to be THAT Tile!
			else if (tileGrabbing != null) { SetTileOver(tileGrabbing); }
			// C) Otherwise, use the one the mouse is over.
			else { SetTileOver(board.GetTile(mousePosBoard)); }
        }
        private void SetTileOver(Tile tile) {
            Tile prevTileOver = tileOver;
            tileOver = tile;
			// It's changed!
			if (prevTileOver != tileOver) {
				if (prevTileOver!=null && prevTileOver.IsInPlay) { Temp_GetTileView(prevTileOver).OnMouseOut(); }
				if (tileOver != null) { Temp_GetTileView(tileOver).OnMouseOver(); }
			}
		}

		private void RegisterButtonInput() {
			// DEBUG
			if (Input.GetKeyDown(KeyCode.T)) { board.Debug_PrintBoardLayout(); }
		}

		private void RegisterTouchInput() {
			if (inputController == null) { return; } // For compiling during runtime.
            
			if 		(IsPlayerMove_L())  { MoveTileAttempt(tileGrabbing, Vector2Int.L); }
			else if (IsPlayerMove_R())  { MoveTileAttempt(tileGrabbing, Vector2Int.R); }
			else if (IsPlayerMove_D())  { MoveTileAttempt(tileGrabbing, Vector2Int.B); }
			else if (IsPlayerMove_U())  { MoveTileAttempt(tileGrabbing, Vector2Int.T); }

			if (inputController.IsTouchUp()) { OnTouchUp(); }
			else if (inputController.IsTouchDown()) { OnTouchDown(); }
        }

		private void OnTouchDown() {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			if (tileOver != null) {
				SetTileGrabbing(tileOver);
			}
		}
		private void OnTouchUp() {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			SetTileGrabbing(null);
		}

		private void SetTileGrabbing(Tile _tile) {
			if (tileGrabbing != _tile) { // If it's changed...!
				Tile prevTileGrabbing = tileGrabbing;
				tileGrabbing = _tile;
				// Tell the Tiles!
				if (prevTileGrabbing!=null && prevTileGrabbing.IsInPlay) { Temp_GetTileView(prevTileGrabbing).OnStopGrabbing(); }
				if (tileGrabbing!=null) { Temp_GetTileView(tileGrabbing).OnStartGrabbing(); }
			}
		}
		/// Call this after we finish a move: tileGrabbing may now be null (it was destroyed in a merge), so we want to set tileGrabbing to what it LOOKS like we were already grabbing.
		private void ConfirmTileGrabbing() {
			if (tileGrabbing != null) {
				SetTileGrabbing(board.GetTile(tileGrabbing.BoardPos));
			}
		}


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        private void OnBoardMoveComplete () {
            // Tell BoardView!
			boardView.OnBoardMoveComplete();
			// Trade-off tileGrabbing, in case it's changed (from a merge)!
			ConfirmTileGrabbing();
			// Tell people!
			gameController.FUEController.OnBoardMoveComplete();
            // In fail scenario? Nullify tileOver and tileGrabbing.
            if (board.IsInKnownFailState) {
                SetTileOver(null);
                SetTileGrabbing(null);
            }
            
            // If our goals are satisfied, win!!
            if (board.AreGoalsSatisfied) {
                gameController.OnBoardGoalsSatisfied();
            }
        }
		public void OnWinLevel() {
			IsLevelOver = true;
			// Make sure to let go of any tileGrabbing for visuals.
			SetTileGrabbing(null);
		}



		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		public void MoveTileAttempt(Tile tileToMove, Vector2Int dir) {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			// If we can't make this specific move, also stop.
			if (!BoardUtils.CanExecuteMove(board, tileToMove, dir)) {
				return;
			}
			// We CAN make this move!
			else {
				// Take a snapshot and add it to our list!
				BoardData preMoveSnapshot = board.SerializeAsData();
				boardSnapshots.Add (preMoveSnapshot);
				// Move it, move it! :D
				board.ExecuteMove(tileToMove.BoardPos, dir); // This will always return success, because we already asked if this move was possible.
				// We make moves.
				NumMovesMade ++;
				// Complete this move!
				OnBoardMoveComplete ();
			}
		}

		public void UndoMoveAttempt() {
			if (!CanUndoMove()) { return; }
			// Get the snapshot to restore to, restore, and decrement moves made!
			BoardData boardSnapshotData = boardSnapshots[boardSnapshots.Count-1];
			// Remake my model and view from scratch!
			RemakeModelAndViewFromData(boardSnapshotData);
			boardSnapshots.Remove(boardSnapshotData);
			NumMovesMade --; // decrement this here!
			gameController.FUEController.OnUndoMove();
			// Tie up loose ends by "completing" this move!
			OnBoardMoveComplete();
		}


	}


}