using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaTapUI : MonoBehaviour {
	// Components
	[SerializeField] private Text t_timeLeft;
	[SerializeField] private Text t_numCorrectTaps;


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		UpdateCorrectTaps(0);
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void UpdateCorrectTaps(int numCorrectTaps) {
		t_numCorrectTaps.text = numCorrectTaps.ToString();
	}
	public void UpdateTimeLeft(float timeLeft) {
		t_timeLeft.text = TextUtils.ToTimeString_ms(timeLeft, true);
		t_timeLeft.color = timeLeft < 3 ? Color.yellow : Color.white;
	}


}
