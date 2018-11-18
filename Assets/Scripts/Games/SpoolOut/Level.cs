using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
	public class Level : MonoBehaviour {
		// Components
        [SerializeField] protected RectTransform myRectTransform=null;
		private Board board; // this reference ONLY changes when we undo a move, where we remake-from-scratch both board and boardView.
		private BoardView boardView;
		// Properties
        [HideInInspector] public bool IsAnimating; // set this to true if we're animating in OR out.
        private bool IsLevelOver;
        public float TimeSpentThisPlay { get; private set; }
        private LevelAddress myAddress;
		public Vector2Int MousePosBoard { get; private set; }
		// References
        [SerializeField] private LevelUI levelUI=null;
		private GameController gameController;
		private Spool spoolOver; // the Spool my mouse is over.
		private Spool spoolGrabbing; // the Spool we're holding and simulating a move for.

		// Getters (Public)
		public Board Board { get { return board; } }
		public BoardView BoardView { get { return boardView; } }
		public GameController GameController { get { return gameController; } }
        public LevelAddress MyAddress { get { return myAddress; } }
        public LevelUI LevelUI { get { return levelUI; } }
		public bool CanMakeAnyMove() {
			if (!IsPlaying) { return false; } // Not playing? Don't allow further movement. :)
			return true;
		}
        // Getters (Private)
        private InputController inputController { get { return InputController.Instance; } }
        private bool IsPlaying { get { return !IsAnimating && !IsLevelOver; } }
		private Vector2Int GetMousePosOffset(Spool bo) {
			if (bo == null) { return Vector2Int.zero; }
			return MousePosBoard - bo.BoardPos.ToVector2Int();
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
			ResetTimeSpentThisPlay();

			// Send in the clowns!
			RemakeModelAndViewFromData(_levelData.boardData);
		}
        private void OnDestroy() {
            // Make sure to increment how long we've spent in me!
            AddToTimeSpentTotal();
        }
        public void DestroySelf() {
            Destroy(this.gameObject);
        }


        private void RemakeModelAndViewFromData(BoardData bd) {
			// Destroy them first!
			DestroyBoardModelAndView ();
			// Make them afresh!
			board = new Board(bd);
            AddReasonableRandSpoolsToBoardIfNone(); // For rando layout generating!
			boardView = Instantiate (ResourcesHandler.Instance.spoolOut_boardView).GetComponent<BoardView>();
			boardView.Initialize (this, board);
            // Tell ppl!
            levelUI.OnBoardMade();
		}
		private void DestroyBoardModelAndView () {
			// Nullify the model (there's nothing to destroy).
			board = null;
			// Destroy view.
			if (boardView != null) {
				boardView.DestroySelf ();
				boardView = null;
			}
			// Nullify spoolOver and spoolGrabbing (the refs don't exist anymore).
			spoolOver = null;
			spoolGrabbing = null;
		}
        
        private void AddReasonableRandSpoolsToBoardIfNone() {
            // First, remember the basic empty boardData.
            //BoardData bd = board.SerializeAsData();
            //for (int i=0; i<99; i++) { // let's try 99 times.
            //    board.Debug_AddSpoolsIfNone(gameController.randGenParams);
            //    // Oh, this isn't a good enough layout? Try again.
            //    if (board.AreAnySpoolColorsSatisfied() || board.NumColors()==1) {
            //        board = new Board(bd);
            //    }
            //    else {
            //        break;
            //    }
            //}TODO: This.
        }
        
        
        // ----------------------------------------------------------------
        //  Animate In/Out
        // ----------------------------------------------------------------
        private const float animInOutDuration = 1.2f;
        private const float animInOutHeight = 1200;
        /** Animates the WHOLE LEVEL, including UI. From up offscreen to onscreen. */
        public void AnimateIn() {
            // I'm animating!
            IsAnimating = true;
            Vector3 posDefault = transform.localPosition;
            transform.localPosition += new Vector3(0, animInOutHeight, 0);
            LeanTween.moveLocal(gameObject, posDefault, animInOutDuration).setEaseInOutQuart().setOnComplete(OnCompleteAnimateIn);
        }
        /** Animates the WHOLE LEVEL, including UI. From onscreen to down offscreen. */
        public void AnimateOut() {
            IsAnimating = true;
            LeanTween.moveLocal(gameObject, new Vector3(0, -animInOutHeight, 0), animInOutDuration).setEaseInOutQuart().setOnComplete(OnCompleteAnimateOut);
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
		public void DependentUpdate() {
			if (board==null || board.spaces == null) { return; } // To prevent errors when compiling during runtime.

            UpdateMousePosBoard();
            UpdateSpoolOver();

            RegisterTouchInput();
            RegisterButtonInput();
            UpdateTimeSpentThisPlay();
		}

		private void UpdateMousePosBoard() {
            Vector2Int pmousePosBoard = MousePosBoard;
            MousePosBoard = GetMousePosBoard();
            if (MousePosBoard != pmousePosBoard) {
                OnMousePosBoardChanged();
            }
        }
        private Vector2Int GetMousePosBoard() {
			Vector2 mousePosScaled = Input.mousePosition/gameController.Canvas.scaleFactor;
			float canvasHeight = gameController.Canvas.GetComponent<RectTransform>().rect.height;
			mousePosScaled = new Vector2(mousePosScaled.x, canvasHeight-mousePosScaled.y); // convert to top-left space.
			mousePosScaled += new Vector2(-boardView.Pos.x, boardView.Pos.y); // Note: Idk why negative...
			int col = Mathf.FloorToInt(mousePosScaled.x / (float)boardView.UnitSize);
			int row = Mathf.FloorToInt(mousePosScaled.y / (float)boardView.UnitSize);
			return new Vector2Int(col,row);
		}
        
        private void OnMousePosBoardChanged() {
            if (spoolGrabbing != null) {
                TryToDragSpoolEndToPos(spoolGrabbing, MousePosBoard);
            }
        }
        
        
        // CANDO: a while loop. Keep trying to go until we can't.
        void TryToDragSpoolEndToPos(Spool spool, Vector2Int _pos) {
            Vector2Int endPos = spool.LastSpacePos;
            Vector2Int dir = Vector2Int.Sign(_pos.x-endPos.x, _pos.y-endPos.y);
            Vector2Int newPos = new Vector2Int(endPos.x+dir.x, endPos.y+dir.y);
            // This is the second-to-last space? REMOVE pathSpace.
            if (spool.IsSecondLastSpacePos(newPos)) {
                spool.RemovePathSpace();
            }
            // Otherwise, can we ADD this space to the path? Do!
            else if (BoardUtils.CanSpoolPathEnterSpace(board, spool, newPos)) {
                spool.AddPathSpace(newPos);
            }
        }
        
        
        
        

		private void UpdateSpoolOver() {
			// A) Can't make a move? Then no highlighting.
			if (!CanMakeAnyMove()) { SetSpoolOver(null); }
			// B) If we're GRABBING a Spool already, FORCE spoolOver to be THAT Spool!
			else if (spoolGrabbing != null) { SetSpoolOver(spoolGrabbing); }
			// C) Otherwise, use the one the mouse is over.
			else { SetSpoolOver(board.GetSpool(MousePosBoard)); }
        }
        private void SetSpoolOver(Spool spool) {
            Spool prevSpoolOver = spoolOver;
            spoolOver = spool;
			// It's changed!
			if (prevSpoolOver != spoolOver) {
				if (prevSpoolOver!=null && prevSpoolOver.IsInPlay) { boardView.Temp_GetSpoolView(prevSpoolOver).OnMouseOut(); }
				if (spoolOver != null) { boardView.Temp_GetSpoolView(spoolOver).OnMouseOver(); }
			}
		}

		private void RegisterButtonInput() {
			// DEBUG
            if (Input.GetKeyDown(KeyCode.O)) { LevelsManager.Instance.Debug_OrderLevelsAndCopyToClipboard(myAddress); }
			else if (Input.GetKeyDown(KeyCode.T)) { board.Debug_CopyLayoutToClipboard(true); }
            else if (Input.GetKeyDown(KeyCode.Y)) { board.Debug_CopyLayoutToClipboard(false); }
            else if (Input.GetKeyDown(KeyCode.C)) { board.Debug_CopyXMLToClipboard(true); }
            else if (Input.GetKeyDown(KeyCode.V)) { board.Debug_CopyXMLToClipboard(false); }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) { board.Debug_CopyXMLToClipboardWithDiff(1); }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) { board.Debug_CopyXMLToClipboardWithDiff(2); }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) { board.Debug_CopyXMLToClipboardWithDiff(3); }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) { board.Debug_CopyXMLToClipboardWithDiff(4); }
            else if (Input.GetKeyDown(KeyCode.Alpha5)) { board.Debug_CopyXMLToClipboardWithDiff(5); }
            else if (Input.GetKeyDown(KeyCode.Alpha6)) { board.Debug_CopyXMLToClipboardWithDiff(6); }
            else if (Input.GetKeyDown(KeyCode.Alpha7)) { board.Debug_CopyXMLToClipboardWithDiff(7); }
		}

		private void RegisterTouchInput() {
			if (inputController == null) { return; } // For compiling during runtime.

			if (inputController.IsTouchUp()) { OnTouchUp(); }
			if (inputController.IsTouchDown()) { OnTouchDown(); }
        }

		private void OnTouchDown() {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			if (spoolOver != null) {
                spoolOver.Truncate(MousePosBoard);
				SetSpoolGrabbing(spoolOver);
			}
		}
		private void OnTouchUp() {
			if (!CanMakeAnyMove()) { return; } // Dark Lord says no move? Then no.
			SetSpoolGrabbing(null);
		}
        
        public void ReleaseSpoolGrabbing() {
            if (spoolGrabbing != null) {
                SetSpoolGrabbing(null);
            }
        }
		private void SetSpoolGrabbing(Spool _spool) {
			if (spoolGrabbing != _spool) { // If it's changed...!
				Spool prevSpoolGrabbing = spoolGrabbing;
				spoolGrabbing = _spool;
                // Tell the Spools!
                if (prevSpoolGrabbing!=null) { boardView.Temp_GetSpoolView(prevSpoolGrabbing).OnStopGrabbing(); }
                if (spoolGrabbing!=null) { boardView.Temp_GetSpoolView(spoolGrabbing).OnStartGrabbing(); }
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
			//boardView.OnExecutedMove();
            // If our goals are satisfied, win!!
            if (board.AreGoalsSatisfied) {
                gameController.OnBoardGoalsSatisfied();
            }
            
            // In fail scenario, OR the level's over? Nullify spoolOver and spoolGrabbing.
            if (IsLevelOver) {
                SetSpoolOver(null);
                ReleaseSpoolGrabbing();
            }
        }
		public void OnWinLevel() {
			IsLevelOver = true;
            AddToTimeSpentTotal();
            IncrementNumWins();
		}
        
        
        private void AddToTimeSpentTotal() {
            string saveKey = SaveKeys.TimeSpentTotal(gameController.MyGameName(), MyAddress);
            float timeSpentTotal = SaveStorage.GetFloat(saveKey,0);
            SaveStorage.SetFloat(saveKey, timeSpentTotal+TimeSpentThisPlay);
            //print("Time spent total now: " + (timeSpentTotal+TimeSpentThisPlay));
            ResetTimeSpentThisPlay(); // we just used it! For safety, clear it out.
        }
        private void IncrementNumWins() {
            string saveKey = SaveKeys.NumWins(gameController.MyGameName(), MyAddress);
            int numWins = SaveStorage.GetInt(saveKey,0) + 1;
            SaveStorage.SetInt(saveKey, numWins);
            //print("Num wins now: " + numWins);
        }


        //  Debug
        // ----------------------------------------------------------------
        public void Debug_RemakeBoardAndViewFromArbitrarySnapshot(BoardData boardData) {
            // Treat this like a real move.
            RemakeModelAndViewFromData(boardData);
            //NumMovesMade ++;
            OnBoardMoveComplete();
        }

	}

}
