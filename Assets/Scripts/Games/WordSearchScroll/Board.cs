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
		private List<WordHighlight> wordHighlights;
		// Properties
		private float unitSize; // how big each board space is in pixels
		private int numCols,numRows;
		private Vector2 size;
		private HashSet<string> wordsFound;
		// References
		[SerializeField] private Level level;

		// Getters (Public)
		public float UnitSize { get { return unitSize; } }
		public Transform tf_BoardSpaces { get { return tf_boardSpaces; } }
		public Vector2 Pos { get { return myRectTransform.anchoredPosition; } }
		public float BoardToX(float col) { return  (col+0.5f)*unitSize; } // +0.5f to center.
		public float BoardToY(float row) { return -(row+0.5f)*unitSize; } // +0.5f to center.
		public Vector2 BoardToPos(Vector2Int boardPos) { return new Vector2(BoardToX(boardPos.x), BoardToY(boardPos.y)); }
		public BoardSpace GetSpace(Vector2Int boardPos) {
			if (boardPos.x<0 || boardPos.y<0  ||  boardPos.x>=numCols || boardPos.y>=numRows) { return null; } // Outta bounds? Return null.
			return spaces[boardPos.x,boardPos.y];
		}

		// Getters (Private)
		private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
		private WordHighlight currentWordHighlight { get { return wordHighlights[wordHighlights.Count-1]; } } // Current girl's always the last one.
		private Vector2 mousePosRelative { get { return level.MousePosRelative; } }
		private Vector2Int mouseBoardPos { get { return level.MouseBoardPos; } }
		private bool DidFindWord(string _word) {
			return wordsFound.Contains(_word);
		}
		private Vector2Int RandBoardPos() {
			return new Vector2Int(Random.Range(0,numCols), Random.Range(0,numRows));
		}
		private Vector2Int RandDir() {
			int side = Random.Range(0,8);
			return MathUtils.GetDir(side);
		}
		private bool CanAddWord(string word, WordBoardPos wordPos) {
			// Make sure all the spaces are ok with this.
			char[] chars = word.ToCharArray();
			for (int i=0; i<chars.Length; i++) {
				Vector2Int letterPos = wordPos.pos + new Vector2Int(wordPos.dir.x*i, wordPos.dir.y*i);
				BoardSpace space = GetSpace(letterPos);
				if (space==null) { return false; } // Outta bounds? No way, Carmen.
				if (!space.CanSetMyLetter(chars[i])) { return false; } // Space can't be this letter? Na-ah, Sebastian.
			}
			return true; // Looks good!
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize () {
			wordManager = new WordManager();

			// TESTing
			numCols = 10;
			numRows = 10;

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
			string[] wordsToAdd = wordManager.GetRandomWords(1);
			AddWordsToBoard(wordsToAdd);

			wordsFound = new HashSet<string>();
			ResetAllWordHighlights();
		}

		private void UpdatePosAndSize() {
			Rect r_availableArea = myRectTransform.rect;
			unitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
//			unitSize *= 0.8f; //DEBUG

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
				TryAddWordToBoard(words[i]);
			}
		}
		private void TryAddWordToBoard(string word) {
			int count=0;
			while (true) {
				WordBoardPos randPos = new WordBoardPos(RandBoardPos(), RandDir(), word.Length);
				if (CanAddWord(word, randPos)) { // This works! Put it in!
					AddWordToBoard(word, randPos);
					break;
				}
				if (count++ > 100) {
					Debug.LogWarning("Oink! Can't find a place to add this word: " + word);
					break;
				}
			}
		}
		private void AddWordToBoard(string word, WordBoardPos wordPos) {
			char[] chars = word.ToCharArray();
			for (int i=0; i<chars.Length; i++) {
				Vector2Int letterPos = wordPos.pos + new Vector2Int(wordPos.dir.x*i, wordPos.dir.y*i);
				BoardSpace space = GetSpace(letterPos);
				space.SetMyLetter(chars[i]);
			}
		}

		private void ResetAllWordHighlights() {
			// Destroy 'em first.
			if (wordHighlights != null) {
				for (int i=0; i<wordHighlights.Count; i++) { Destroy(wordHighlights[i].gameObject); }
				wordHighlights = null;
			}
			// Make a new list with our first item!
			wordHighlights = new List<WordHighlight>();
			AddWordHighlight();
		}


		// ----------------------------------------------------------------
		//  Gameplay Doers
		// ----------------------------------------------------------------
		private void AddWordHighlight() {
			WordHighlight newObj = Instantiate(resourcesHandler.wordSearchScroll_wordHighlight).GetComponent<WordHighlight>();
			newObj.Initialize(this);
			wordHighlights.Add(newObj);
		}


		// ----------------------------------------------------------------
		//  Gameplay Events
		// ----------------------------------------------------------------
		public void OnTouchDown() {
			BoardSpace spaceOver = GetSpace(mouseBoardPos);
			if (spaceOver != null) {
				currentWordHighlight.Show(spaceOver);
			}
		}
		public void OnTouchUp() {
			// Is a word?!
			string word = currentWordHighlight.WordSpelling;
			if (!DidFindWord(word) && wordManager.IsRealWord(word)) {
				OnCurrentHighlightFoundWord();
			}
			// Not a word.
			else {
				currentWordHighlight.Hide();
			}
		}

		private void OnCurrentHighlightFoundWord() {
			string word = currentWordHighlight.WordSpelling;
			// Add the word to the list!
			wordsFound.Add(word);
			currentWordHighlight.Solidify();
			// Add a new highlight to be my new currentWordHighlight! :D
			AddWordHighlight();
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (currentWordHighlight.IsVisible) {
				currentWordHighlight.OnMousePosChanged(mousePosRelative);
			}
		}


	}


}
