using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbacusToy {
	public class UndoMoveButton : Button {
		// Properties
		private bool isButtonHeld = false; // Sigh. Unity's IsPressed() function isn't working.
		// References
		private UndoMoveInputController undoController;


		// ANNOYINGLY, Unity editor doesn't show undoController as a SerializedField ('cause we extend Button). So we need someone else to set my ref FOR me.
		public void SetUndoControllerRef(UndoMoveInputController _undoController) {
			undoController = _undoController;
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		public void Update () {
			if (isButtonHeld) {
				undoController.OnButton_Undo_Held ();
			}
		}

		// ----------------------------------------------------------------
		//  Button Events
		// ----------------------------------------------------------------
		override public void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData) {
			undoController.OnButton_Undo_Down ();
			isButtonHeld = true;
		}
		override public void OnPointerUp (UnityEngine.EventSystems.PointerEventData eventData) {
			undoController.OnButton_Undo_Up ();
			isButtonHeld = false;
		}

	}

}