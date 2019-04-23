using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaTapOrder {
	public class GameUI : MonoBehaviour {
		// Components
		[SerializeField] private Text t_levelName=null;


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start () {
//			UpdateCorrectTaps(0);
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void UpdateLevelName(int levelIndex) {
			t_levelName.text = levelIndex.ToString();
		}


	}
}