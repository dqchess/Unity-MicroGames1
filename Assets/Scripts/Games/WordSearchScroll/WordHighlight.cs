using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WordSearchScroll {
	public class WordHighlight : MonoBehaviour {
		// Components
		[SerializeField] private Image i_body;
		// Properties
		private string wordSpelling; // updated when we change mySpaces.
		// References
		private Board board;
		private List<BoardSpace> mySpaces;

		// Getters (Public)
		public bool IsVisible { get { return i_body.enabled; } }
		public string WordSpelling { get { return wordSpelling; } }
		// Getters (Private)
		private BoardSpace startSpace { get { return mySpaces[0]; } }
		private BoardSpace endSpace { get { return mySpaces[mySpaces.Count-1]; } }



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Board _board) {
			board = _board;
			GameUtils.ParentAndReset(this.gameObject, board.transform);

			// Gimme a random color!
			float h = Random.Range(0f,1f);
			i_body.color = new ColorHSB(h, 0.4f, 1f, 0.4f).ToColor();

			Hide();
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void Hide() {
			i_body.enabled = false;
		}
		public void Show(BoardSpace _startSpace) {
			i_body.enabled = true;
			mySpaces = new List<BoardSpace>();
			mySpaces.Add(_startSpace);
			UpdateEndSpace(_startSpace); // also start with the endSpace the same as the startSpace.
		}
		public void OnMousePosChanged(Vector2 mousePosRelative) {
			// HACK. Gotta fix up/clarify the anchoring business.
			mousePosRelative = new Vector2(mousePosRelative.x, -mousePosRelative.y);

			// Snap the angle to 45-degrees.
			float angle = LineUtils.GetAngle_Radians(startSpace.Pos, mousePosRelative);
			angle = MathUtils.SnapAngle(angle, Mathf.PI*0.25f); // snap to 45-degrees (in radians tho).
			float dist = Vector2.Distance(startSpace.Pos, mousePosRelative);
			int distBoard = Mathf.FloorToInt(dist/board.UnitSize);

			// What should the new end space be??
			Vector2Int offsetFromStart = new Vector2Int(Mathf.Cos(angle)*distBoard, -Mathf.Sin(angle)*distBoard);
			BoardSpace newEndSpace = board.GetSpace(startSpace.BoardPos + offsetFromStart);

			// Is this a neeeewww endSpace??
			if (newEndSpace!=null && newEndSpace != endSpace) {
				UpdateEndSpace(newEndSpace);
			}
		}

		private void UpdateEndSpace(BoardSpace desiredEndSpace) {
			if (desiredEndSpace==null) { return; } // Not on the board?? Do nothin'.

			// What dir is this space relative to startSpace?
			Vector2Int dir = MathUtils.GetDir(startSpace.BoardPos.ToVector2(), desiredEndSpace.BoardPos.ToVector2());
			// How many spaces are we traveling?
			int numSpaces = MathUtils.ChebyshevDistance(startSpace.BoardPos, desiredEndSpace.BoardPos) + 1; // +1 to include the startSpace.

			// Remake my spaces, yo!
			BoardSpace _startSpace = startSpace; // rememmer this.
			mySpaces.Clear();
			mySpaces.Add(_startSpace); // ok, start with the startSpace again.
			for (int i=1; i<numSpaces; i++) { // Add to the list, starting at the SECOND space (we already added the start space)!
				Vector2Int bp = new Vector2Int(startSpace.BoardPos.x+dir.x*i, startSpace.BoardPos.y-dir.y*i); // ooooohh HACK-y minus y.
				mySpaces.Add(board.GetSpace(bp));
			}
			OnMySpacesChanged();

			Vector2 startPos = startSpace.Pos;
			Vector2 endPos = desiredEndSpace.Pos;

			Vector2 center = Vector2.Lerp(startPos,endPos, 0.5f);
			float rotation = LineUtils.GetAngle_Degrees(startPos, endPos);
			float dist = Vector2.Distance(startPos,endPos);//numSpaces*board.UnitSize;//
			i_body.rectTransform.anchoredPosition = center;
			i_body.rectTransform.localEulerAngles = new Vector3(0,0,rotation);
			float lineLength = dist + board.UnitSize; // beef up the line length by a full board unit.
			GameUtils.SizeUIGraphic(i_body, lineLength,board.UnitSize);
		}

		public void Solidify() {
			// Temp: Make color darker.
			i_body.color = Color.Lerp(i_body.color, Color.black, 0.2f);
		}

		private void OnMySpacesChanged() {
			wordSpelling = "";
			foreach (BoardSpace space in mySpaces) {
				wordSpelling += space.MyLetter.ToString();
			}
//			print("Word spelling: " + wordSpelling); // TEMP DEBUG
		}

	}
}