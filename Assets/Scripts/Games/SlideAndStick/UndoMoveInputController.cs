using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	/** For cleanliness. Handles what happens when we hold down the Undo button.
Key presses are handled internally; UI Undo-Button presses I'm told about by Button_UndoMove.cs. */
	public class UndoMoveInputController : MonoBehaviour {
		// Properties
		private bool isUndoButtonEmphasized; // TRUE when we're in a known, hardcoded fail state! When TRUE, an arrow bounce-points at undo button.
		private float undoLoc; // when this hits past 1, we say to undo a move (and reset its value)!
		private float undoVel;
        private int numUndosSingleLeft; // saved/loaded!
        private Vector2 calloutArrowPosNeutral;
		// References
		[SerializeField] private Level level=null;
		[SerializeField] private Button restartLevelButton=null;
        [SerializeField] private Image i_calloutArrow=null;
        [SerializeField] private TextMeshProUGUI t_numUndosLeft=null;
        [SerializeField] private UndoMoveButton undoButton=null;

		// Getters (Public)
		public RectTransform rt_undoButton { get { return undoButton.GetComponent<RectTransform>(); } }


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start() {
			//// We can't SerializeField my ref for the button, so *I* have to tell the button who I am.
			//undoButton.SetUndoControllerRef(this);
            // Set calloutArrowPosNeutral.
            calloutArrowPosNeutral = i_calloutArrow.rectTransform.anchoredPosition;
		}



		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update () {
            RegisterButtonInput();
            UpdateEmphasizeButtons();
        }
        
        private void RegisterButtonInput() {
			bool isButton_undo_held = Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Z);
			bool isButton_undo_up = Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Z);
			bool isButton_undo_down = Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Z);

            // Shift + Undo = Undo All Moves!
            //if (Input.GetKey(KeyCode.LeftShift) && isButton_undo_down) { level.UndoAllMoves(); }
            if (Input.GetKeyDown(KeyCode.X)) { level.UndoAllMoves(); }
			else if (isButton_undo_up) { OnButton_Undo_Up (); }
			else if (isButton_undo_down) { OnButton_Undo_Down (); }
			else if (isButton_undo_held) { OnButton_Undo_Held (); }
        }

        private void UpdateEmphasizeButtons() {
			if (isUndoButtonEmphasized) {
                float loc = Mathf.Abs(MathUtils.Sin01(Time.unscaledTime*7f));
                loc = Mathf.Pow(loc, 0.25f);
                float yOffset = loc * 36;
                i_calloutArrow.rectTransform.anchoredPosition = calloutArrowPosNeutral + new Vector2(0, yOffset);
				//SetButtonsScale(0.9f + Mathf.Abs(Mathf.Sin(Time.unscaledTime*8f))*0.3f);
			}
		}

        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
		public void SetButtonsVisible(bool _isVisible) {
			undoButton.gameObject.SetActive(_isVisible);
			//restartLevelButton.gameObject.SetActive(_isVisible);//NOTE: Disabled restartLevelButton!
		}
//		private void SetButtonsScale(float scale) {
//			undoButton.transform.localScale = Vector3.one * scale;
////			restartLevelButton.transform.localScale = Vector3.one * scale;DISABLED scaling this one.
		//}
		private void UpdateCalloutArrowEnabledByFailState() {
			isUndoButtonEmphasized = level.Board != null && level.Board.IsInKnownFailState;
			//if (!areButtonsEmphasized) { // Stop emphasizing?
				//SetButtonsScale(1);
			//}
            i_calloutArrow.enabled = isUndoButtonEmphasized;
		}
        private void UpdateNumUndosLeftText() {
            t_numUndosLeft.text = level.GameController.NumUndosLeft.ToString();
        }
        
        
        
		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
        public void OnBoardMade() {
            SetButtonsVisible(true);
            UpdateNumUndosLeftText();
        }
        public void OnWinLevel() {
            SetButtonsVisible(false);
        }
		public void OnNumMovesMadeChanged(int numMovesMade) {
			undoButton.SetInteractable(numMovesMade > 0);
			restartLevelButton.interactable = numMovesMade > 0;
			UpdateCalloutArrowEnabledByFailState();
        }
        public void OnUndoMove() {
            UpdateNumUndosLeftText();
        }
        


		// ----------------------------------------------------------------
		//  Button Events
		// ----------------------------------------------------------------
		public void OnRestartLevelButtonClick() {
			level.UndoAllMoves();//.GameController.RestartLevel();//ReloadScene();
		}

		public void OnButton_Undo_Held() {
			// Update vel
			undoVel += 0.006f;
			//if (undoVel > 1f) { undoVel = 1f; } // Max vel! Note: Disabled, as one-per-frame limit is enough.
			// Apply vel
			undoLoc += undoVel;
			// Maybe undo!
			if (undoLoc > 1) {
				undoLoc = 0; // reset it hard to 0, instead of just subtracting 1, so we DON'T preserve that extra bit of momentum; we want to definitely only allow one undo per frame.
				level.UndoMoveAttempt();
			}
		}
		public void OnButton_Undo_Up() {

		}
		public void OnButton_Undo_Down() {
			// Do the first undo!
			level.UndoMoveAttempt();
			// Reset dees.
			undoLoc = 0; // reset dis.
			undoVel = 0; // reset dis too.
		}


	}
}