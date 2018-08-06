using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class Player : MonoBehaviour {
		// Components
		[SerializeField] private Image i_body=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private bool didHitBar; // each time we turn around, this is set to false! Set to true when we do hit it successfully.
		private bool isDead;
		private float loc; // from 0 (TOP of wave) to 1 (BOTTOM of wave).
		private float locSpeed;
		private float radius;
//		private const float rangeY = 200; // how high/low we can go from center!
		private float posYStart = 200f; // the TOP of my wave.
		private float posYEnd = -200f; // the BOTTOM of my wave.
		// References
		[SerializeField] private Bar bar;
		[SerializeField] private GameController gameController;
		[SerializeField] private Sprite s_bodyNormal=null;
		[SerializeField] private Sprite s_bodyDashedOutline=null;


		// Getters (Public)
		public float PosY { get { return posY; } }
		public float Radius { get { return radius; } }
		// Getters (Private)
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
		private float bodyColorAlpha {
			get { return i_body.color.a; }
			set { i_body.color = new Color(i_body.color.r,i_body.color.g,i_body.color.b, value); }
		}
		private Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		private float posY {
			get { return Pos.y; }
			set { Pos = new Vector2(Pos.x, value); }
		}
		private float PosYWhenPassBar {
			get {
				int dirMoving = MathUtils.Sign(locSpeed);
				return bar.PosY - radius*dirMoving;
			}
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
//		public void Initialize(GameController _gameController, Transform tf_parent, Vector2 _pos, int _levelIndex) {
//			gameController = _gameController;
//			this.transform.SetParent(tf_parent);
//			this.transform.localScale = Vector3.one;
//			this.transform.localPosition = Vector3.zero;
//			this.transform.localEulerAngles = Vector3.zero;
//
//			Pos = _pos;
//			speed = 1f; // TO DO: This.
//		}
		public void Reset(int _levelIndex) {
			// Reset basic values.
			isDead = false;
			didHitBar = false;
			i_body.sprite = s_bodyNormal;
			bodyColor = new Color(0/255f, 214/255f, 190/255f);

			// Set level-specific values!
			SetRadius(20);
			StartCoroutine(Coroutine_StartMoving(_levelIndex));
		}

		private IEnumerator Coroutine_StartMoving(int _levelIndex) {
			// First, start me frozen.
			loc = 0f;
			locSpeed = 0;
			UpdatePosY();
			// Flash me that I'm ready to roar!
			for (int i=0; i<4; i++) {
				bodyColorAlpha = 0.12f;
				yield return new WaitForSeconds(0.12f);
				bodyColorAlpha = 1f;
				yield return new WaitForSeconds(0.12f);
			}

			// Rooooar!
			locSpeed = 0.017f + _levelIndex*0.0002f;
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			i_body.rectTransform.sizeDelta = new Vector2(radius*2, radius*2);
		}
		private void TurnAround() {
			locSpeed *= -1;
			didHitBar = false; // when we turn around, this is set to false!
		}
		public void Die(LoseReasons reason) {
			isDead = true;
			i_body.sprite = s_bodyDashedOutline;
			bodyColor = new Color(1f, 0.1f, 0f);
			gameController.OnPlayerDie(reason);
		}

		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void OnPassedBar() {
			Die(LoseReasons.MissedTap);
		}
		public void OnHitBar() {
			didHitBar = true;
			if (bar.NumHitsLeft <= 0) { // Bar's toast? Let's just stop moving.
				locSpeed = 0;
				bodyColor = Color.green;
			}
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (Time.timeScale == 0) { return; } // No time? No dice.
			if (isDead) { return; }

			AdvanceLoc();
			ApplyLocBounds();
			CheckIfPassedBar();
			UpdatePosY();
		}
		private void AdvanceLoc() {
			loc += locSpeed * Time.timeScale;
		}
		private void ApplyLocBounds() {
			if (locSpeed>0 && loc>=1) { TurnAround(); }
			else if (locSpeed<0 && loc<=0) { TurnAround(); }
		}
		private void CheckIfPassedBar() {
			if (didHitBar) { return; } // Don't check if we just hit it.
			if (gameController.IsLevelWon) { return; } // If we already won, do nothin'!

			if ((locSpeed>0 && posY<PosYWhenPassBar)
			|| (locSpeed<0 && posY>PosYWhenPassBar)) {
				OnPassedBar();
			}
		}
		private void UpdatePosY() {
			posY = Mathf.Lerp(posYStart, posYEnd, loc);
		}





	}
}