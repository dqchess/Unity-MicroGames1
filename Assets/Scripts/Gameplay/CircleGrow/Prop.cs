using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	abstract public class Prop : MonoBehaviour {
		// Components
        [SerializeField] protected Image i_body;
        [SerializeField] protected RectTransform myRectTransform=null;
        protected ImageLine il_movePathLine; // for MOVING Props.
		// Properties
		private float moveSpeed=1f; // for MOVING Props.
		private float moveOscVal; // for MOVING Props.
		private float rotateSpeed=0f; // for ROTATING Props.
        private Vector2 posA,posB; // for MOVING Props.
        private Vector2 size;
		// References
		protected Level myLevel;

		// Getters (Public)
		private Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		private float Rotation {
			get { return myRectTransform.localEulerAngles.z; }
			set {
                myRectTransform.localEulerAngles = new Vector3(myRectTransform.localEulerAngles.x, myRectTransform.localEulerAngles.y, value);
                OnSetRotation();
            }
		}
		// Getters (Protected)
		protected Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
        }
        protected Vector2 Size { get { return size; } }
        /** DoesMove returns if we are a MOVING Prop. MayMove returns if we're a moving Prop AND we're allowed to move (e.g. false for solid Growers). */
        private bool DoesMove() { return moveSpeed != 0; }
        private bool DoesRotate() { return rotateSpeed!=0; }
		virtual protected bool MayMove() {
			return DoesMove() && myLevel.IsGameStatePlaying;
		}
		virtual protected bool MayRotate() {
            return DoesRotate() && myLevel.IsGameStatePlaying;
		}
		// Getters (Private)
		private float timeScale { get { return Time.timeScale; } }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
        protected void BaseInitialize(Level _myLevel, Transform tf_parent, PropData data) {//, Vector2 _pos, Vector2 _size) {
			myLevel = _myLevel;

			GameUtils.ParentAndReset(this.gameObject, tf_parent);

            SetPoses(data.pos.x, data.pos.y);
            SetMoveSpeed(data.moveSpeed, data.moveLocOffset);
            SetSize(data.size);
            SetRotation(data.rotation);
            SetRotateSpeed(data.rotateSpeed);
            if (data.posB.x != Mathf.Infinity) { // TODO: #cleancode. Clean this pos A/B business up.
                SetPosB(data.posB);
            }

////			SetPosA(_pos); // default BOTH poses to the one provided. Assume we don't move.
////			SetPosB(_pos);
			//SetPoses(_pos.x,_pos.y); // default BOTH poses to the one provided. Assume we don't move.

			//SetMoveSpeed(1, 0); // Default my move-speed values.
		}
        virtual protected void OnDestroy() {
            DestroyMovePathLine();
        }


        public Prop SetSize(float x,float y) { return SetSize(new Vector2(x,y)); }
		virtual public Prop SetSize(Vector2 _size) {
            size = _size;
			myRectTransform.sizeDelta = size;
			return this;
		}
		public void SetPoses(float x,float y) {
			Vector2 _pos = new Vector2(x,y);
			posA = _pos;
			SetPosB(_pos);
		}
//		public Prop SetPosA(Vector2 _pos) { return SetPosA(_pos.x, _pos.y); }
//		public Prop SetPosA(float x,float y) {
//			posA = new Vector2(x,y);
//			ApplyPos();
//			return this;
//		}
		public Prop SetPosB(Vector2 _pos) { return SetPosB(_pos.x, _pos.y); }
		public Prop SetPosB(float x,float y) {
			posB = new Vector2(x,y);
			ApplyPos();
            UpdateMovePathLine();
			return this;
		}
		public Prop SetMoveSpeed(float _speed, float _startLocOffset=0) {
			moveSpeed = _speed*0.02f; // awkward scaling down the speed here.
			moveOscVal = _startLocOffset;
			ApplyPos();
			return this;
		}
		public Prop SetRotation(float _rotation) {
			Rotation = _rotation;
			return this;
		}
		public Prop SetRotateSpeed(float _rotateSpeed, float _startRotation=0) {
			rotateSpeed = _rotateSpeed;
			SetRotation(_startRotation);
			return this;
		}

        private void UpdateMovePathLine() {
            // We SHOULD have a MovePathLine! Update (and/or add) it!
            if (posA != posB) {
                if (il_movePathLine == null) { AddMovePathLine(); }
                il_movePathLine.StartPos = posA;
                il_movePathLine.EndPos = posB;
            }
        }
        private void AddMovePathLine() {
            il_movePathLine = Instantiate(ResourcesHandler.Instance.imageLine).GetComponent<ImageLine>();
            il_movePathLine.Initialize(transform.parent); // put it as the same level as me! I'll MANUALLY destroy it when we're destroyed. //myLevel.tf_MovePathLines
            il_movePathLine.name = "PathLine_" + this.name;
            il_movePathLine.transform.SetSiblingIndex(3); // put it BEHIND all Props! NOTE: HARDCODED 3!! We want it in front of border and bounds.
            il_movePathLine.SetColor(new Color(0,0,0, 0.3f));
            il_movePathLine.SetThickness(2f);
            il_movePathLine.rectTransform.anchorMax = myRectTransform.anchorMax;
            il_movePathLine.rectTransform.anchorMin = myRectTransform.anchorMin;
            //il_movePathLine
        }
        private void DestroyMovePathLine() {
            if (il_movePathLine != null) {
                Destroy(il_movePathLine.gameObject);
                il_movePathLine = null;
            }
        }



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
        public void SetColliderEnabled(bool _isEnabled) {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) { collider.enabled = _isEnabled; }
        }
		private void ApplyPos() {
			// Do move?? Lerp me between my two poses.
			if (DoesMove()) {
				float moveLoc = MathUtils.Sin01(moveOscVal);
				Pos = Vector2.Lerp(posA,posB, moveLoc);// + posDipOffset + posDanceOffset;
			}
			// DON'T move? Just put me at my one known pos.
			else {
				Pos = posA;// + posDipOffset + posDanceOffset;
			}
		}

		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
        virtual protected void OnSetRotation() { } // Override this if you wanna do something when this happens. #code #programming
		abstract public void OnIllegalOverlap();


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		virtual protected void Update() {
			if (Time.timeScale == 0) { return; } // No time? No dice.
            if (myLevel.IsAnimating || !myLevel.IsGameStatePlaying) { return; } // Animating in? Don't move.

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