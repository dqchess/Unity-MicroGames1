using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class Player : Prop {
		// Components
		[SerializeField] private Image i_body=null;
		// Properties
		private float loc; // from 0 (TOP of wave) to 1 (BOTTOM of wave).
		private float baseLocSpeed; // always positive.
		private float radius;
//		private const float rangeY = 200; // how high/low we can go from center!
		private float posYStart; // TOP of my wave.
		private float posYEnd; // BOTTOM of my wave.
		private float posYWhenPassBar; // nextBar's y pos +/- my radius.
		private int dirMoving; // Direction moving. -1 or 1.
//		private int dirWhenPassBar; // Direction moving. -1 or 1.
		private List<Bar> barsBeforeTurn; // the nice list of all the bars we gotta hit before we turn around! Set every time we turn around.
		// References
		[SerializeField] private Sprite s_bodyNormal=null;
		[SerializeField] private Sprite s_bodyDashedOutline=null;


		// Getters (Public)
		public float Radius { get { return radius; } }
		public int DirMoving { get { return dirMoving; } }
		// Getters (Private)
		private Bar NextBar { get { return level.NextBar; } }
		private float NextBarY { get { return NextBar==null ? 0 : NextBar.PosY; } }
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
		private float bodyColorAlpha {
			get { return i_body.color.a; }
			set { i_body.color = new Color(i_body.color.r,i_body.color.g,i_body.color.b, value); }
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Level _level, Transform tf_parent, PlayerData _data) {
			InitializeAsProp(_level, tf_parent, _data);

			posYStart = _data.posYStart;
			posYEnd = _data.posYEnd;
			loc = _data.startingLoc;
			i_body.sprite = s_bodyNormal;
			bodyColor = new Color(0/255f, 214/255f, 190/255f);
			SetRadius(20);
			SetDirMoving(1);

			// Let's goo!
			StartCoroutine(Coroutine_StartMoving());
		}

		private IEnumerator Coroutine_StartMoving() {
			// First, start me frozen.
			baseLocSpeed = 0;
			UpdatePosY();
			// Flash me that I'm ready to roar!
			for (int i=0; i<4; i++) {
				bodyColorAlpha = 0.12f;
				yield return new WaitForSeconds(0.12f);
				bodyColorAlpha = 1f;
				yield return new WaitForSeconds(0.12f);
			}

			// Rooooar!
			baseLocSpeed = 0.008f + LevelIndex*0.00012f;
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			i_body.rectTransform.sizeDelta = new Vector2(radius*2, radius*2);
		}
		private void TurnAround() {
			dirMoving *= -1;
//			didHitBar = false; // when we turn around, this is set to false!
		}
		private void SetDirMoving(int _dirMoving) {
			dirMoving = dirMoving;
			barsBeforeTurn = level.GetBarsInOrder(dirMoving);
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void OnMissedNextBar() {
			level.OnMissedNextBar();
		}
		public void OnWinLevel() {
			// Stop moving and turn green!
			baseLocSpeed = 0;
			bodyColor = Color.green;
		}
		public void OnLoseLevel() {
			// Stop moving and turn red!
			baseLocSpeed = 0;
			i_body.sprite = s_bodyDashedOutline;
			bodyColor = new Color(1f, 0.1f, 0f);
		}
		public void OnHitBar() {
		}
		public void OnSetNextBar(bool isNextBarInSameDir) {
//			bool  = NextBarIndex==0;
			dirWhenPassBar = dirMoving * (isNextBarInSameDir?1:-1);
			posYWhenPassBar = NextBarY - radius*dirWhenPassBar;
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (Time.timeScale == 0) { return; } // No time? No dice.

			AdvanceLoc();
			ApplyLocBounds();
			CheckIfPassedBar();
			UpdatePosY();
		}
		private void AdvanceLoc() {
			float locSpeed = baseLocSpeed * dirMoving;
			loc += locSpeed * Time.timeScale;
		}
		private void ApplyLocBounds() {
			if (dirMoving>0 && loc>=1) { TurnAround(); }
			else if (dirMoving<0 && loc<=0) { TurnAround(); }
		}
		private void CheckIfPassedBar() {
//			if (didHitBar) { return; } // Don't check if we just hit it.
			if (!level.IsGameStatePlaying) { return; } // If we're not playing, do nothing.

			if ((dirWhenPassBar>0 && dirMoving>0 && posY<posYWhenPassBar)
			|| (dirWhenPassBar<0 && dirMoving<0 && posY>posYWhenPassBar)) {
				OnMissedNextBar();
			}
		}
		private void UpdatePosY() {
			posY = Mathf.Lerp(posYStart, posYEnd, loc);
		}





	}
}