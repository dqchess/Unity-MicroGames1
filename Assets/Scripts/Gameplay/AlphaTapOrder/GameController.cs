using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

namespace AlphaTapOrder {
	public enum GameStates { PreLevel, Playing, PostLevel, GameOver }

	public class GameController : BaseGameController {
		// Components
		private TapSpace[] tapSpaces;
		// Properties
		private GameStates gameState;
		private int currentLevelIndex;
		private int nextSpaceNumber;
		// References
		[SerializeField] private GameUI ui;
		[SerializeField] private RectTransform rt_spaces;



		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		override protected void Start () {
			base.Start();

			SetCurrentLevel(0);
		}


		private void InitializeTapSpaces(int numSpaces) {
			DestroyTapSpaces(); // Just in case

			// Instantiate, and do color!
			float hueA = Random.Range(0,1f);
			float hueB = (hueA + Random.Range(0.25f, 0.75f)) % 1f;
			Color colorA = new ColorHSB(hueA, 0.8f, 1f).ToColor();
			Color colorB = new ColorHSB(hueB, 0.8f, 1f).ToColor();
//			Color colorA = new ColorHSB(Random.Range(0,1f), 1f, 0.2f).ToColor();
//			Color colorA = Color.Lerp(colorB, Color.black, 0.8f);
			tapSpaces = new TapSpace[numSpaces];
			for (int i=0; i<numSpaces; i++) {
				TapSpace newSpace = Instantiate(resourcesHandler.alphaTapOrder_tapSpace).GetComponent<TapSpace>();
				Color spaceColor = Color.Lerp(colorA,colorB, i/(float)(numSpaces-1));
				newSpace.Initialize(this, rt_spaces, i, spaceColor);
				tapSpaces[i] = newSpace;
			}

			// Position and size!
			Vector2 availableSize = rt_spaces.rect.size;
			Vector2 spaceSize = new Vector2(availableSize.x/3f-1f,availableSize.x/3f-1f); // 3 cols. -1 for safety.
			float tempX = 0;
			float tempY = 0;
			int[] shuffledIndexes = MathUtils.GetShuffledIntArray(numSpaces);
			for (int i=0; i<numSpaces; i++) {
				int index = shuffledIndexes[i];
				TapSpace space = tapSpaces[index];
				space.SetSizePos(spaceSize, new Vector2(tempX,tempY));
				tempX += spaceSize.x;
				if (tempX+spaceSize.x > availableSize.x) { // Wrap!
					tempX = 0;
					tempY += spaceSize.y;
				}
			}

//			// TEST: Start with the first one already tapped!
//			OnCorrectTap(tapSpaces[0]);
		}
		private void DestroyTapSpaces() {
			if (tapSpaces!=null) {
				foreach (TapSpace tapSpace in tapSpaces) {
					Destroy(tapSpace.gameObject);
				}
			}
			tapSpaces = null;
		}


		// ----------------------------------------------------------------
		//  Game Flow
		// ----------------------------------------------------------------
		private void StartNextLevel() { SetCurrentLevel(currentLevelIndex+1); }
		private void SetCurrentLevel(int _levelIndex) {
			currentLevelIndex = _levelIndex;

			// Set basics!
			nextSpaceNumber = 0;
			gameState = GameStates.Playing;
			Camera.main.backgroundColor = new Color(0.1f,0.1f,0.1f);

			// Tell the UI!
			ui.UpdateLevelName(currentLevelIndex);

			// Set properties!
			int numSpaces;
//			switch (currentLevelIndex) {
//			case 0:
//				numSpaces = 3;
//				break;
//			default:
//				numSpaces = 4;
//				break;
//			}
			numSpaces = 3 + currentLevelIndex;
			numSpaces = Mathf.Min(12, numSpaces); // note: added a cap so they don't go offscreen. Also it gets crazy hard.

			// Initialize spaces!
			InitializeTapSpaces(numSpaces);
		}
		private void SetGameOver() {
			gameState = GameStates.GameOver;
			SetIsPaused(true);
			Camera.main.backgroundColor = new Color(0.6f,0.1f,0.1f);
		}

		private void OnCompleteLevel() {
			gameState = GameStates.PostLevel;
			Camera.main.backgroundColor = new Color(0.1f,0.8f,0.1f);
//			StartCoroutine(Coroutine_CompleteLevel());
//		}
//		private IEnumerator Coroutine_CompleteLevel() {
//			// Set properties and wait a brief moment!
//			gameState = GameStates.PostLevel;
////			i_correctIcon.enabled = true;
//			yield return new WaitForSecondsRealtime(0.3f);
//
//			// Wait for click!
//			while (true) {
//				if (Input.GetMouseButtonDown(0)) { break; }
//				yield return null;
//			}
//
//			// After click, start the next level!
//			StartNextLevel();
//			yield return null;
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		override protected void Update () {
			base.Update();

			RegisterMouseInput();
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
			if (gameState == GameStates.GameOver) {
				ReloadScene();
				return;
			}
			else if (gameState == GameStates.PostLevel) {
				StartNextLevel();
				return;
			}
		}


		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		public void OnTapSpaceClicked(TapSpace tapSpace) {
			if (!tapSpace.CanTapMe) { return; } // Discard non-tappable spaces.

			// HACk. Allow tapping either end first.
			if (nextSpaceNumber==0 && tapSpace.MyNumber==tapSpaces.Length-1) {
				for (int i=0; i<tapSpaces.Length; i++) {
					tapSpaces[i].SetMyNumber(tapSpaces.Length-1-i);
				}
			}

			// GOOD tap!
			if (tapSpace.MyNumber == nextSpaceNumber) {
				OnCorrectTap(tapSpace);
			}
			// BAD tap!
			else {
				OnIncorrectTap(tapSpace);
			}
		}

		private void OnCorrectTap(TapSpace tapSpace) {
			tapSpace.OnCorrectTap();
			nextSpaceNumber ++;
			if (nextSpaceNumber >= tapSpaces.Length) {
				OnCompleteLevel();
			}
		}
		private void OnIncorrectTap(TapSpace tapSpace) {
			tapSpace.OnIncorrectTap();
			// End the game!
			SetGameOver();
		}





	}
}
