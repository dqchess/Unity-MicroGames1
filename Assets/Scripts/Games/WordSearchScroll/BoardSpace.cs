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

		public int Col { get { return boardPos.x; } }
		public int Row { get { return boardPos.y; } }


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
			this.transform.localPosition = board.BoardToPos(boardPos);
			myRectTransform.sizeDelta = new Vector2(board.UnitSize,board.UnitSize);

            float fillS = Random.Range(0.2f, 0.25f);
            float fillB = Random.Range(0.9f, 0.98f);
            //float fillB = Random.Range(0.01f, 0.06f);
			fillColor = new ColorHSB(30/360f, fillS, fillB).ToColor();
			i_backing.color = fillColor;

			// Default my letter to a period for testing.
			SetMyLetter('.');
		}

		public void SetMyLetter(char _myLetter) {
			myLetter = _myLetter;
			t_letter.text = myLetter.ToString();
		}


	}
}