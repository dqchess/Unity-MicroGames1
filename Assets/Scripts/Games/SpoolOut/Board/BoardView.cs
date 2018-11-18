using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
	public class BoardView : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private Transform tf_boardSpaces=null;
        [SerializeField] private Transform tf_spools=null;
        [SerializeField] private Transform tf_walls=null;
		// Objects
        public Board MyBoard { get; private set; } // this reference does NOT change during our existence! (If we undo a move, I'm destroyed completely and a new BoardView is made along with a new Board.)
        public Level MyLevel { get; private set; }
		private BoardSpaceView[,] spaceViews;
		private List<SpoolView> spoolViews;
        private List<WallView> wallViews;
        // Properties
        public float UnitSize { get; private set; } // how big each board space is in pixels
        // Variable Properties
        public bool IsInitializing { get; private set; }
        

        // Getters (Public)
        public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
        public RectTransform MyRectTransform { get { return myRectTransform; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
        public SpoolView Temp_GetSpoolView(Spool _spool) {
            foreach (SpoolView view in spoolViews) {
                if (view.MySpool == _spool) {
                    return view;
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
					spaceViews[i,j] = Instantiate(resourcesHandler.spoolOut_boardSpaceView).GetComponent<BoardSpaceView>();
					spaceViews[i,j].Initialize (this, MyBoard.GetSpace(i,j));
				}
			}
			// Clear out all my lists!
			spoolViews = new List<SpoolView>();
            wallViews = new List<WallView>();
            foreach (Spool bo in MyBoard.spools) { AddSpoolView (bo); }
            foreach (Wall bo in MyBoard.walls) { AddWallView (bo); }

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

        SpoolView AddSpoolView(Spool data) {
            SpoolView newObj = Instantiate(resourcesHandler.spoolOut_spoolView).GetComponent<SpoolView>();
            newObj.Initialize (this, tf_spools, data);
            spoolViews.Add (newObj);
            return newObj;
        }
        WallView AddWallView(Wall data) {
            WallView newObj = Instantiate(resourcesHandler.spoolOut_wallView).GetComponent<WallView>();
            newObj.Initialize (this, tf_walls, data);
            wallViews.Add (newObj);
            return newObj;
        }

		private void AddObjectView (BoardObject sourceObject) {
			if (sourceObject is Spool) { AddSpoolView (sourceObject as Spool); }
			else { Debug.LogError ("Trying to add BoardOccupantView from BoardObject, but no clause to handle this type! " + sourceObject.GetType().ToString()); }
		}
        

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void AddViewsForAddedObjects() {
			foreach (BoardObject bo in MyBoard.objectsAddedThisMove) {
				AddObjectView (bo);
			}
		}
        
        
        
        


	}
}

