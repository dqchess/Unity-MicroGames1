using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class FUEController : MonoBehaviour {
		// Constants
		public const string ID_FUE_1 = "1"; // Matches Levels.xml's fueID.
		public const string ID_FUE_2 = "2"; // Matches Levels.xml's fueID.
		public const string ID_INTRO_UNDO = "introToUndo"; // Matches Levels.xml's fueID.
		private const string SEQ_FUE_1 = "SEQ_FUE_1"; // Doesn't matter what this is.
		private const string SEQ_FUE_2 = "SEQ_FUE_2"; // Doesn't matter what this is.
		private const string SEQ_INTRO_UNDO = "SEQ_INTRO_UNDO"; // Doesn't matter what this is.
		// Specific tutorial properties
		private Vector2[] FingerMoveBoardPoses_FUE_1 = new Vector2[] {
			new Vector2(0,1.5f),
			new Vector2(1,1.5f),
		};
		private Vector2[] FingerMoveBoardPoses_FUE_2A = new Vector2[] {
			new Vector2(0,1.5f),
			new Vector2(1,1.5f),
		};
		private Vector2[] FingerMoveBoardPoses_FUE_2B() {
			// They HAVE merged the green Tiles??
			if (level.Board.GetNumTiles(1) == 1) {
				return new Vector2[] {
					new Vector2(0,1.5f),
					new Vector2(1,1.5f),
				};
			}
			return new Vector2[] {
				new Vector2(0,0),
				new Vector2(1,0),
			};
		}
		// Components
		[SerializeField] private Image i_arrow=null;
		[SerializeField] private Image i_dragFinger=null;
		[SerializeField] private TextMeshProUGUI t_instructions=null;
//		[SerializeField] private GameObject go_findTheseWordsPopup=null;
		// Properties
		private Coroutine fingerMoveTileCoroutine;
		private Coroutine bounceArrowCoroutine;
		public bool CanTouchBoard { get; private set; }
		private int currentStep; // Each sequence is made up of steps. Which step are we on?
		private float timeUntilNextStep; // when -1, we won't be counting down.
		private string currentSeq; // set to currentLevel's fueID, and matches one of the constants up top (or is empty).
		// References
		private Level level;

		// Getters (Private)
		private bool IsSequenceActive() { return !string.IsNullOrEmpty(currentSeq); }
		private InputController inputController { get { return InputController.Instance; } }
		private Vector2[] BoardPosesToScreen(Vector2[] boardPoses) {
			Vector2[] returnPoses = new Vector2[boardPoses.Length];
			for (int i=0; i<returnPoses.Length; i++) {
				returnPoses[i] = new Vector2(
					level.BoardView.BoardToXGlobal(boardPoses[i].x),
					level.BoardView.BoardToYGlobal(boardPoses[i].y));
			}
			return returnPoses;
		}


		// ----------------------------------------------------------------
		//  Awake / Destroy
		// ----------------------------------------------------------------
		private void Awake () {
			// Hide things by default!
			HideAllComponents();
			CanTouchBoard = true;

			// Add event listeners!
//			GameManagers.Instance.EventManager.StartGameAtLevelEvent += OnStartLevel;
		}
		private void OnDestroy () {
			// Remove event listeners!
//			GameManagers.Instance.EventManager.StartGameAtLevelEvent -= OnStartLevel;
		}


		// ================================================================
		//  STARTING / Ending Sequences!
		// ================================================================
		private void StartSequence(string newSeqName) {
			if (IsSequenceActive()) {
				Debug.LogError ("Hey, bud! We're trying to start sequence " + newSeqName + ", but we haven't finished sequence " + currentSeq + ".");
				return;
			}

			// Reset values
			HideAllComponents();
			timeUntilNextStep = -1;
			currentStep = 0;
			CanTouchBoard = true;

			currentSeq = newSeqName;

			// Start with the first step!
			NextStep ();
		}
		private void EndSequence() {
			if (!IsSequenceActive()) {
				Debug.LogError ("Hey, dood! We're trying to end a sequence... but we're not running one right now. :P");
				return;
			}
			// Hide everything!
			HideAllComponents();
			// Reset values
			currentSeq = null;
			timeUntilNextStep = -1;
			currentStep = 0;
			CanTouchBoard = true;
			GameUtils.ParentAndReset(i_arrow.gameObject, this.transform); // Make sure it's back on my tf (we may have tossed it on another tf).
		}



		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnStartLevel(Level _level) {
			level = _level;

			// In case we were sequencin', stop.
			if (IsSequenceActive()) {
				EndSequence();
			}

			// Start an FUE sequence??
			string fueID = _level.Board.FUEID;
			if (fueID == ID_FUE_1) {
				StartSequence(SEQ_FUE_1);
			}
			else if (fueID == ID_FUE_2) {
				StartSequence(SEQ_FUE_2);
			}
			//else if (fueID == ID_INTRO_UNDO) {NOTE: Disabled this FUE. It's meh.
			//	StartSequence(SEQ_INTRO_UNDO);
			//}
		}
		public void OnCompleteLevel() {
			HideAllComponents();
		}
		public void OnBoardMoveComplete() {
			if (currentSeq == SEQ_FUE_2) {
				if (level.Board.tiles.Count==3) {
					NextStep();
				}
			}
			else if (currentSeq == SEQ_INTRO_UNDO) {
				if (level.NumMovesMade == 1) { // First move? Next step!
					NextStep();
				}
			}
		}
		public void OnUndoMove() {
			if (currentSeq == SEQ_INTRO_UNDO) {
				NextStep();
			}
		}
		public void OnTouchUp() {
		}
		public void OnTouchDown() {
			if (currentSeq == SEQ_INTRO_UNDO) {
				if (currentStep == 2) {
					NextStep();
				}
			}
		}



		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void HideAllComponents() {
			// Stop all coroutines.
			StopAnimation_FingerMoveTile();
			StopAnimation_BounceArrow();
			// Hide businesses!
			t_instructions.text = "";
//			go_findTheseWordsPopup.SetActive(false);
		}
		private void SetDragFingerPos(Vector2 _pos) {
			i_dragFinger.rectTransform.anchoredPosition = _pos;
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
        public void DependentUpdate() {
			// We've got a sequence going on??
			if (IsSequenceActive()) {
				// Counting down to next step.
				if (timeUntilNextStep != -1) {
					timeUntilNextStep -= Time.unscaledDeltaTime;
					if (timeUntilNextStep <= 0) {
						timeUntilNextStep = -1;
						NextStep ();
					}
				}

				RegisterMouseInput();
			}

		}
		private void RegisterMouseInput() {
			if (inputController == null) { return; } // For compiling during runtime.
			// Mouse UP
			if (inputController.IsTouchUp()) {
				OnTouchUp();
			}
			// Mouse DOWN
			else if (inputController.IsTouchDown()) {
				OnTouchDown();
			}
		}



		// ================================================================
		//  STEPPING Through Sequences!
		// ================================================================
		private void NextStep() {
			int s = 1; // shortcut to avoid hard-coding numbas.

			currentStep ++;

			// ---- FUE 1 ----
			if (currentSeq == SEQ_FUE_1) {
				if (currentStep == s++) {
					level.UndoMoveInputController.SetButtonsVisible(false); // No undos/resets.
					StartAnimation_FingerMoveTile(FingerMoveBoardPoses_FUE_1);
					t_instructions.text = "Merge the Tiles!";
				}
			}

			// ---- FUE 2 ----
			else if (currentSeq == SEQ_FUE_2) {
				if (currentStep == s++) {
					level.UndoMoveInputController.SetButtonsVisible(false); // No undos/resets.
					StartAnimation_FingerMoveTile(FingerMoveBoardPoses_FUE_2A, 3); // Give them 3 seconds to merge something before we show the finger animation.
//					t_instructions.text = "Merge colors together!";
				}
				else if (currentStep == s++) {
					StopAnimation_FingerMoveTile();
					StartAnimation_FingerMoveTile(FingerMoveBoardPoses_FUE_2B(), 1.1f); // Give them 1 second to merge something before we show the finger animation.
				}
			}

			// ---- FUE 3/3 ----
			else if (currentSeq == SEQ_INTRO_UNDO) {
				// Wait for the first move.
				if (currentStep == s++) {
					level.UndoMoveInputController.SetButtonsVisible(false); // No undos/resets... yet!
				}
				// Show instructions.
				else if (currentStep == s++) {
					CanTouchBoard = false; // ignore Board input! I'm da captain now!
					t_instructions.text = "Press to UNDO.";
					level.UndoMoveInputController.SetButtonsVisible(true);
					RectTransform rt_undoButton = level.UndoMoveInputController.rt_undoButton;
					GameUtils.ParentAndReset(i_arrow.gameObject, rt_undoButton);
					Vector2 arrowPos = new Vector2(0, rt_undoButton.rect.height*0.5f+20);
					StartAnimation_BounceArrow(arrowPos, 0);
					timeUntilNextStep = 0.4f; // Wait a moment for the player to read the popup.
				}
				// Wait for input.
				else if (currentStep == s++) {
				}
				// Wait for them to undo the move.
				else if (currentStep == s++) {
					t_instructions.text = "";
					StopAnimation_BounceArrow();
					CanTouchBoard = true;
//					go_findTheseWordsPopup.SetActive(false);
				}
//				// Show second popup.
//				else if (currentStep == s++) {
//					go_useAHintPopup.SetActive(true);
//					GameUtils.ParentAndReset(i_arrow.gameObject, getHintUI.transform);
//					Vector2 arrowPos = new Vector2(0, 60);
//					StartAnimation_BounceArrow(arrowPos, 0);
//					timeUntilNextStep = 0.4f; // Wait a moment for the player to read the popup.
//				}
//				// Wait for input.
//				else if (currentStep == s++) {
//				}
				// End sequence!
				else {
					EndSequence();
				}
			}

		}



		// ----------------------------------------------------------------
		//  Coroutine Animations
		// ----------------------------------------------------------------
		private void StopAnimation_BounceArrow() {
			if (bounceArrowCoroutine!=null) { StopCoroutine(bounceArrowCoroutine); }
			i_arrow.gameObject.SetActive(false);
		}
		private void StartAnimation_BounceArrow(Vector2 arrowPos, float rotation) {
			StopAnimation_BounceArrow();
			bounceArrowCoroutine = StartCoroutine(Coroutine_BounceArrow(arrowPos, rotation));
		}
		private IEnumerator Coroutine_BounceArrow(Vector2 arrowPos, float rotation) {
			i_arrow.gameObject.SetActive(true);
			i_arrow.transform.localEulerAngles = new Vector3(0,0,rotation);

			float bounceDist = 50;
			Vector2 posB = arrowPos + new Vector2(Mathf.Sin(rotation*Mathf.Deg2Rad), Mathf.Cos(rotation*Mathf.Deg2Rad))*bounceDist;
			while (true) {
				float loc = Mathf.Abs(Mathf.Sin(Time.time*6f));
				i_arrow.rectTransform.anchoredPosition = Vector2.Lerp(arrowPos,posB, loc);
				yield return null;
			}
		}


		private void StopAnimation_FingerMoveTile() {
			if (fingerMoveTileCoroutine!=null) { StopCoroutine(fingerMoveTileCoroutine); }
			i_dragFinger.gameObject.SetActive(false);
		}
		private void StartAnimation_FingerMoveTile(Vector2[] boardPoses, float delay=0f) {
			StopAnimation_FingerMoveTile();
			fingerMoveTileCoroutine = StartCoroutine(Coroutine_FingerMoveTile(boardPoses, delay));
		}
		private IEnumerator Coroutine_FingerMoveTile(Vector2[] boardPoses, float delay) {
			// Wait for the delay, yo.
			yield return new WaitForSeconds(delay);

			i_dragFinger.gameObject.SetActive(true);
			Vector2[] poses = BoardPosesToScreen(boardPoses);

			// Start finger gently entering to right position.
			LeanTween.value(i_dragFinger.gameObject, SetDragFingerPos,
				poses[0] + new Vector2(10,-10),
				poses[0],
				0.6f).setEaseOutBack();
			i_dragFinger.rectTransform.localScale = Vector3.one*0.6f;
			LeanTween.scale(i_dragFinger.rectTransform, Vector3.one, 0.3f).setEaseOutSine();

			// Fade in.
			GameUtils.SetUIGraphicAlpha(i_dragFinger, 0);
			LeanTween.alpha(i_dragFinger.rectTransform, 0.8f, 0.12f);
			yield return new WaitForSeconds(0.4f);

			// Move it!
			for (int i=1; i<poses.Length; i++) { // start at the SECOND (we're already at the first).
				Vector2 posA = i_dragFinger.rectTransform.anchoredPosition;
				Vector2 posB = poses[i];
				float duration = 1f;
				LeanTween.value(i_dragFinger.gameObject, SetDragFingerPos, posA,posB, duration).setEaseOutQuart();
				//			LeanTween.moveLocal(i_dragFinger, poses[i], duration).setEaseOutQuint();
				yield return new WaitForSeconds(duration);
			}

			// Fade out!
			LeanTween.alpha(i_dragFinger.rectTransform, 0, 0.15f);
			yield return new WaitForSeconds(0.3f);

			// Finally, rinse and repeat!
			StartAnimation_FingerMoveTile(boardPoses);
		}



	}
}