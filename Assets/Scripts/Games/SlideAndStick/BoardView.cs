using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardView : MonoBehaviour {
		// Visual properties
		private float unitSize; // how big each board space is in pixels
//		private Rect viewRect; // contains my pos and size.
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Transform tf_boardSpaces=null;
		// Objects
		private BoardSpaceView[,] spaceViews;
        //private List<BoardObjectView> allObjectViews; // includes EVERY single BoardObjectView!
        private List<TileView> tileViews;
		// References
		private Board myBoard; // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
		private BoardController levelRef;

		// Getters
		public Board MyBoard { get { return myBoard; } }
		public BoardController MyLevel { get { return levelRef; } }
//		public Rect ViewRect { get { return viewRect; } }
//		public Vector2 Pos { get { return viewRect.position; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
		//public List<BoardObjectView> AllObjectViews { get { return tileViews; } }
		public float UnitSize { get { return unitSize; } }

		private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
		private int numCols { get { return myBoard.NumCols; } }
		private int numRows { get { return myBoard.NumRows; } }
		public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }

//		public float BoardToX(float col) { return Pos.x + (col+0.5f)*unitSize; } // +0.5f to center.
//		public float BoardToY(float row) { return Pos.y - (row+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToX(float col) { return  (col+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToY(float row) { return -(row+0.5f)*unitSize; } // +0.5f to center.
		//	public float XToBoard(float x) { return Pos.x + col*unitSize; }
		//	public float YToBoard(float y) { return Pos.y - row*unitSize; }

		private Rect TEMP_availableArea; // TEMP DEBUG
//		private void OnDrawGizmos () {
//			Gizmos.color = Color.yellow;
//			Gizmos.DrawWireCube (TEMP_availableArea.center*GameVisualProperties.WORLD_SCALE, TEMP_availableArea.size*GameVisualProperties.WORLD_SCALE);
//		}

		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (BoardController _levelRef, Board _myBoard) {
			levelRef = _levelRef;
			myBoard = _myBoard;
			this.transform.SetParent (levelRef.transform);
			this.transform.localScale = Vector3.one;
            // Position me nice and horz centered!
            float parentWidth = levelRef.GetComponent<RectTransform>().rect.width;
            myRectTransform.anchoredPosition = new Vector2((parentWidth-myRectTransform.rect.width)*0.5f,-200);

			// Determine unitSize and other board-specific visual stuff
			UpdatePosAndSize();

			// Make spaces!
			spaceViews = new BoardSpaceView[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaceViews[i,j] = Instantiate(resourcesHandler.slideAndStick_boardSpaceView).GetComponent<BoardSpaceView>();
					spaceViews[i,j].Initialize (this, myBoard.GetSpace(i,j));
				}
			}
			// Clear out all my lists!
			tileViews = new List<TileView>();
			foreach (Tile bo in myBoard.tiles) { AddTileView (bo); }

			// Look right right away!
            AnimateInNewTilesFromSource(null);
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
			TileView newObj = Instantiate(resourcesHandler.slideAndStick_tileView).GetComponent<TileView>();
			newObj.Initialize (this, data);
			tileViews.Add (newObj);
			return newObj;
		}

		private void AddObjectView (BoardObject sourceObject) {
			if (sourceObject is Tile) { AddTileView (sourceObject as Tile); }
			else { Debug.LogError ("Trying to add BoardObjectView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString()); }
		}



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
        public void AnimateInNewTilesFromSource(Tile sourceTile) {
            foreach (Tile t in myBoard.tilesRecentlyAdded) {
                TileView newTV = AddTileView (t);
                newTV.AnimateIn(sourceTile);
            }
            // Clear out the list! We've used 'em.
            myBoard.tilesRecentlyAdded.Clear();
        }
        public void AnimateOutRemovedTiles(RemovalTypes removalType) {
            for (int i=tileViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
				if (!tileViews[i].MyBoardObject.IsInPlay) {
                    // It'll animate out and destroy itself.
                    tileViews[i].AnimateOut(removalType);
					// Remove it from the list of views.
					tileViews.RemoveAt(i);
				}
			}
		}


		private void RemoveViewsNotInPlay() {
			for (int i=tileViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
				if (!tileViews[i].MyBoardObject.IsInPlay) {
					// Destroy the object.
					GameObject.Destroy (tileViews[i].gameObject);
					// Remove it from the list of views.
					tileViews.RemoveAt(i);
				}
			}
		}



	}
}


//		public void UpdateViewsPostMove () {
//			RemoveViewsNotInPlay();
//			AddViewsForAddedObjects();
//			for (int i=allObjectViews.Count-1; i>=0; --i) { // Go through backwards, as objects can be removed from the list as we go!
//				allObjectViews[i].UpdateVisualsPostMove ();
//			}
//		}
//		private void AddViewsForAddedObjects () {
//			foreach (BoardObject bo in myBoard.objectsAddedThisMove) {
//				AddObjectView (bo);
//			}
//			// Clear out the list! We've used 'em.
//			myBoard.objectsAddedThisMove.Clear();
//		}

//		/** Very inefficient. Just temporary. */
//		public BoardOccupantView TEMP_GetOccupantView (BoardOccupant _occupant) {
//			foreach (BoardObjectView objectView in tileViews) {
//				if (objectView is BoardOccupantView) {
//					if (objectView.MyBoardObject == _occupant) {
//						return objectView as BoardOccupantView;
//					}
////				BoardOccupantView occupantView = objectView as BoardOccupantView;
//		}
//	}
//	return null; // oops.
//}
///** Very inefficient. Just temporary. */
//public BoardObjectView TEMP_GetObjectView (BoardObject bo) {
//	foreach (BoardObjectView objectView in tileViews) {
//		if (objectView.MyBoardObject == bo) {
//			return objectView;
//		}
//	}
//	return null; // oops.
//}
/*
		private void UpdatePosAndSize() {
			Rect r_availableArea = myRectTransform.rect;
			//TO DO: Clean this up
//			const float minGapBottom = 40;
//			const float minGapTop = 180;
//			const float minGapLeft = 40;
//			const float minGapRight = 40;
//			Rect r_availableArea = new Rect(minGapLeft,minGapBottom, screenSize.x-minGapLeft-minGapRight,screenSize.y-minGapBottom-minGapTop);
			unitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
//			// Position us real good!
//			viewRect = new Rect();
//			viewRect.size = new Vector2(unitSize*numCols, unitSize*numRows);
//			viewRect.position = new Vector2(r_availableArea.position.x,r_availableArea.yMax);
//			viewRect.position += new Vector2((r_availableArea.size.x-viewRect.size.x)*0.5f, -(r_availableArea.size.y-viewRect.size.y)); // horizontally center us, dawg!
		}
		*/
