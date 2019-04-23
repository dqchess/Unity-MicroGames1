using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaTapOrder {
	public class TapSpace : MonoBehaviour {
		// Components
		[SerializeField] private Image i_body=null;
		[SerializeField] private Text t_myNumber=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private bool wasTapped;
		private int myNumber;
		// References
		private GameController gameControllerRef;

		// Getters
		public bool CanTapMe { get { return !wasTapped; } }
		public int MyNumber { get { return myNumber; } }



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize (GameController _gameControllerRef, RectTransform parentTransform, int _myNumber, Color bodyColor) {
			gameControllerRef = _gameControllerRef;
			SetMyNumber(_myNumber);

			// Parent jazz!
			this.transform.SetParent(parentTransform);
			this.transform.localEulerAngles = Vector3.zero;
			this.transform.localScale = Vector3.one;
			this.gameObject.name = "TapSpace " + (myNumber+1);

			wasTapped = false;
			t_myNumber.enabled = false;
			i_body.color = bodyColor;
		}

		public void SetMyNumber(int _myNumber) {
			myNumber = _myNumber;
			t_myNumber.text = myNumber.ToString();
		}


		public void SetSizePos(Vector2 size, Vector2 pos) {
			myRectTransform.sizeDelta = size;
			myRectTransform.anchoredPosition = pos;
		}


		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		public void OnClick() {
			gameControllerRef.OnTapSpaceClicked(this);
		}


		public void OnCorrectTap() {
			wasTapped = true;
			t_myNumber.enabled = true;
			t_myNumber.color = new Color(1,1,1, 0.5f);
		}
		public void OnIncorrectTap() {
			wasTapped = true;
			t_myNumber.enabled = true;
			t_myNumber.color = Color.red;
		}


	}
}
