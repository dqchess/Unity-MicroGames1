using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class Board : MonoBehaviour {
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Transform tf_boardSpaces=null;
		private BoardSpace[,] spaces;
		private WordManager wordManager;
		// Properties
		private float unitSize; // how big each board space is in pixels
		private int numCols,numRows;
		private Vector2 size;
		// References
		[SerializeField] private Level level;

		// Getters (Private)
		private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
		// Getters (Public)
		public float UnitSize { get { return unitSize; } }
		public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }

		public float BoardToX(float col) { return  (col+0.5f)*unitSize - size.x*0.5f; } // +0.5f to center.
		public float BoardToY(float row) { return -(row+0.5f)*unitSize + size.y*0.5f; } // +0.5f to center.
		public Vector2 BoardToPos(Vector2Int boardPos) { return new Vector2(BoardToX(boardPos.x), BoardToY(boardPos.y)); }
		//	public float XToBoard(float x) { return Pos.x + col*unitSize; }
		//	public float YToBoard(float y) { return Pos.y - row*unitSize; }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize () {
			wordManager = new WordManager();

			// TESTing
			numCols = 20;
			numRows = 20;

			// Determine unitSize and other board-specific visual stuff
			UpdatePosAndSize();

			// Make spaces!
			spaces = new BoardSpace[numCols,numRows];
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					spaces[i,j] = Instantiate(resourcesHandler.wordSearchScroll_boardSpace).GetComponent<BoardSpace>();
					spaces[i,j].Initialize (this, new Vector2Int(i,j));
				}
			}

			// Add words to board!
			string[] wordsToAdd = wordManager.GetRandomWords(20);
			AddWordsToBoard(wordsToAdd);
		}

		private void UpdatePosAndSize() {
			Rect r_availableArea = myRectTransform.rect;
			unitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
			unitSize *= 0.8f; //QQQ

			RectTransform rt_canvas = level.Canvas.GetComponent<RectTransform>();
			Vector2 canvasSize = rt_canvas.rect.size;
			size = new Vector2(unitSize*numCols, unitSize*numRows);
			myRectTransform.sizeDelta = size;
			float x = (canvasSize.x-size.x) * 0.5f;
			float y = (-canvasSize.y+size.y) * 0.5f;
			myRectTransform.anchoredPosition = new Vector2(x,y); // offset so I'm centered.
		}

		private void AddWordsToBoard(string[] words) {
			for (int i=0; i<words.Length; i++) {
				AddWordToBoard(words[i]);
			}
		}
		private void AddWordToBoard(string word) {
			// TODO: This.
		}







	}
}
