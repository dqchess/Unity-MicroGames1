using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
	public class BoardObjectView : MonoBehaviour {
		// Components
		protected RectTransform myRectTransform; // Set in Start.
		// Properties
		private float _rotation; // so we can use the ease-ier (waka waka) system between -180 and 180 with minimal processing effort.
		private float rotation_to;
		private float rotation_from;
		private float _scale=1;
		protected float scale_to=1;
		protected float scale_from=1;
		private Vector2 pos_to; // visuals. Set right away; x and y ease to this.
		private Vector2 pos_from; // visuals. Set right away; x and y ease to this.
		// References
		private BoardObject myObject;
		private BoardObject mySimulatedMoveObject; // when we're lerping between two movement states, this is the "to" guy that's already completed its move!
		private BoardView myBoardView;

		// Getters (Public)
		public bool IsAnimating () { return !IsAtTargetPos() || !IsAtTargetRotation(); }
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
		// Getters (Private)
		private bool IsAtTargetPos () { return Vector2.Distance (Pos, pos_to) < 3f; }
		private bool IsAtTargetRotation () { return Mathf.Abs(Rotation-rotation_to) < 1f; }
		private bool IsAtTargetScale () { return Mathf.Abs(Scale-scale_to) < 0.05f; }
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
			GameUtils.ParentAndReset(this.gameObject, myBoardView.transform);
			myRectTransform.sizeDelta = new Vector2(myBoardView.UnitSize, myBoardView.UnitSize); // NOTE: There's no code pattern to follow here with this! TileView totally does its own thing.

			// Start me in the right spot!
			SetValues_From (_myObject);
			SetValues_To (_myObject); // For safety, default my "to" values to where I already am.
			GoToValues (myBoardView.ObjectsAnimationLocTarget);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		/** The moment after each move is logically executed, this is called for ALL BoardObjects. 
		This function will update target Pos/Rotation/Scale for Occupants, plus any extra stuff any extensions want to do too (like Player updating its pull arrows). */
		virtual public void UpdateVisualsPreMove () {
			SetValues_To (myObject);
		}
		/** This is called once all the animation is finished! */
		virtual public void UpdateVisualsPostMove () {
			SetValues_From (myObject);
			GoToValues (myBoardView.ObjectsAnimationLocTarget); // go 100% to the target values, of course! (This could be either 1 *or* 0.)
		}
		virtual public void SetValues_To (BoardObject _bo) {
			if (_bo == null) { return; }
			pos_to = GetPosFromBO (_bo);
			rotation_to = GetRotationFromBO (_bo);
			scale_to = 1;// GetScaleFromBO (_bo);
		}
		private void SetValues_From (BoardObject _bo) {
			if (_bo == null) { return; }
			pos_from = GetPosFromBO (_bo);
			rotation_from = GetRotationFromBO (_bo);
			scale_from = 1;// GetScaleFromBO (_bo);
		}
		public void SetMySimulatedMoveObject (BoardObject _object) {
			mySimulatedMoveObject = _object;
		}
		public void SetValues_To_ByMySimulatedMoveBoardObject () {
			SetValues_To (mySimulatedMoveObject);
		}
//		virtual public void SetValues_From_ByCurrentValues () {
//			pos_from = Pos;
//			rotation_from = Rotation;
//			scale_from = Scale;
//			//		SetValues_From (myObject);
//		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnGoToPrevBoardPos () {
			// No animating in an undo. It's simply more professional.
			GoToValues (myBoardView.ObjectsAnimationLocTarget);
		}
		virtual public void GoToValues (float lerpLoc) {
			//		if (mySimulatedMoveObject == null) { return; }
			Pos = Vector2.LerpUnclamped (pos_from, pos_to, lerpLoc);
			Rotation = Mathf.LerpUnclamped (rotation_from, rotation_to, lerpLoc);
			Scale = Mathf.LerpUnclamped (scale_from, scale_to, lerpLoc);
		}





	}
}