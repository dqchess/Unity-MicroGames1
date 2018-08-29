using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDebugUIButton : MonoBehaviour {
	// References
	[SerializeField] private GameObject go_debugUI;

	public void OnClick() {
		ToggleDebugUIVisible();
	}
	private void ToggleDebugUIVisible() {
		go_debugUI.SetActive(!go_debugUI.activeSelf);
	}

}
