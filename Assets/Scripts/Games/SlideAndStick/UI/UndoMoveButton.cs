using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class UndoMoveButton : MonoBehaviour {
        // Components
        //[SerializeField] private DepthButtonExt dbe=null;
        [SerializeField] private CanvasGroup myCanvasGroup=null;
		// Properties
		private bool isButtonHeld = false; // Sigh. Unity's IsPressed() function isn't working.
		// References
		[SerializeField] private UndoMoveInputController undoController=null;


        //// ANNOYINGLY, Unity editor doesn't show undoController as a SerializedField ('cause we extend Button). So we need someone else to set my ref FOR me.
        //public void SetUndoControllerRef(UndoMoveInputController _undoController) {
        //	undoController = _undoController;
        //}

        private void Awake() {
            myCanvasGroup.alpha = 0;
        }


        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        public void Update () {
			if (isButtonHeld) {
				undoController.OnButton_Undo_Held();
			}
		}
        
        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetInteractable(bool isInteractable) {
            //dbe.interactable = isInteractable;HACK temp disabled 'cause visual issues I don't have time to address.
            //this.gameObject.SetActive(isInteractable);
            myCanvasGroup.alpha = isInteractable ? 1 : 0;
        }

		// ----------------------------------------------------------------
		//  Button Events
		// ----------------------------------------------------------------
        //override public void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData) {
        //    undoController.OnButton_Undo_Down ();
        //    isButtonHeld = true;
        //}
        //override public void OnPointerUp (UnityEngine.EventSystems.PointerEventData eventData) {
        //    undoController.OnButton_Undo_Up ();
        //    isButtonHeld = false;
        //}
        public void OnTouchDown() {
            undoController.OnButton_Undo_Down();
            isButtonHeld = true;
        }
        public void OnTouchUp() {
            undoController.OnButton_Undo_Up();
            isButtonHeld = false;
        }

	}

}
