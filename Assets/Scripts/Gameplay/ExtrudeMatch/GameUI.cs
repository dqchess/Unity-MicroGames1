using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtrudeMatch {
	public class GameUI : MonoBehaviour {
		// Components
		[SerializeField] private Text t_score;


		private void Start () {
			UpdateScoreText(0);
		}


		public void UpdateScoreText(int score) {
			t_score.text = score.ToString();
		}

	}
}