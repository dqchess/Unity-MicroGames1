using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlideAndStick {
	public class LevelUI : MonoBehaviour {
        // Components
		[SerializeField] private TextMeshProUGUI t_levelName=null;
		// References
		[SerializeField] private Level level=null;


        // ----------------------------------------------------------------
        //  Start
        // ----------------------------------------------------------------
		private void Start () {
			t_levelName.text = "LEVEL " + level.LevelIndex.ToString();
		}


	}
}