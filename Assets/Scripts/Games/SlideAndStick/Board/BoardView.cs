using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardView : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private Transform tf_boardSpaces=null;
        [SerializeField] private Transform tf_tiles=null;
        [SerializeField] private Transform tf_walls=null;
        [SerializeField] private MergeSpotViews mergeSpotViews=null;
		// Objects
        public Board MyBoard { get; private set; } // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
        public Level MyLevel { get; private set; }
		private BoardSpaceView[,] spaceViews;
		private List<BoardOccupantView> allOccupantViews; // includes EVERY single BoardOccupantView!
        private List<WallView> wallViews;
        private Board simMoveBoard; // for TOUCH INPUT feedback. Same story as the pre-move dragging in Threes!.
        // Properties
        public float UnitSize { get; private set; } // how big each board space is in pixels
        // Variable Properties
        public bool IsInitializing { get; private set; }
        private MoveResults simMoveResult;
        private Vector2Int simMoveDir;
        // Animation Variables
        private bool areObjectsAnimating;
        private float animLoc; // eases to 0 or 1 while we're animating!
        private float animLocVel;
        private float animLocTarget; // either 0 (undoing a halfway animation) or 1 (going to the new, updated position).
        private bool isPostMoveNudge;
        private float postMoveNudgeAmount;
        private Vector2Int postMoveNudgeDir;
        // References
        private Tile lastTileGrabbed; // once set, never null again. Always the last known guy.
        

        // Getters (Public)
        public bool CanExecuteSimMove { get { return simMoveResult==MoveResults.Success; } }
        public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
        public RectTransform MyRectTransform { get { return myRectTransform; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
		public float ObjectsAnimationLocTarget { get { return animLocTarget; } }
        public TileView Temp_GetTileView(Tile _tile) {
            foreach (BoardOccupantView bov in allOccupantViews) {
                if (bov.MyBoardOccupant == _tile) {
                    return bov as TileView;
                }
            }
            return null;
        }
		// Getters (Private)
		private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
		private int numCols { get { return MyBoard.NumCols; } }
		private int numRows { get { return MyBoard.NumRows; } }

		public float BoardToX(float col) { return  (col+0.5f)*UnitSize; } // +0.5f to center.
		public float BoardToY(float row) { return -(row+0.5f)*UnitSize; } // +0.5f to center.
//		public float BoardToX(float col) { return Pos.x + (col+0.5f)*unitSize; } // +0.5f to center.
//		public float BoardToY(float row) { return Pos.y - (row+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToXGlobal(float col) { return BoardToX(col) + Pos.x; }
		public float BoardToYGlobal(float row) { return BoardToY(row) + Pos.y; }
        public Vector2 BoardToLocal(Vector2 pos) { return new Vector2(BoardToX(pos.x), BoardToY(pos.y)); }
        public Vector2 BoardToLocal(Vector2Int pos) { return BoardToLocal(pos.ToVector2()); }
		public Vector2 BoardToGlobal(Vector2Int pos) { return new Vector2(BoardToXGlobal(pos.x), BoardToYGlobal(pos.y)); }
		public Vector2 BoardToGlobal(BoardPos bp) { return new Vector2(BoardToXGlobal(bp.col), BoardToYGlobal(bp.row)); }
		//	public float XToBoard(float x) { return Pos.x + col*unitSize; }
		//	public float YToBoard(float y) { return Pos.y - row*unitSize; }
        
        private bool IsFinishedAnimating() {
            if (animLocTarget == 0) {
                return animLoc <= 0 && animLocVel <= 0;
            }
            else if (animLocTarget == 1) {
                return animLoc >= 1;
            }
            // animLocTarget between 0-1? That's mid-move sliding-- always consider anim NOT finished.
            else {
                return false;
            }
        }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (Level _levelRef, Board _myBoard) {
            IsInitializing = true;
        
			MyLevel = _levelRef;
			MyBoard = _myBoard;
			GameUtils.ParentAndReset(this.gameObject, MyLevel.transform);
			this.transform.SetAsFirstSibling(); // Put me behind the UI.

			// Determine unitSize and other board-specific visual stuff
			UpdatePosAndSize();

			// Make spaces!
			spaceViews = new BoardSpaceView[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaceViews[i,j] = Instantiate(resourcesHandler.slideAndStick_boardSpaceView).GetComponent<BoardSpaceView>();
					spaceViews[i,j].Initialize (this, MyBoard.GetSpace(i,j));
				}
			}
			// Clear out all my lists!
			allOccupantViews = new List<BoardOccupantView>();
            wallViews = new List<WallView>();
            foreach (Tile bo in MyBoard.tiles) { AddTileView (bo); }
            foreach (Wall bo in MyBoard.walls) { AddWallView (bo); }

			// Start off with all the right visual bells and whistles!
			UpdateAllViewsMoveEnd();
            
            IsInitializing = false;
		}
		public void DestroySelf() {
			// Destroy my entire GO.
			Destroy (this.gameObject);
		}

		private void UpdatePosAndSize() {
			// Calculate unitSize.
			Rect r_availableArea = myRectTransform.rect;
			UnitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
			// Update my rectTransform size.
			myRectTransform.sizeDelta = new Vector2(numCols*UnitSize, numRows*UnitSize);
			// Position me nice and horz centered!
			Vector2 parentSize = MyLevel.GetComponent<RectTransform>().rect.size;
			float posX = (parentSize.x-myRectTransform.rect.width)*0.5f;
			//float posY = -(parentSize.y*0.8f - myRectTransform.rect.height); // bottom-align me!
            float posY = -(parentSize.y-myRectTransform.rect.height) * 0.5f; // center-align me!
			myRectTransform.anchoredPosition = new Vector2(posX,posY);
		}

        TileView AddTileView(Tile data) {
            TileView newObj = Instantiate(resourcesHandler.slideAndStick_tileView).GetComponent<TileView>();
            newObj.Initialize (this, tf_tiles, data);
            allOccupantViews.Add (newObj);
            return newObj;
        }
        WallView AddWallView(Wall data) {
            WallView newObj = Instantiate(resourcesHandler.slideAndStick_wallView).GetComponent<WallView>();
            newObj.Initialize (this, tf_walls, data);
            wallViews.Add (newObj);
            return newObj;
        }

		private void AddObjectView (BoardObject sourceObject) {
			if (sourceObject is Tile) { AddTileView (sourceObject as Tile); }
			else { Debug.LogError ("Trying to add BoardOccupantView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString()); }
		}
        
        
        public void PreAnimateInFreshBoard() {
            if (MyBoard.DidRandGen) { return; } // For debug! Don't animate debug levels.//GameProperties.IsDebugFeatures && 
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    spaceViews[i,j].PreAnimateInFreshBoard();
                }
            }
            foreach (BoardOccupantView bov in allOccupantViews) { bov.PreAnimateInFreshBoard(); }
        }
        public void AnimateInFreshBoard() {
            if (MyBoard.DidRandGen) { return; } // For debug! Don't animate debug levels.//GameProperties.IsDebugFeatures && 
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    spaceViews[i,j].AnimateInFreshBoard();
                }
            }
            foreach (BoardOccupantView bov in allOccupantViews) { bov.AnimateInFreshBoard(); }
        }
        


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnOccupantViewDestroyedSelf (BoardOccupantView bo) {
			// Remove it from the list of views!
			allOccupantViews.Remove (bo);
		}
        public void OnUndoMove(BoardData bdPreUndo) {
            //Board boardFrom = new Board(bdPreUndo);
            //SetS
        }



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void UpdateAllViewsMoveStart() {
			AddViewsForAddedObjects();
            // Tell Occupants.
			foreach (BoardOccupantView bo in allOccupantViews) {
				bo.UpdateVisualsPreMove();
			}
			// Note that destroyed Objects' views will be removed by the view in the UpdateVisualsMoveEnd.
			// Reset our BoardOccupantView' "from" values to where they *currently* are! Animate from there.
//			foreach (BoardOccupantView bov in allOccupantViews) {Note: Disabled this! Doesn't have much of an effect.
//				bov.SetValues_From_ByCurrentValues();
//			}
            animLocVel = 0.05f;
            animLocTarget = 1;
            areObjectsAnimating = true;
			// Do this for safety.
			ApplyObjectsAnimationLoc();
		}
		public void UpdateAllViewsMoveEnd() {
            // Set anim values.
			areObjectsAnimating = false;
			animLoc = 0; // reset this back to 0, no matter what the target value is.
            animLocVel = 0;
            // Tell Spaces and Occupants.
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    spaceViews[i,j].UpdateVisualsPostMove();
                }
            }
			for (int i=allOccupantViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
				allOccupantViews[i].UpdateVisualsPostMove();
			}
            mergeSpotViews.UpdateVisualsPostMove();
            MyLevel.eh.OnUpdateAllViewsMoveEnd();
		}
        private void OnAnimLocReachTarget() {
            UpdateAllViewsMoveEnd();
            
            if (isPostMoveNudge) {
                DoPostMoveNudge();
            }
        }

		private void AddViewsForAddedObjects() {
			foreach (BoardObject bo in MyBoard.objectsAddedThisMove) {
				AddObjectView (bo);
			}
		}

		public void UpdateSimMove(BoardOccupant boToMove, SimMoveController smc) {
			UpdateSimMove(boToMove, smc.SimMoveDir, smc.SimMovePercent);
		}
		public void UpdateSimMove(BoardOccupant boToMove, Vector2Int _simMoveDir, float _simMovePercent) {
            if (simMoveDir != _simMoveDir) { // If the proposed simulated moveDir is *different* from the current one...!
                SetSimMoveDirAndBoard(boToMove, _simMoveDir);
            }
            else if (simMoveDir != Vector2Int.zero) {
                UpdateViewsTowardsSimMove(_simMovePercent);
            }
        }
        private void UpdateViewsTowardsSimMove(float _simMovePercent) {
            animLocTarget = _simMovePercent;
			if (!CanExecuteSimMove) {
                animLocTarget *= 0.1f; // Can't make the move?? Allow views to move a liiiitle, but barely (so user intuits it's illegal, and why).
            }
            // Keep the value locked to the target value.
            animLoc = animLocTarget;
            areObjectsAnimating = false; // ALWAYS say we're not animating here. If we swipe a few times really fast, we don't want competing animations.
            ApplyObjectsAnimationLoc();
        }
        private void ClearSimMoveDirAndBoard() {
            simMoveDir = Vector2Int.zero;
			simMoveBoard = null;
            // Not at target loc? Say we're animating!
            if (!IsFinishedAnimating()) {
    			areObjectsAnimating = true;
            }
        }
		public void OnCanceledMove() {
			ClearSimMoveDirAndBoard();
            animLocVel = -0.01f; // give vel just a teensy head-start.
            animLocTarget = 0;
            // Cancel postMoveNudge.
            isPostMoveNudge = false;
		}
		public void OnExecutedMove() {
            // We've grabbed at least one Tile at some point? Plan post-move nudge.
            if (lastTileGrabbed != null) {
                PlanPostMoveNudge();
            }
            // Clear our simMove (for now) and set start anim values.
			ClearSimMoveDirAndBoard();
			UpdateAllViewsMoveStart();
		}
        private void PlanPostMoveNudge() {
            isPostMoveNudge = true;
            postMoveNudgeDir = simMoveDir;
            postMoveNudgeAmount = Mathf.Abs(animLoc-1); // the farther loc is from 1, the higher the nudge amount.
        }
        private void DoPostMoveNudge() {
            if (lastTileGrabbed == null) { Debug.LogError("lastTileGrabbed is null! Hmmm."); return; } // Safety check.
            // Set this as a simMove!
            Vector2Int bp = lastTileGrabbed.BoardPos.ToVector2Int() + MyLevel.TileGrabbingClickBoardOffset;
            BoardOccupant bo = MyBoard.GetTile(bp);
            SetSimMoveDirAndBoard(bo, postMoveNudgeDir);
            // Prep ze animations.
            //UpdateAllViewsMoveStart();
            areObjectsAnimating = true;
            animLocVel = postMoveNudgeAmount * 0.02f; // we can scale the intensity of the nudge as we'd like here.
            animLoc = 0;
            animLocTarget = 0;
            isPostMoveNudge = false;
        }
        
        /** Clones our current Board, and applies the move to it! */
        private void SetSimMoveDirAndBoard(BoardOccupant boToMove, Vector2Int _simMoveDir) {
            // If we accidentally used this function incorrectly, simply do the correct function instead.
			if (_simMoveDir == Vector2Int.zero) { ClearSimMoveDirAndBoard (); return; } // TO DO: Clean this up. Have SimMoveController mandate calling my ClearSim().

			// Make sure we FINISH how things were supposed to look before we set new to/from states!
			UpdateAllViewsMoveEnd();
            
            simMoveDir = _simMoveDir;
            // Clone our current Board.
            simMoveBoard = MyBoard.Clone();
            // Set BoardOCCUPANTs' references within the new, simulated Board! NOTE: We DON'T set any references for BoardObjects. Those don't move (plus, there's currently no way to find the matching references, as BoardObjects aren't added to spaces).
            foreach (BoardOccupantView bov in allOccupantViews) {
                BoardObject thisSimulatedMoveBO = BoardUtils.GetOccupantInClonedBoard(bov.MyBoardOccupant, simMoveBoard);
                bov.SetMySimulatedMoveObject(thisSimulatedMoveBO);
            }
            // Now actually simulate the move!
            simMoveResult = simMoveBoard.ExecuteMove(boToMove.BoardPos, simMoveDir);
            // Now that the simulated Board has finished its move, we can set the "to" values for all SpaceViews/OccupantViews!
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    spaceViews[i,j].SetValues_To(simMoveBoard.spaces[i,j]);
                }
            }
            foreach (BoardOccupantView bov in allOccupantViews) {
                bov.SetValues_To_ByMySimulatedMoveBoardObject();
			}
            mergeSpotViews.SetValues_To(simMoveBoard);
        }
        
        
        public void OnSetTileGrabbing(Tile _tileGrabbing, Tile _prevTileGrabbing) {
            MoveTileViewToTop(_tileGrabbing); // move the TileView on TOP of all others!
            
            if (_tileGrabbing != null) {
                lastTileGrabbed = _tileGrabbing;
            }
        }
        private void MoveTileViewToTop(Tile tile) {
            if (tile == null) { return; } // Check for da obvious.
            TileView tileView = Temp_GetTileView(tile);
            if (tileView != null) { tileView.transform.SetAsLastSibling(); }
            else { Debug.LogError("Whoa, MoveTileViewToTop passed in Tile that doesn't have corresponding TileView??"); }
        }




		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
        public void DependentFixedUpdate() {
			//print(Time.frameCount + " animating: " + areObjectsAnimating + ", animLocTarget: " + animLocTarget + ", animLoc: " + animLoc + ", animLocVel: " + animLocVel + ", simMoveDir: " + simMoveDir);
			if (areObjectsAnimating) {
                animLocVel *= 0.98f;
				animLocVel += (animLocTarget-animLoc) * 0.018f;
                animLoc += animLocVel;
                
				ApplyObjectsAnimationLoc();
				if (IsFinishedAnimating()) {
					OnAnimLocReachTarget();
				}
			}
		}
		private void ApplyObjectsAnimationLoc() {
            // BoardSpaceViews!
            for (int i=0; i<numCols; i++) {
                for (int j=0; j<numRows; j++) {
                    spaceViews[i,j].GoToValues(animLoc);
                }
            }
            // BoardOccupantViews!
			foreach (BoardOccupantView bov in allOccupantViews) {
				bov.GoToValues(animLoc);
			}
            // MergeSpotViews!
            mergeSpotViews.GoToValues(animLoc);
		}



	}
}

