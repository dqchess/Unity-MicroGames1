using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class Board : MonoBehaviour {
		// Constants
		private const int MinWordLength = 3; // don't pick or allow words smaller than this.
		// Components
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private Transform tf_boardSpaces=null;
		private BoardSpace[,] spaces;
		private WordManager wordManager;
		private List<WordHighlight> wordHighlights;
		// Properties
		private float unitSize; // how big each board space is in pixels
		private float scrollAmount; // in board units.
		private int numCols,numRows;
//		private string[,] wordsPlanned; // TEMP, not infinite. For testing.
		private Dictionary<string,BoardWord> boardWords; // ALL of the words we've added! Key is the word string.
		private List<BoardWord> wordsVisible; // just the ones we can see, from the ones we've added.
		private Vector2 posTarget;
		private Vector2 size;
		private HashSet<string> wordsFound;
		// References
		[SerializeField] private Level level;
		[SerializeField] private LevelUI ui;

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

//		private void OnGUI() {
//			if (wordsVisible==null) { return; }
//			for (int i=0; i<wordsVisible.Count; i++) {
//				GUI.contentColor = wordsVisible[i].didFindMe ? new Color(1,1,1, 0.3f) : Color.white;
//				GUI.Label(new Rect(8,10+i*16, 800,40), wordsVisible[i].word);
//			}
//		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize () {
			wordManager = new WordManager();

			// TESTing
			numCols = 80;
			numRows = 12;

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

//			wordsPlanned = new string[1000,numRows];

			// Add words to board!
			boardWords = new Dictionary<string,BoardWord>();
			int numWordsToAdd = Mathf.CeilToInt(numCols*numRows / 1f);//hacky TEST 6f); // 1 word for every 6 spaces.
			string[] wordsToAdd = wordManager.GetRandomWords(numWordsToAdd, MinWordLength);
			AddWordsToBoard(wordsToAdd);
			FillEmptySpacesWithRandLetters();

			wordsFound = new HashSet<string>();
			ResetAllWordHighlights();
			UpdateWordsVisible();
		}

		private void UpdatePosAndSize() {
			Rect r_availableArea = myRectTransform.rect;
//			unitSize = Mathf.Min(r_availableArea.size.x/(float)(numCols), r_availableArea.size.y/(float)(numRows));
			unitSize = r_availableArea.size.y/(float)(numRows) * 0.9f; // TEMP TESTING

			size = new Vector2(unitSize*numCols, unitSize*numRows);
			myRectTransform.sizeDelta = size;
			scrollAmount = 0;
			UpdatePosTarget();
		}
		private void UpdatePosTarget() {
//			float xFrom = myRectTransform.anchoredPosition.x;
			Vector2 canvasSize = level.CanvasSize;
			posTarget = new Vector2(-scrollAmount*unitSize,//canvasSize.x*0.5f 
				//									(-canvasSize.y+size.y) * 0.5f); // offset so I'm centered.
				-canvasSize.y + size.y + 50); // bottom-aligned

//			myRectTransform.anchoredPosition = new Vector2(xFrom,posTarget.y);
//			LeanTween.value(this.gameObject, SetXPos, xFrom,posTarget.x, 1.2f).setEaseInOutQuart();
			UpdateWordsVisible();
		}
		private void SetXPos(float _x) {
			myRectTransform.anchoredPosition = new Vector2(_x, myRectTransform.anchoredPosition.y);
		}

		private void FillEmptySpacesWithRandLetters() {
			for (int i=0; i<numCols; i++) {
				for (int j=0; j<numRows; j++) {
					BoardSpace space = spaces[i,j];
					if (space.CanSetMyLetter()) {
						space.SetMyLetter(WordManager.RandAlphabetChar(), null);
					}
				}
			}
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
			// Add it to boardWords!
			BoardWord bw = new BoardWord(word, wordPos);
			boardWords[word] = bw;
			// Pop it in da board.
			char[] chars = word.ToCharArray();
			for (int i=0; i<chars.Length; i++) {
				Vector2Int letterPos = wordPos.pos + new Vector2Int(wordPos.dir.x*i, wordPos.dir.y*i);
				BoardSpace space = GetSpace(letterPos);
				space.SetMyLetter(chars[i], bw);
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
				// DEBUG
				spaceOver.Debug_PrintMyWords();
			}
		}
		public void OnTouchUp() {
			// Is a word?!
			string word = currentWordHighlight.WordSpelling;
			if (CanSubmitWord(word)) {
				OnCurrentHighlightFoundWord();
			}
			// Not a word.
			else {
				currentWordHighlight.Hide();
			}
		}
		private bool CanSubmitWord(string word) {
			if (DidFindWord(word)) { return false; }
			if (word.Length < MinWordLength) { return false; }
			if (!wordManager.IsRealWord(word)) { return false; }
			return true;
		}

		private void OnCurrentHighlightFoundWord() {
			string word = currentWordHighlight.WordSpelling;
			// Add the word to the list!
			wordsFound.Add(word);
			currentWordHighlight.Solidify();
			// Tell the BoardWord!
			if (boardWords.ContainsKey(word)) {
				boardWords[word].didFindMe = true;
			}
			// Add a new highlight to be my new currentWordHighlight! :D
			AddWordHighlight();
			// Scroll!!
			scrollAmount += 0.4f;
			UpdatePosTarget();
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (currentWordHighlight.IsVisible) {
				currentWordHighlight.OnMousePosChanged(mousePosRelative);
			}

			// TEST
			if (myRectTransform.anchoredPosition != posTarget) {
				myRectTransform.anchoredPosition += (posTarget-myRectTransform.anchoredPosition) / 12f;
				if (Vector2.Distance(myRectTransform.anchoredPosition,posTarget) <= 0.1f) {
					myRectTransform.anchoredPosition = posTarget;
				}
			}

			// DEBUG TESTTT
			if (Input.GetKey(KeyCode.LeftArrow)) {
				scrollAmount -= 0.1f;
				UpdatePosTarget();
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				scrollAmount += 0.1f;
				UpdatePosTarget();
			}
		}


		private void UpdateWordsVisible() {
			if (boardWords==null) { return; } // Safety check.
			wordsVisible = new List<BoardWord>();
			int colMin = Mathf.FloorToInt(-posTarget.x/unitSize);
			int colMax = colMin + Mathf.FloorToInt(level.CanvasSize.x/unitSize);
			foreach (BoardWord bw in boardWords.Values) {
				if (bw.bpos.pos.x<colMin || bw.bpos.pos.x>colMax) { continue; } // Quick check: First pos is outta bounds.
				bool isWordVisible = true; // I'll say otherwise next.
				for (int l=0; l<bw.word.Length; l++) {
					float thisCol = bw.bpos.pos.x + bw.bpos.dir.x*l;
					if (thisCol<colMin || thisCol>colMax) {
						isWordVisible = false; // ahh, it's only PARTIALLY visible. Nah.
						break;
					}
				}
				if (!isWordVisible) { continue; } // We found it not fully visible, so skip it.
				// This girl is totally in the visible section!!
				wordsVisible.Add(bw);
			}
			// Tell the UI!
			ui.OnSetWordsVisible(wordsVisible);
		}


	}


	public class BoardWord {
		public bool didFindMe=false;
		public string word;
		public WordBoardPos bpos;
		public BoardWord(string word, WordBoardPos bpos) {
			this.word = word;
			this.bpos = bpos;
		}
	}


}
