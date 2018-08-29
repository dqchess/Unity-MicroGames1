using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildInfo {
	static public System.DateTime BuildTime() {
		#if UNITY_EDITOR
		return System.DateTime.Now;
		#else
		return System.DateTime.Parse(BuildtimeInfo.DateTimeString());
		#endif
	}
}


public class DebugController : MonoBehaviour {
	// Constants
	private const int NumTapsReq = 5;
	// Properties
	private int numCorrectTaps=0; // if we tap 5 times in the top-left of the screen (consecutively), we'll unlock Debug features!
	// Components
	[SerializeField] private TMPro.TextMeshProUGUI t_buildDate;
	// References
	[SerializeField] private GameObject go_toggleDebugUI=null;


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start() {
		UpdateToggleDebugUIActive();

		t_buildDate.text = BuildInfo.BuildTime().ToString();
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void UnlockDebug() {
		GameProperties.IsDebugFeatures = true;
		UpdateToggleDebugUIActive();
	}
	private void UpdateToggleDebugUIActive() {
		go_toggleDebugUI.SetActive(GameProperties.IsDebugFeatures);
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
		RegisterMouseInput();
	}
	private void RegisterMouseInput() {
		if (Input.GetMouseButtonDown(0)) {
			// Correct tap!
			if (Input.mousePosition.x<80 && Input.mousePosition.y<80) {
				numCorrectTaps ++;
				if (numCorrectTaps >= NumTapsReq) {
					UnlockDebug();
				}
			}
			// Incorrect tap.
			else {
				numCorrectTaps = 0;
			}
		}
	}


}
