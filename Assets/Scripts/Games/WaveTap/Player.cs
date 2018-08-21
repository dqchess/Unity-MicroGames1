using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class Player : Prop {
        // Constants
        private readonly Color bodyColorNeutral = new Color(0/255f, 214/255f, 190/255f);
		// Components
		[SerializeField] private CircleCollider2D myCollider=null;
		[SerializeField] private Image i_body=null;
		// Properties
        private bool isFrozen; // TRUE while we're flashing on/off before we start moving!
		private float baseSpeed; // always positive.
		private float radius;
		private float posYMax; // TOP of my wave.
		private float posYMin; // BOTTOM of my wave.
		private float posYWhenPassBar; // nextBar's y pos +/- my radius.
		private int dirMoving; // Direction moving. -1 or 1.
		private List<Bar> barsTouching; // 99% of the time will have ZERO or ONE element. Added/removed from by OnTriggerEnter/Exit2D. We may only tap when we're touching a bar!
		// References
		[SerializeField] private Sprite s_bodyNormal=null;
		[SerializeField] private Sprite s_bodyDashedOutline=null;


        // Getters (Public)
        public bool IsFrozen { get { return isFrozen; } }
        public bool IsTouchingABar() { return barsTouching.Count > 0; }
        public float PosYMax { get { return posYMax; } }
        public float PosYMin { get { return posYMin; } }
        public float Diameter { get { return radius*2; } }
        public float Radius { get { return radius; } }
		public int DirMoving { get { return dirMoving; } }
		// Getters (Private)
        static private float GetBaseSpeed(int _levelIndex) {
            return 5f + _levelIndex*0.06f;
        }
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
		private float bodyColorAlpha {
			get { return i_body.color.a; }
			set { i_body.color = new Color(i_body.color.r,i_body.color.g,i_body.color.b, value); }
		}
		private Bar GetBarFromCollider(Collider2D col) {
			return col.GetComponent<Bar>();
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Level _level, Transform tf_parent, PlayerData _data) {
			InitializeAsProp(_level, tf_parent, _data);

            posYMax = _data.posYMax;
            posYMin = _data.posYMin;
			i_body.sprite = s_bodyNormal;
            bodyColor = bodyColorNeutral;
			SetRadius(20);
			barsTouching = new List<Bar>();

            // Start me at the top, moving down!
            SetDirMoving(-1);
            posY = posYMax;

			// Let's goo!
			StartCoroutine(Coroutine_StartMoving());
		}

		private IEnumerator Coroutine_StartMoving() {
            // First, start me frozen.
            isFrozen = true;
            baseSpeed = GetBaseSpeed(LevelIndex);
			//UpdatePosY();
			// Flash me that I'm ready to roar!
			for (int i=0; i<3; i++) {
				bodyColorAlpha = 0.12f;
				yield return new WaitForSeconds(0.12f);
				bodyColorAlpha = 1f;
				yield return new WaitForSeconds(0.12f);
			}

			// Rooooar!
            isFrozen = false;
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			i_body.rectTransform.sizeDelta = new Vector2(Diameter, Diameter);
			myCollider.radius = radius;
		}
		private void TurnAround() {
			dirMoving *= -1;
		}
		private void SetDirMoving(int _dirMoving) {
			dirMoving = _dirMoving;
		}
		public void RapBarsTouching() {
			foreach (Bar bar in barsTouching) { RapBar(bar); }
		}
		private void RapBar(Bar bar) {
			bar.RapMe();
		}



		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void OnTriggerEnter2D(Collider2D col) {
			Bar bar = GetBarFromCollider(col);
			if (bar != null) { OnTriggerEnterBar(bar); }
		}
		private void OnTriggerExit2D(Collider2D col) {
			Bar bar = GetBarFromCollider(col);
			if (bar != null) { OnTriggerExitBar(bar); }
		}

		private void OnTriggerEnterBar(Bar bar) {
//			if (barsTouching.Contains(bar)) { Debug.LogError("Whoa, Player just touched a Bar it was ALREADY touching. Brett! Fix some collision code!"); return; } // Safety check.
			barsTouching.Add(bar);
			bar.DidRapDuringContact = false; // Set to false so we can set to true when we rap before we lose contact!
		}
		private void OnTriggerExitBar(Bar bar) {
			// Snap, did we NOT rap it in time? Then we lose!
			if (!bar.DidRapDuringContact) {
                bar.OnMissMe();
                level.OnMissedBar();
            }
            bar.DidRapDuringContact = false; // Clear this out for cleanliness.
			barsTouching.Remove(bar);
		}


		public void OnWinLevel() {
			// Stop moving and turn green!
            isFrozen = true;
			bodyColor = Color.green;
		}
		public void OnLoseLevel() {
            // Stop moving and turn red!
            isFrozen = true;
			i_body.sprite = s_bodyDashedOutline;
			bodyColor = new Color(1f, 0.1f, 0f);
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (Time.timeScale == 0) { return; } // No time? No dice.

			AdvancePosY();
            ApplyBounds();
		}
        private void AdvancePosY() {
            if (isFrozen) { return; } // Frozen? Do nada.
            float speed = baseSpeed * dirMoving;
			posY += speed * Time.timeScale;
		}
		private void ApplyBounds() {
			if (dirMoving>0 && posY>=posYMax) { TurnAround(); }
			else if (dirMoving<0 && posY<=posYMin) { TurnAround(); }
		}
		//private void UpdatePosY() {
		//	posY = Mathf.Lerp(posYStart, posYEnd, loc);
		//}





	}
}