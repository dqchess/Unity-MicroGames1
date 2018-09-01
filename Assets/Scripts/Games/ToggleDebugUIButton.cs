using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDebugUIButton : MonoBehaviour {
	// Properties
	private bool isDebugUIVisible = false;

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	public void OnClick() {
		ToggleDebugUIVisible();
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void ToggleDebugUIVisible() {
		isDebugUIVisible = !isDebugUIVisible;
		GameManagers.Instance.EventManager.OnSetDebugUIVisible(isDebugUIVisible);
	}

}
