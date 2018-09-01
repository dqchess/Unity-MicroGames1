using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Add this to any GameObject you want ENABLED when DebugUI is visible, and DISABLED when it's not.
 * This GameObject itself is not affected: all its children are instead (so we can still listen for events).
 */
public class DebugUI : MonoBehaviour {

	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Start () {
		// Uhh, start hidden.
		OnSetDebugUIVisible(false);

		// Add event listeners!
		GameManagers.Instance.EventManager.SetDebugUIVisibleEvent += OnSetDebugUIVisible;
	}
	private void OnDestroy () {
		// Remove event listeners!
		GameManagers.Instance.EventManager.SetDebugUIVisibleEvent -= OnSetDebugUIVisible;
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnSetDebugUIVisible(bool isVisible) {
		SetChildrenActive(isVisible);
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void SetChildrenActive(bool isActive) {
		for (int i=0; i<transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(isActive);
		}
	}
}
