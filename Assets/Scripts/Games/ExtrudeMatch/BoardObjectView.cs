using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrudeMatch {
	public class BoardObjectView : MonoBehaviour {
		// Components
		protected RectTransform myRectTransform; // Set in Start.
		// Properties
		private float _rotation; // so we can use the ease-ier (waka waka) system between -180 and 180 with minimal processing effort.
		private float _scale=1;
		// References
		private BoardObject myObject;
		private BoardView myBoardView;

		// Getters
		public BoardView MyBoardView { get { return myBoardView; } }
		public BoardObject MyBoardObject { get { return myObject; } }

		public Vector2 Pos {
			get { return this.transform.localPosition; }
			set {
				this.transform.localPosition = value;
				OnSetPos ();
			}
		}
		/** In degrees. */
		public float Rotation {
			get { return _rotation; }
			set {
				_rotation = value;
				this.transform.localEulerAngles = new Vector3 (this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, _rotation);
				OnSetRotation ();
			}
		}
		public float Scale {
			get { return _scale; }
			set {
				_scale = value;
				this.transform.localScale = Vector2.one * _scale;// * myBoardView.BaseObjectScaleConstant;
				OnSetScale ();
			}
		}
        protected Vector2 GetPosFromBO (BoardObject _bo) {
			return new Vector2 (myBoardView.BoardToX (_bo.Col), myBoardView.BoardToY (_bo.Row));
		}
		private float GetRotationFromBO (BoardObject _bo) {
			float returnValue = -90 * _bo.SideFacing;
			if (returnValue<-180) returnValue += 360;
			if (returnValue> 180) returnValue -= 360;
			if (Mathf.Abs (returnValue-Rotation) > 180) {
				if (Rotation<returnValue) { Rotation += 360; }
				else { Rotation -= 360; }
			}
			return returnValue;
		}

		virtual protected void OnSetPos () { }
		virtual protected void OnSetRotation () { }
		virtual protected void OnSetScale () { }


		// ----------------------------------------------------------------
		//  Initialize / Destroy
		// ----------------------------------------------------------------
		protected void InitializeAsBoardObjectView (BoardView _myBoardView, BoardObject _myObject) {
			myBoardView = _myBoardView;
			myObject = _myObject;
			myRectTransform = this.GetComponent<RectTransform>();

			// Parent me!
			this.transform.SetParent (myBoardView.transform);
			this.transform.localScale = Vector3.one;
			myRectTransform.sizeDelta = new Vector2(myBoardView.UnitSize, myBoardView.UnitSize); // NOTE: There's no code pattern to follow here with this! TileView totally does its own thing.

			// Start me in the right spot!
			Pos = GetPosFromBO(myObject);
			Rotation = GetRotationFromBO(myObject);
			Scale = 1;
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		/** This is called once all the animation is finished! */
		virtual public void UpdateVisualsPostMove () {
		}





	}
}
