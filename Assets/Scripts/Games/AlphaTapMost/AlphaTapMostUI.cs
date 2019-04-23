using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaTapMost {
	public class AlphaTapMostUI : MonoBehaviour {
		// Components
		[SerializeField] private Text t_timeLeft=null;
		[SerializeField] private Text t_numCorrectTaps=null;


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
}