using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbacusToy {
    public class BoardView : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private Transform tf_boardSpaces=null;
        [SerializeField] private Transform tf_tiles=null;
        //[SerializeField] private Transform tf_walls=null;
        public Board MyBoard { get; private set; } // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
        public Level MyLevel { get; private set; }
        // Objects
        private BoardSpaceView[,] spaceViews;
        private List<BoardOccupantView> allOccupantViews; // includes EVERY single BoardOccupantView!
        private Board simMoveBoard; // for TOUCH INPUT feedback. Same story as the pre-move dragging in Threes!.
        // Properties
        public float UnitSize { get; private set; } // how big each board space is in pixels
        private bool areObjectsAnimating;
        private float animLoc; // eases to 0 or 1 while we're animating!
        private float animLocVel;
        private float animLocTarget; // either 0 (undoing a halfway animation) or 1 (going to the new, updated position).
        private MoveResults simMoveResult;
        private Vector2Int simMoveDir;

        // Getters (Public)
        public bool CanExecuteSimMove { get { return simMoveResult==MoveResults.Success; } }
        public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
        public RectTransform MyRectTransform { get { return myRectTransform; } }
        public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
        public bool AreObjectsAnimating { get { return areObjectsAnimating; } }
        public float ObjectsAnimationLocTarget { get { return animLocTarget; } }
        // Getters (Private)
        private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
        private int numCols { get { return MyBoard.NumCols; } }
        private int numRows { get { return MyBoard.NumRows; } }

        public float BoardToX(float col) { return  (col+0.5f)*UnitSize; } // +0.5f to center.
        public float BoardToY(float row) { return -(row+0.5f)*UnitSize; } // +0.5f to center.
        public float BoardToXGlobal(float col) { return BoardToX(col) + Pos.x; }
        public float BoardToYGlobal(float row) { return BoardToY(row) + Pos.y; }
        public Vector2 BoardToLocal(Vector2 pos) { return new Vector2(BoardToX(pos.x), BoardToY(pos.y)); }
        public Vector2 BoardToLocal(Vector2Int pos) { return BoardToLocal(pos.ToVector2()); }
        public Vector2 BoardToGlobal(Vector2Int pos) { return new Vector2(BoardToXGlobal(pos.x), BoardToYGlobal(pos.y)); }
        public Vector2 BoardToGlobal(BoardPos bp) { return new Vector2(BoardToXGlobal(bp.col), BoardToYGlobal(bp.row)); }
        //  public float XToBoard(float x) { return Pos.x + col*unitSize; }
        //  public float YToBoard(float y) { return Pos.y - row*unitSize; }
        
        private bool IsAnimLocAtTarget() {
            return Mathf.Abs (animLocTarget-animLoc) < 0.01f && Mathf.Abs(animLocVel)<0.01f;
        }
        private bool IsFinishedAnimating() {
            // We don't consider an animation finished if animLocTarget is between 0 or 1. (That's mid-move sliding territory.)
            if (animLocTarget!=0 || animLocTarget!=1) { return false; }
            return IsAnimLocAtTarget();
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize (Level _levelRef, Board _myBoard) {
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
                    spaceViews[i,j] = Instantiate(resourcesHandler.abacusToy_boardSpaceView).GetComponent<BoardSpaceView>();
                    spaceViews[i,j].Initialize (this, MyBoard.GetSpace(i,j));
                }
            }
            // Clear out all my lists!
            allOccupantViews = new List<BoardOccupantView>();
            //wallViews = new List<WallView>();
            foreach (Tile bo in MyBoard.tiles) { AddTileView (bo); }
            //foreach (Wall bo in myBoard.walls) { AddWallView (bo); }

            // Start off with all the right visual bells and whistles!
            UpdateAllViewsMoveEnd();
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
            TileView newObj = Instantiate(resourcesHandler.abacusToy_tileView).GetComponent<TileView>();
            newObj.Initialize (this, tf_tiles, data);
            allOccupantViews.Add (newObj);
            return newObj;
        }
        //WallView AddWallView(Wall data) {
        //    WallView newObj = Instantiate(resourcesHandler.abacusToy_wallView).GetComponent<WallView>();
        //    newObj.Initialize (this, tf_walls, data);
        //    wallViews.Add (newObj);
        //    return newObj;
        //}

        private void AddObjectView (BoardObject sourceObject) {
            if (sourceObject is Tile) { AddTileView (sourceObject as Tile); }
            else { Debug.LogError ("Trying to add BoardOccupantView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString()); }
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnOccupantViewDestroyedSelf (BoardOccupantView bo) {
            // Remove it from the list of views!
            allOccupantViews.Remove (bo);
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        private void UpdateAllViewsMoveStart() {
            AddViewsForAddedObjects();
            foreach (BoardOccupantView bo in allOccupantViews) {
                bo.UpdateVisualsPreMove();
            }
            // Note that destroyed Objects' views will be removed by the view in the UpdateVisualsMoveEnd.
            // Reset our BoardOccupantView' "from" values to where they *currently* are! Animate from there.
//          foreach (BoardOccupantView bov in allOccupantViews) {Note: Disabled this! Doesn't have much of an effect.
//              bov.SetValues_From_ByCurrentValues();
//          }
//          animLoc = 0;
            animLocVel = 0.08f;
            animLocTarget = 1;
            areObjectsAnimating = true;
            // Do this for safety.
            ApplyObjectsAnimationLoc();
        }
        public void UpdateAllViewsMoveEnd() {
            areObjectsAnimating = false;
            animLoc = 0; // reset this back to 0, no matter what the target value is.
            animLocVel = 0;
            for (int i=allOccupantViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
                allOccupantViews[i].UpdateVisualsPostMove();
            }
        }
        private void OnAnimLocReachTarget() {
            UpdateAllViewsMoveEnd();
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
            ApplyObjectsAnimationLoc ();
        }
        private void ClearSimMoveDirAndBoard() {
            simMoveDir = Vector2Int.zero;
            simMoveBoard = null;
            // Not at target loc? Say we're animating!
            if (!IsFinishedAnimating()) {
                areObjectsAnimating = true;
                //animLocVel = 0.08f;
                //animLocTarget = 0;
            }
        }
        public void OnCancelSimMove() {
            ClearSimMoveDirAndBoard();
            animLocTarget = 0;
        }
        public void OnBoardMoveComplete() {
            ClearSimMoveDirAndBoard();
            UpdateAllViewsMoveStart();
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
            // Now that the simulated Board has finished its move, we can set the "to" values for all my OccupantViews!
            foreach (BoardOccupantView bov in allOccupantViews) {
                bov.SetValues_To_ByMySimulatedMoveBoardObject();
            }
        }
        
        
        
        public TileView Temp_GetTileView(Tile _tile) {
            foreach (BoardOccupantView bov in allOccupantViews) {
                if (bov.MyBoardOccupant == _tile) {
                    return bov as TileView;
                }
            }
            return null;
        }
        




        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void FixedUpdate() {
            if (areObjectsAnimating) {
                animLocVel *= 0.75f;
                animLocVel += (animLocTarget-animLoc) * 0.05f;//AnimationEasing;
                animLoc += animLocVel;
                
    //            animLoc += (animLocTarget-animLoc) * 0.1f;//AnimationEasing;
                ApplyObjectsAnimationLoc ();
                if (IsAnimLocAtTarget()) {
                    OnAnimLocReachTarget();
                }
            }
        }
        private void ApplyObjectsAnimationLoc() {
            foreach (BoardOccupantView bov in allOccupantViews) {
                bov.GoToValues(animLoc);
            }
        }



    }
}


/*
	public class BoardView : MonoBehaviour {
		// Visual properties
		private float unitSize; // how big each board space is in pixels
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Transform tf_boardSpaces=null;
		// Objects
		private BoardSpaceView[,] spaceViews;
		public List<BoardOccupantView> allOccupantViews; // includes EVERY single BoardOccupantView!
		// References
		private Board myBoard; // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
		private Board simMoveBoard; // for TOUCH INPUT feedback. Same story as the pre-move dragging in Threes!.
		private Level levelRef;
		// Variable Properties
		private bool areObjectsAnimating;
		private float objectsAnimationLoc; // eases to 0 or 1 while we're animating!
		private float objectsAnimationLocTarget; // either 0 (undoing a halfway animation) or 1 (going to the new, updated position).
		private MoveResults simMoveResult;
		private Vector2Int simMoveDir;

		// Getters (Public)
		public Board MyBoard { get { return myBoard; } }
		public Level MyLevel { get { return levelRef; } }
		public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
		public float UnitSize { get { return unitSize; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
		public List<BoardOccupantView> AllOccupantViews { get { return allOccupantViews; } }
		public bool AreObjectsAnimating { get { return areObjectsAnimating; } }
//		public bool AreGoalsSatisfied { get { return myBoard.AreGoalsSatisfied; } }
		public float ObjectsAnimationLocTarget { get { return objectsAnimationLocTarget; } }
		// Getters (Private)
		private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
		private int numCols { get { return myBoard.NumCols; } }
		private int numRows { get { return myBoard.NumRows; } }

//		public float BoardToX(float col) { return Pos.x + (col+0.5f)*unitSize; } // +0.5f to center.
//		public float BoardToY(float row) { return Pos.y - (row+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToX(float col) { return  (col+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToY(float row) { return -(row+0.5f)*unitSize; } // +0.5f to center.
		//	public float XToBoard(float x) { return Pos.x + col*unitSize; }
		//	public float YToBoard(float y) { return Pos.y - row*unitSize; }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (Level _levelRef, Board _myBoard) {
			levelRef = _levelRef;
			myBoard = _myBoard;
			this.transform.SetParent (levelRef.transform);
			this.transform.localScale = Vector3.one;

			// Determine unitSize and other board-specific visual stuff
			UpdatePosAndSize();
            float parentWidth = levelRef.GetComponent<RectTransform>().rect.width;
            myRectTransform.sizeDelta = new Vector2(numCols,numRows) * UnitSize;
            myRectTransform.anchoredPosition = new Vector2((parentWidth-myRectTransform.rect.width)*0.5f,-200);

			// Make spaces!
			spaceViews = new BoardSpaceView[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaceViews[i,j] = Instantiate(resourcesHandler.abacusToy_boardSpaceView).GetComponent<BoardSpaceView>();
					spaceViews[i,j].Initialize (this, myBoard.GetSpace(i,j));
				}
			}
			// Clear out all my lists!
			allOccupantViews = new List<BoardOccupantView>();
			foreach (Tile bo in myBoard.tiles) { AddTileView (bo); }

			// Start off with all the right visual bells and whistles!
			UpdateAllViewsMoveEnd ();
		}
		public void DestroySelf () {
			// Destroy my entire GO.
			Destroy (this.gameObject);
		}

		private void UpdatePosAndSize() {
			Rect r_availableArea = myRectTransform.rect;
			unitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
		}

		TileView AddTileView (Tile data) {
			TileView newObj = Instantiate(resourcesHandler.abacusToy_tileView).GetComponent<TileView>();
			newObj.Initialize (this, data);
			allOccupantViews.Add (newObj);
			return newObj;
		}

		private void AddObjectView (BoardObject sourceObject) {
			if (sourceObject is Tile) { AddTileView (sourceObject as Tile); }
			else { Debug.LogError ("Trying to add BoardOccupantView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString()); }
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnOccupantViewDestroyedSelf(BoardOccupantView bo) {
			// Remove it from the list of views!
			allOccupantViews.Remove (bo);
		}



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void UpdateAllViewsMoveStart() {
			AddViewsForAddedObjects();
			UpdateBoardOccupantViewVisualsMoveStart ();
			// Note that destroyed Objects' views will be removed by the view in the UpdateVisualsMoveEnd.
			// Reset our BoardOccupantView' "from" values to where they *currently* are! Animate from there.
			foreach (BoardOccupantView bov in allOccupantViews) {
				bov.SetValues_From_ByCurrentValues ();
			}
			areObjectsAnimating = true;
			objectsAnimationLoc = 0;
			objectsAnimationLocTarget = 1;
			//		// Blindly confirm that at least someone is moving!
			//		areAnyOccupantsAnimating = true;
			//		doCheckIfOccupantsFinishedAnimating = true;
		}
		public void UpdateAllViewsMoveEnd() {
			areObjectsAnimating = false;
			objectsAnimationLoc = 0; // reset this back to 0, no matter what the target value is.
			for (int i=allOccupantViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
				allOccupantViews[i].UpdateVisualsPostMove ();
			}
		}
		private void UpdateBoardOccupantViewVisualsMoveStart() {
			foreach (BoardOccupantView bo in allOccupantViews) {
				bo.UpdateVisualsPreMove ();
			}
		}

		private void AddViewsForAddedObjects() {
			foreach (BoardObject bo in myBoard.objectsAddedThisMove) {
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
			objectsAnimationLocTarget = _simMovePercent;
			//objectsAnimationLocTarget *= 0.9f; // don't go all the way to 1.
			if (simMoveResult == MoveResults.Fail) {
				objectsAnimationLocTarget *= 0.1f; // Can't make the move?? Allow views to move a liiiitle, but barely (so user intuits it's illegal, and why).
			}
			// Keep the value locked to the target value.
			objectsAnimationLoc = objectsAnimationLocTarget;
			areObjectsAnimating = false; // ALWAYS say we're not animating here. If we swipe a few times really fast, we don't want competing animations.
			ApplyObjectsAnimationLoc ();
		}
		public void ClearSimMoveDirAndBoard() {
			simMoveDir = Vector2Int.zero;
            simMoveBoard = null;
			// Animate all views back to their original positions.
			areObjectsAnimating = true;
			objectsAnimationLocTarget = 0;
		}
		/** Clones our current Board, and applies the move to it! * /
		private void SetSimMoveDirAndBoard(BoardOccupant boToMove, Vector2Int _simMoveDir) {
			// If we accidentally used this function incorrectly, simply do the correct function instead.
			if (_simMoveDir == Vector2Int.zero) { ClearSimMoveDirAndBoard (); return; }
			// Oh, NO boToMove? Ok, no simulated move.
			if (boToMove == null) { ClearSimMoveDirAndBoard(); return; }
            
			simMoveDir = _simMoveDir;
			// Clone our current Board.
			simMoveBoard = myBoard.Clone();
			// Set BoardOCCUPANTs' references within the new, simulated Board! NOTE: We DON'T set any references for BoardObjects. Those don't move (plus, there's currently no way to find the matching references, as BoardObjects aren't added to spaces).
			foreach (BoardOccupantView bov in allOccupantViews) {
				BoardObject thisSimulatedMoveBO = BoardUtils.GetOccupantInClonedBoard(bov.MyBoardOccupant, simMoveBoard);
				bov.SetMySimulatedMoveObject(thisSimulatedMoveBO);
			}
			// Now actually simulate the move!
			simMoveResult = simMoveBoard.ExecuteMove(boToMove.BoardPos, simMoveDir);
			// Now that the simulated Board has finished its move, we can set the "to" values for all my OccupantViews!
			foreach (BoardOccupantView bov in allOccupantViews) {
				bov.SetValues_To_ByMySimulatedMoveBoardObject();
			}
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void FixedUpdate () {
			if (areObjectsAnimating) {
				objectsAnimationLoc += (objectsAnimationLocTarget-objectsAnimationLoc) / 2f;
				ApplyObjectsAnimationLoc();
				if (Mathf.Abs (objectsAnimationLocTarget-objectsAnimationLoc) < 0.01f) {
					UpdateAllViewsMoveEnd();
				}
			}
		}
		private void ApplyObjectsAnimationLoc() {
			foreach (BoardOccupantView bov in allOccupantViews) {
				bov.GoToValues(objectsAnimationLoc);
			}
		}
	}
}
*/

