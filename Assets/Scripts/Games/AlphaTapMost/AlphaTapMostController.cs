using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaTapMost {
	public enum GameStates {
		PreGame, Playing, GameOver
	}
	public class AlphaTapMostController : BaseGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.Undefined; }
		// Constants
		private const float AlphaGraceWindow = 0.12f; // we'll allow this much wiggle room in clicking tiles, so we're not TOO much of an alpha stickler.
		// Components
		private TapSpace[] tapSpaces;
		// Properties
		private GameStates gameState;
		private float timeLeft; // in SECONDS.
		private int numCorrectTaps;
		// References
		[SerializeField] private AlphaTapMostUI ui;
		[SerializeField] private SpriteRenderer sr_incorrectIcon;
		private Ray ray;
		private RaycastHit2D hit;

		// Getters
		private float GetHighestAvailableSpaceAlpha() {
			float highestAlpha = 0;
			foreach (TapSpace tapSpace in tapSpaces) {
				if (tapSpace.CanTapMe) {
					highestAlpha = Mathf.Max(highestAlpha, tapSpace.Alpha);
				}
			}
			return highestAlpha;
		}


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

			tapSpaces = FindObjectsOfType<TapSpace>();

			timeLeft = 11f;
			numCorrectTaps = 0;
			sr_incorrectIcon.enabled = false;
			gameState = GameStates.Playing;
		}


		// ----------------------------------------------------------------
		//  Game Flow
		// ----------------------------------------------------------------
		private void SetGameOver() {
			gameState = GameStates.GameOver;
			SetIsPaused(true);
		}
		private void OnTimeUp() {
			timeLeft = 0;
			SetGameOver();
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		override protected void Update () {
			base.Update();

			DeductTimeLeft();
			RegisterMouseInput();
		}

		private void DeductTimeLeft() {
			timeLeft -= Time.deltaTime;
			ui.UpdateTimeLeft(timeLeft);
			if (timeLeft <= 0) {
				OnTimeUp();
			}
		}

		private void RegisterMouseInput() {
			if (Input.GetMouseButtonDown(0)) {
				OnMouseDown();
			}
		}



		// ----------------------------------------------------------------
		//  Input
		// ----------------------------------------------------------------
		private void OnMouseDown() {
			if (gameState != GameStates.Playing) { return; } // Not playing? Don't do any checks!

			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);

			if (hit!=null && hit.collider!=null) {
				TapSpace tapSpace = hit.collider.gameObject.GetComponent<TapSpace>();
				if (tapSpace != null) {
					OnClickTapSpace(tapSpace);
				}
			}
		}


		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		private void OnClickTapSpace(TapSpace tapSpace) {
			if (!tapSpace.CanTapMe) { return; } // Discard non-tappable spaces.

			float highestSpaceAlpha = GetHighestAvailableSpaceAlpha();
			// GOOD tap!
			if (tapSpace.Alpha >= highestSpaceAlpha-AlphaGraceWindow) {
				OnCorrectTap(tapSpace);
			}
			// BAD tap!
			else {
				OnIncorrectTap(tapSpace);
			}
		}

		private void OnCorrectTap(TapSpace tapSpace) {
			tapSpace.OnCorrectTap();
			numCorrectTaps ++;
			ui.UpdateCorrectTaps(numCorrectTaps);
			// TEST
			timeLeft += 0.2f;
		}
		private void OnIncorrectTap(TapSpace tapSpace) {
			// Show the incorrect icon, yo!
			sr_incorrectIcon.enabled = true;
			sr_incorrectIcon.transform.localPosition = tapSpace.transform.localPosition;
			// End the game!
			SetGameOver();
		}



	}
}
