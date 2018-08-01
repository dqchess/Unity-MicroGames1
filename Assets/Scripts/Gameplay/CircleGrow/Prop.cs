using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	abstract public class Prop : MonoBehaviour {
		// Components
		[SerializeField] protected Image i_body;
		[SerializeField] protected RectTransform myRectTransform=null;
		// Properties
		private float moveSpeed=1f; // for MOVING Props.
		private float moveOscVal; // for MOVING Props.
		private float rotateSpeed=0f; // for ROTATING Props.
		private Vector2 posA,posB; // for MOVING Props.
		// References
		protected Level myLevel;

		// Getters (Public)
		public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		public float Rotation {
			get { return myRectTransform.localEulerAngles.z; }
			set { myRectTransform.localEulerAngles = new Vector3(myRectTransform.localEulerAngles.x, myRectTransform.localEulerAngles.y, value); }
		}
		// Getters (Protected)
		protected Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
		virtual protected bool MayMove() {
			return moveSpeed != 0;
		}
		virtual protected bool MayRotate() {
			return rotateSpeed != 0;
		}
		// Getters (Private)
		private float timeScale { get { return Time.timeScale; } }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		protected void BaseInitialize(Level _myLevel, Transform tf_parent, Vector2 _pos, Vector2 _size) {
			myLevel = _myLevel;

			this.transform.SetParent(tf_parent);
			this.transform.localScale = Vector3.one;
			this.transform.localPosition = Vector3.zero;
			this.transform.localEulerAngles = Vector3.zero;
			myRectTransform.sizeDelta = _size;

			posA = posB = _pos; // default BOTH poses to the one provided. Assume we don't move.
			ApplyPos();

			SetMoveSpeed(1, 0); // Default my move-speed values.
		}
		public Prop SetPosB(float x,float y) {
			posB = new Vector2(x,y);
			return this;
		}
		public Prop SetMoveSpeed(float _speed, float _startLocOffset=0) {
			moveSpeed = _speed*0.02f; // awkward scaling down the speed here.
			moveOscVal = _startLocOffset;
			ApplyPos();
			return this;
		}
		public Prop SetRotateSpeed(float _rotateSpeed, float _startRotation=0) {
			rotateSpeed = _rotateSpeed;
			Rotation = _startRotation;
			return this;
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void ApplyPos() {
			// Do move?? Lerp me between my two poses.
			if (MayMove()) {
				float moveLoc = MathUtils.Sin01(moveOscVal);
				Pos = Vector2.Lerp(posA,posB, moveLoc);// + posDipOffset + posDanceOffset;
			}
			// DON'T move? Just put me at my one known pos.
			else {
				Pos = posA;// + posDipOffset + posDanceOffset;
			}
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (Time.timeScale == 0) { return; } // No time? No dice.
			if (myLevel.IsAnimatingIn) { return; } // Animating in? Don't move.

			UpdateMove();
			UpdateRotate();
			ApplyPos();
		}
		private void UpdateMove() {
			if (MayMove()) {
				moveOscVal += moveSpeed * timeScale;
			}
		}
		private void UpdateRotate() {
			if (MayRotate()) {
				Rotation += rotateSpeed * timeScale;
			}
		}



	}
}