using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugToggleControlMode : MonoBehaviour {
	// Components
	[SerializeField] private Text myText;


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		UpdateText();
	}
	

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void UpdateText() {
		myText.text = "controls:\n";
		if (GameProperties.CirclePop_Debug_TapAndHold) {
			myText.text += "tap-and-hold";
		}
		else {
			myText.text += "tap twice";
		}
	}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	public void OnClick() {
		// Toggle the control mode!
		GameProperties.CirclePop_Debug_TapAndHold = !GameProperties.CirclePop_Debug_TapAndHold;
		UpdateText();
	}



}
