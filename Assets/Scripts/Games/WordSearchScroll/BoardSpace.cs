using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WordSearchScroll {
	public class BoardSpace : MonoBehaviour {
		// Components
		[SerializeField] private Image i_backing;
		[SerializeField] private RectTransform myRectTransform;
		[SerializeField] private TextMeshProUGUI t_letter;
		// Properties
		private Color fillColor;
		private char myLetter;
		private Vector2Int boardPos;
		// References
		private Board board;
		private List<BoardWord> myWords; // a reference to all the words I'm a part of. For debug testing.

		// Getters (Private)
		private bool HasLetter() { return WordManager.IsCharInAlphabet(myLetter); }
		// Getters (Public)
		public int Col { get { return boardPos.x; } }
		public int Row { get { return boardPos.y; } }
		public char MyLetter { get { return myLetter; } }
		public Vector2 Pos { get { return board.BoardToPos(BoardPos); } }
		public Vector2Int BoardPos { get { return boardPos; } }
		public bool CanSetMyLetter(char _char=' ') {
			// We can set my letter if I DON'T have one yet, OR if it's the same as what we wanna set it to!
			return !HasLetter() || myLetter==_char;
		}

		// Debug
		public void Debug_PrintMyWords() {
			if (myWords.Count==0) { print("No words at this space."); return; }
			string str = "Words: ";
			foreach (BoardWord word in myWords) {
				str += word.word + ", ";
			}
			print(str);
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (Board _board, Vector2Int _boardPos) {
			board = _board;
			boardPos = _boardPos;

			// Parent me to my boooard!
			GameUtils.ParentAndReset(this.gameObject, board.tf_BoardSpaces);
			this.gameObject.name = "BoardSpace_" + Col + "," + Row;

			// Size/position me right!
			myRectTransform.anchoredPosition = board.BoardToPos(boardPos);
			myRectTransform.sizeDelta = new Vector2(board.UnitSize,board.UnitSize);

//            float fillS = Random.Range(0.04f, 0.06f);
			float fillS = 0.04f;
            float fillB = Random.Range(0.9f, 1f);
            //float fillB = Random.Range(0.01f, 0.06f);
			fillColor = new ColorHSB(30/360f, fillS, fillB).ToColor();
			i_backing.color = fillColor;

			myWords = new List<BoardWord>();

			// Default my letter to a period for testing.
			SetMyLetter('-', null);
		}

		public void SetMyLetter(char _myLetter, BoardWord wordFrom) {
			myLetter = _myLetter;
			t_letter.text = myLetter.ToString();
			if (wordFrom != null) {
				myWords.Add(wordFrom);
			}

//			if (myWords.Count == 0) {
//				t_letter.color = new Color(0,0,0, 0.3f); // DEBUG TEST
//			}
//			else {
			t_letter.color = new Color(0,0,0, 0.8f);
//			}
		}


	}
}