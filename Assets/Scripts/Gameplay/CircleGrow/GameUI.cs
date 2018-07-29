using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	public class GameUI : MonoBehaviour {
		// Components
		[SerializeField] private Text t_score;

		public void SetScoreText(float score) {
			t_score.text = Mathf.RoundToInt(score).ToString();
		}


	}
}