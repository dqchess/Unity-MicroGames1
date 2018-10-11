using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AlphaTapMatch {
	public enum GameStates { PreLevel, Playing, PostLevel, GameOver }

	public class GameController : BaseGameController {
        // Overrideables
        override public string MyGameName() { return GameNames.Undefined; }
		// Constants
//		private const float AlphaGraceWindow = 0.12f; // we'll allow this much wiggle room in clicking tiles, so we're not TOO much of an alpha stickler.
		// Properties
		private GameStates gameState;
//		private ColorHSB targetColorHSB;
//		private float oscSpeedH,oscSpeedS,oscSpeedB;
//		private float currCol
		private Color targetColor;
		private Color colorA, colorB; // we lerp between these two
		private float currentColorLoc; // this oscillates between 0 and 1.
		private float oscSpeed; // how quickly we oscillate between colors A and B.
		private int currentLevelIndex;
		private int currentColorLocDir=1;//TEST for linearness
		// References
		[SerializeField] private GameUI ui;
//		[SerializeField] private Image i_currentColor;
		[SerializeField] private TextMeshProUGUI t_levelNumber; // note: THIS is what we color!
		[SerializeField] private Image i_targetColor;
		[SerializeField] private Image i_correctIcon;
		[SerializeField] private Image i_incorrectIcon;

		// Getters
		private Color CurrentColor() {
			return Color.Lerp(colorA,colorB, currentColorLoc);
		}
		private bool IsCurrentColorMatch() {
			float percentMatch = GetPercentColorMatch(CurrentColor(), targetColor);
//			Debug.Log("percentMatch: " + percentMatch);
			return percentMatch < 0.05f;
		}
		private float GetPercentColorMatch(Color cA, Color cB) {
			float total = 0;
			total += Mathf.Abs(cA.r - cB.r);
			total += Mathf.Abs(cA.g - cB.g);
			total += Mathf.Abs(cA.b - cB.b);
			return total / 3f; // the range goes from 0 to 3, so normalize it to a 0-1 percentage.
		}



		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

			SetCurrentLevel(1);
		}


		// ----------------------------------------------------------------
		//  Game Flow
		// ----------------------------------------------------------------
		private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
		private void SetCurrentLevel(int _levelIndex) {
			currentLevelIndex = _levelIndex;

			// Set basics!
			i_correctIcon.enabled = false;
			i_incorrectIcon.enabled = false;
			gameState = GameStates.Playing;

			// Tell the UI!
//			ui.UpdateLevelName(currentLevelIndex);
			t_levelNumber.text = currentLevelIndex.ToString();
			// First set some defaults
			colorA = new Color(0.2f,0.2f,0.2f);
			colorB = new Color(0.8f,0.8f,0.8f);
			targetColor = Color.Lerp(colorA,colorB, 0.5f);
			oscSpeed = 0.4f;

			// Set properties!
			switch (currentLevelIndex) {
			case 1:
				colorA = new Color(0.2f,0.2f,0.2f);
				colorB = new Color(0.8f,0.8f,0.8f);
				targetColor = Color.Lerp(colorA,colorB, 0.5f);
				oscSpeed = 0.6f;
				break;
			case 2:
				colorA = Color.blue;
				colorB = new Color(0.8f,0.8f,0.8f);
				targetColor = Color.Lerp(colorA,colorB, 0.5f);
				oscSpeed = 0.9f;
				break;
			default:
				oscSpeed = 1f + currentLevelIndex*0.05f;
				float hA = Random.Range(0f,1f);
				float hB = (hA+Mathf.Min(0.5f, currentLevelIndex*0.03f)) % 1f;
				colorA = new ColorHSB(hA, Random.Range(0.4f,1f), Random.Range(0.84f,1f)).ToColor();
				colorB = new ColorHSB(hB, Random.Range(0.4f,1f), Random.Range(0.84f,1f)).ToColor();
				targetColor = Color.Lerp(colorA,colorB, Random.Range(0f,1f));//colorA;
				break;
			}

			// Apply initial visuals!
			ApplyCurrentColor();
			i_targetColor.color = targetColor;
		}
		private void SetGameOver() {
			gameState = GameStates.GameOver;
			SetIsPaused(true);
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		override protected void Update () {
			base.Update();

			if (gameState == GameStates.Playing) {
				StepCurrentColor();
				ApplyCurrentColor();
			}

			RegisterMouseInput();
		}

		private void StepCurrentColor() {
//			currentColorLoc = MathUtils.Sin01(Time.time * oscSpeed);
			// TEST linear
			currentColorLoc += oscSpeed*0.01f * currentColorLocDir;
			if ((currentColorLocDir== 1 && currentColorLoc>=1)
			||  (currentColorLocDir==-1 && currentColorLoc<=0)) {
				currentColorLocDir *= -1;
			}
		}
		private void ApplyCurrentColor() {
			t_levelNumber.color = CurrentColor();
		}




		// ----------------------------------------------------------------
		//  Input
		// ----------------------------------------------------------------
		private void RegisterMouseInput() {
			if (Input.GetMouseButtonDown(0)) {
				OnMouseDown();
			}
		}
		private void OnMouseDown() {
			if (gameState != GameStates.Playing) { return; } // Not playing? Don't do any checks!

			// The colors match!
			if (IsCurrentColorMatch()) {
				SubmitCorrectColor();
			}
			// The colors don't match.
			else {
				SubmitIncorrectColor();
			}
		}


		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		private void SubmitCorrectColor() {
			StartCoroutine(Coroutine_SubmittedCorrectColor());
		}
		private IEnumerator Coroutine_SubmittedCorrectColor() {
			// Set properties and wait a brief moment!
			gameState = GameStates.PostLevel;
			i_correctIcon.enabled = true;
			yield return new WaitForSecondsRealtime(0.3f);

			// Wait for click!
			while (true) {
				if (Input.GetMouseButtonDown(0)) {
					break;
				}
				yield return null;
			}

			// After click, start the next level!
			StartNextLevel();
			yield return null;
		}
		private void SubmitIncorrectColor() {
			// Show the incorrect icon, yo!
			i_incorrectIcon.enabled = true;
			// End the game!
			SetGameOver();
		}



	}
}
