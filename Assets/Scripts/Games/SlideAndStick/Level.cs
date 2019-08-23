using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class Level : MonoBehaviour {
		// Components
		public UnityEngine.UI.Image i_test0;// DEBUG! TODO: Remove these.
		public UnityEngine.UI.Image i_test1;
        [SerializeField] protected RectTransform myRectTransform=null;
        [SerializeField] private LevelEffectsHandler effectsHandler=null;
        [SerializeField] private UndoMoveInputController undoMoveInputController=null;
        public Board Board { get; private set; } // this ONLY changes when we undo a move, as we remake-from-scratch both Board and BoardView.
        public BoardView BoardView { get; private set; }
        private SimMoveController simMoveController; // this guy handles all the mobile touch stuff.
		// Properties
        [HideInInspector] public bool IsAnimating; // set this to true if we're animating in OR out.
        private bool IsLevelOver;
        public float TimeSpentThisPlay { get; private set; }
		private int numMovesMade; // reset to 0 at the start of each level. Undoing a move will decrement this.
        public LevelAddress MyAddress { get; private set; }
        public Vector2Int MousePosBoard { get; private set; }
		public  Vector2Int TileGrabbingClickBoardOffset { get; private set; } // when we set tileGrabbing, this is the boardPos difference between our mouse and the Tile. For retaining tileGrabbing for split tiles.
		private List<BoardData> boardSnapshots; // for undoing moves! Before each move, we add a snapshot of the board to this list (and remove from list when we undo).
		// References
        [SerializeField] private LevelUI levelUI=null;
        private Tile tileOver; // the Tile my mouse is over.
		private Tile tileGrabbing; // the Tile we're holding and simulating a move for.
        public GameController GameController { get; private set; }

		// Getters (Public)
        //public TileView Temp_TileViewGrabbing { get { return Temp_GetTileView(tileGrabbing); } }
        //public Tile Temp_TileGrabbing { get { return tileGrabbing; } }
        public BoardData Debug_FirstBoardSnapshot { get { return boardSnapshots.Count>0 ? boardSnapshots[0] : null; } }
        public LevelEffectsHandler EffectsHandler { get { return effectsHandler; } }
        public LevelUI LevelUI { get { return levelUI; } }
		public UndoMoveInputController UndoMoveInputController { get { return undoMoveInputController; } }
		public bool CanMakeAnyMove() {
			if (!IsPlaying) { return false; } // Not playing? Don't allow further movement. :)
            if (!GameController.CanTouchBoard()) { return false; }
			if (Board!=null && Board.IsInKnownFailState) { return false; } // In a known fail state? Force us to undo.
			return true;
		}
        // Getters (Private)
        private InputController inputController { get { return InputController.Instance; } }
        private SlideAndStickSfxController sfxController { get { return GameController.SFXController; } }
        private bool IsPlaying { get { return !IsAnimating && !IsLevelOver; } }
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
			}
		}
		private Vector2Int GetMousePosOffset(BoardOccupant bo) {
			if (bo == null) { return Vector2Int.zero; }
			return MousePosBoard - bo.BoardPos.ToVector2Int();
		}


		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		public void Initialize (GameController _gameController, Transform tf_parent, LevelData _levelData) {
			GameController = _gameController;
            MyAddress = _levelData.myAddress;
            IsLevelOver = false;
    
            gameObject.name = "Level " + MyAddress.level;
            GameUtils.ParentAndReset(this.gameObject, tf_parent);
            myRectTransform.SetAsFirstSibling(); // put me behind all other UI.
            myRectTransform.anchoredPosition = Vector2.zero;
			myRectTransform.offsetMax = myRectTransform.offsetMin = Vector2.zero;

			// Reset easy stuff
			ResetTimeSpentThisPlay();
			ResetSnapshotsAndMovesMade();

			// Send in the clowns!
			RemakeModelAndViewFromData(_levelData.boardData);
            simMoveController = new SimMoveController(this);
            // Prep me for my boardView animating in.
            BoardView.PreAnimateInFreshBoard();
            
            // Dispatch event!
            GameManagers.Instance.EventManager.SlideAndStick_OnStartLevel(this);
		}
        private void OnDestroy() {
            // Make sure to increment how long we've spent in me!
            AddToTimeSpentTotal();
        }
        public void DestroySelf() {
            Destroy(this.gameObject);
        }


		private void ResetSnapshotsAndMovesMade() {
			boardSnapshots = new List<BoardData>();
			NumMovesMade = 0;
		}
        private void RemakeModelAndViewFromData(BoardData bd) {
			// Destroy them first!
			DestroyBoardModelAndView ();
			// Make them afresh!
			Board = new Board(bd);
            AddReasonableRandTilesToBoardIfNone(); // For rando layout generating!
			BoardView = Instantiate (ResourcesHandler.Instance.slideAndStick_boardView).GetComponent<BoardView>();
			BoardView.Initialize (this, Board);
            // Tell ppl!
            levelUI.OnBoardMade();
            undoMoveInputController.OnBoardMade();
		}
		private void DestroyBoardModelAndView () {
			// Nullify the model (there's nothing to destroy).
			Board = null;
			// Destroy view.
			if (BoardView != null) {
				BoardView.DestroySelf ();
				BoardView = null;
			}
			// Nullify tileOver and tileGrabbing (the refs don't exist anymore).
			tileOver = null;
			tileGrabbing = null;
		}
        
        private void AddReasonableRandTilesToBoardIfNone() {
            // First, remember the basic empty boardData.
            BoardData bd = Board.SerializeAsData();
            int count=0;
            while (true) {
                Board.Debug_AddTilesIfNone(GameController.randGenParams);
                if (++count >= 99) { break; } // Try 99 times!
                // Not a good enough layout? Try again.
                if (Board.AreAnyTileColorsSatisfied() || Board.NumColors()==1) {
                    Board = new Board(bd);
                }
                else {
                    break;
                }
            }
        }
        
        
        // ----------------------------------------------------------------
        //  Animate In/Out
        // ----------------------------------------------------------------
        private const float animInOutDuration = 1.2f;
        public static readonly Vector2 animInOutOffset = new Vector2(400, 1200);
        /** Animates the WHOLE LEVEL, including UI. From up offscreen to onscreen. */
        public void AnimateIn() {
            // I'm animating!
            IsAnimating = true;
            Vector3 posDefault = transform.localPosition;
            transform.localPosition += new Vector3(animInOutOffset.x,animInOutOffset.y,0);
            LeanTween.moveLocal(gameObject, posDefault, animInOutDuration).setEaseInOutQuart().setOnComplete(OnCompleteAnimateIn);
            levelUI.OnLevelAnimateIn();
             // Animate-in the Board elements before I'm fully in for tighter transition.
            LeanTween.delayedCall(animInOutDuration*0.6f, AnimateInBoard);
            // Play sound!
            sfxController.OnLevelAnimateIn();
        }
        /** Animates the WHOLE LEVEL, including UI. From onscreen to down offscreen. */
        public void AnimateOut() {
            IsAnimating = true;
            levelUI.OnLevelAnimateOut();
            LeanTween.moveLocal(gameObject, new Vector3(-animInOutOffset.x*1.8f,-animInOutOffset.y*1.8f,0), animInOutDuration).setEaseInOutQuart().setOnComplete(OnCompleteAnimateOut);
        }
        
        public void AnimateInBoard() {
            BoardView.AnimateInFreshBoard();
        }
        
        public void OnCompleteAnimateIn() {
            IsAnimating = false;
        }
        private void OnCompleteAnimateOut() {
            IsAnimating = false;
            DestroySelf();
        }



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
        public void DependentFixedUpdate() {
            BoardView.DependentFixedUpdate();
        }
		public void DependentUpdate() {
			if (Board==null || Board.spaces == null) { return; } // To prevent errors when compiling during runtime.
			if (simMoveController == null) { return; } // Safety check.

            simMoveController.Update();

            UpdateMousePosBoard();
            UpdateTileOver();

            RegisterTouchInput();
            RegisterButtonInput();
            UpdateTimeSpentThisPlay();

			if (tileGrabbing != null) {
            	BoardView.UpdateSimMove(tileGrabbing, simMoveController);
			}
		}

		private void UpdateMousePosBoard() {
			Vector2 mousePosScaled = InputController.Instance.TouchPosScaled;
			float canvasHeight = GameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
			mousePosScaled += new Vector2(-BoardView.Pos.x, BoardView.Pos.y); // Note: Idk why negative...
			int col = Mathf.FloorToInt(mousePosScaled.x / (float)BoardView.UnitSize);
			int row = Mathf.FloorToInt(mousePosScaled.y / (float)BoardView.UnitSize);
			MousePosBoard = new Vector2Int(col,row);
		}

		private void UpdateTileOver() {
			// A) Can't make a move? Then no highlighting.
			if (!CanMakeAnyMove()) { SetTileOver(null); }
			// B) If we're GRABBING a Tile already, FORCE tileOver to be THAT Tile!
			else if (tileGrabbing != null) { SetTileOver(tileGrabbing); }
			// C) Otherwise, use the one the mouse is over.
			else { SetTileOver(Board.GetTile(MousePosBoard)); }
        }
        private void SetTileOver(Tile tile) {
            Tile prevTileOver = tileOver;
            tileOver = tile;
			// It's changed!
			if (prevTileOver != tileOver) {
				if (prevTileOver!=null && prevTileOver.IsInPlay) { BoardView.Temp_GetTileView(prevTileOver).OnMouseOut(); }
				if (tileOver != null) { BoardView.Temp_GetTileView(tileOver).OnMouseOver(); }
			}
		}

		private void RegisterButtonInput() {
			// DEBUG
            if (Input.GetKeyDown(KeyCode.O)) { LevelsManager.Instance.Debug_OrderLevelsAndCopyToClipboard(MyAddress); }
			else if (Input.GetKeyDown(KeyCode.T)) { Board.Debug_CopyLayoutToClipboard(true); }
            else if (Input.GetKeyDown(KeyCode.Y)) { Board.Debug_CopyLayoutToClipboard(false); }
            else if (Input.GetKeyDown(KeyCode.C)) { Board.Debug_CopyXMLToClipboard(true); }
            else if (Input.GetKeyDown(KeyCode.V)) { Board.Debug_CopyXMLToClipboard(false); }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) { Board.Debug_CopyXMLToClipboardWithDiff(1); }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) { Board.Debug_CopyXMLToClipboardWithDiff(2); }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) { Board.Debug_CopyXMLToClipboardWithDiff(3); }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) { Board.Debug_CopyXMLToClipboardWithDiff(4); }
            else if (Input.GetKeyDown(KeyCode.Alpha5)) { Board.Debug_CopyXMLToClipboardWithDiff(5); }
            else if (Input.GetKeyDown(KeyCode.Alpha6)) { Board.Debug_CopyXMLToClipboardWithDiff(6); }
            else if (Input.GetKeyDown(KeyCode.Alpha7)) { Board.Debug_CopyXMLToClipboardWithDiff(7); }
		}

		private void RegisterTouchInput() {
			if (inputController == null) { return; } // For compiling during runtime.

			//if (inputController.IsTouchUp()) { OnTouchUp(); }
			if (inputController.IsTouchDown()) { OnTouchDown(); }
        }

		private void OnTouchDown() {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			if (tileOver != null) {
				SetTileGrabbing(tileOver);
                // Sound's good!
                sfxController.OnGrabTile();
			}
		}
		//private void OnTouchUp() {
		//	if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
		//	SetTileGrabbing(null);
		//}

		public void ExecuteMoveAttempt(Vector2Int dir) {
			MoveTileAttempt(tileGrabbing, dir);
		}
		public void OnCancelSimMove() {
			ReleaseTileGrabbing();
			BoardView.OnCanceledMove();
		}
        
        public void ReleaseTileGrabbing() {
            if (tileGrabbing != null) {
                SetTileGrabbing(null);
                // Sound's good!
                sfxController.OnReleaseTile();
            }
        }
		private void SetTileGrabbing(Tile _tile) {
			if (tileGrabbing != _tile) { // If it's changed...!
				Tile prevTileGrabbing = tileGrabbing;
				tileGrabbing = _tile;
				TileGrabbingClickBoardOffset = GetMousePosOffset(tileGrabbing);
                BoardView.OnSetTileGrabbing(tileGrabbing, prevTileGrabbing); // tell BoardView.
                // Tell the Tiles!
                if (prevTileGrabbing!=null && prevTileGrabbing.IsInPlay) { BoardView.Temp_GetTileView(prevTileGrabbing).OnStopGrabbing(); }
                if (tileGrabbing!=null) { BoardView.Temp_GetTileView(tileGrabbing).OnStartGrabbing(); }
			}
		}
		/// Call this after we finish a move: tileGrabbing may now be null (it was destroyed in a merge), so we want to set tileGrabbing to what it LOOKS like we were already grabbing.
		private void ConfirmTileGrabbing() {
			if (tileGrabbing != null) {
				Vector2Int boardPos = tileGrabbing.BoardPos.ToVector2Int() + TileGrabbingClickBoardOffset;
				SetTileGrabbing(Board.GetTile(boardPos));
			}
		}
        private void UpdateTimeSpentThisPlay() {
            if (!IsAnimating && !IsLevelOver) {
                TimeSpentThisPlay += Mathf.Min(1, Time.unscaledDeltaTime); // ignore any long stretches of time between frames (we were probably inactive).
            }
        }
        public void ResetTimeSpentThisPlay() {
            TimeSpentThisPlay = 0;
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        private void OnBoardMoveComplete() {
            // Tell BoardView!
			BoardView.OnExecutedMove();
			// Trade-off tileGrabbing, in case it's changed (from a merge)!
			ConfirmTileGrabbing();
			// Tell people!
            GameController.FUEController.OnBoardMoveComplete();
            effectsHandler.OnBoardMoveComplete();
            if (Board.DidAnyTilesMergeLastMove) {
                sfxController.OnTilesMerged();
            }
            // If our goals are satisfied, win!!
            if (Board.AreGoalsSatisfied) {
                GameController.OnBoardGoalsSatisfied();
            }
            
            // In fail scenario, OR the level's over? Nullify tileOver and tileGrabbing.
            if (Board.IsInKnownFailState || IsLevelOver) {
                SetTileOver(null);
                ReleaseTileGrabbing();
            }
        }
		public void OnWinLevel() {
			IsLevelOver = true;
            AddToTimeSpentTotal();
            IncrementNumWins();
            // Tell ppl.
			undoMoveInputController.OnWinLevel();
		}
        
        
        private void AddToTimeSpentTotal() {
            string saveKey = SaveKeys.TimeSpentTotal(GameController.MyGameName(), MyAddress);
            float timeSpentTotal = SaveStorage.GetFloat(saveKey,0);
            SaveStorage.SetFloat(saveKey, timeSpentTotal+TimeSpentThisPlay);
            //print("Time spent total now: " + (timeSpentTotal+TimeSpentThisPlay));
            ResetTimeSpentThisPlay(); // we just used it! For safety, clear it out.
        }
        private void IncrementNumWins() {
            string saveKey = SaveKeys.NumWins(GameController.MyGameName(), MyAddress);
            int numWins = SaveStorage.GetInt(saveKey,0) + 1;
            SaveStorage.SetInt(saveKey, numWins);
            //print("Num wins now: " + numWins);
        }



		// ----------------------------------------------------------------
		//  Game Doers
		// ----------------------------------------------------------------
		public void MoveTileAttempt(Tile tileToMove, Vector2Int dir) {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			// If we can't make this specific move, also stop.
			if (!BoardUtils.CanExecuteMove(Board, tileToMove, dir)) {
				return;
			}
			// We CAN make this move!
			else {
				// Take a snapshot and add it to our list!
				BoardData preMoveSnapshot = Board.SerializeAsData();
				boardSnapshots.Add (preMoveSnapshot);
				// Move it, move it! :D
				Board.ExecuteMove(tileToMove.BoardPos, dir); // This will always return success, because we already asked if this move was possible.
				// We make moves.
				NumMovesMade ++;
				// Complete this move!
				OnBoardMoveComplete();
			}
		}

		public void UndoMoveAttempt() {
            if (!CanUndoMove()) { return; }
            if (!GameController.CanAffordUndo()) { GameController.OpenGetUndosPopup(); return; }
            // Remember snapshot from BEFORE undo.
            BoardData snapshotPreUndo = Board.SerializeAsData();
			// Get the snapshot to restore to, restore, and decrement moves made!
			BoardData snapshotData = boardSnapshots[boardSnapshots.Count-1];
            // Spend the undo!
            GameController.DecrementNumUndosLeft();
			// Remake my model and view from scratch!
			RemakeModelAndViewFromData(snapshotData);
			boardSnapshots.Remove(snapshotData);
			NumMovesMade --; // decrement this here!
			GameController.FUEController.OnUndoMove();
            BoardView.OnUndoMove(snapshotPreUndo);
            undoMoveInputController.OnUndoMove();
            // Play sound!
            sfxController.OnUndoMove();
			// Tie up loose ends by "completing" this move!
			//OnBoardMoveComplete();//Note: Commented out. We don't neeed to call these as the code is now. (TBH there's really two functions in OnBoardMoveComplete: handling the move just executed, and any post-move-forward-or-backward paperwork.)
		}
		public void UndoAllMoves() {
			if (!CanUndoMove()) { return; }
			// Restore from the first snapshot, and reset 'em!
			BoardData snapshotData = boardSnapshots[0];
			ResetSnapshotsAndMovesMade();
			RemakeModelAndViewFromData(snapshotData);
//			// Tie up loose ends by "completing" this move!
//			OnBoardMoveComplete();
		}
        
        
        
        // ----------------------------------------------------------------
        //  Debug
        // ----------------------------------------------------------------
        public void Debug_RemakeBoardAndViewFromArbitrarySnapshot(BoardData boardData) {
            //// Take a snapshot and add it to our list!
            //BoardData preMoveSnapshot = board.SerializeAsData();
            //boardSnapshots.Add (preMoveSnapshot);
            // Treat this like a real move.
            RemakeModelAndViewFromData(boardData);
            //NumMovesMade ++;
            OnBoardMoveComplete();
        }

	}

}
