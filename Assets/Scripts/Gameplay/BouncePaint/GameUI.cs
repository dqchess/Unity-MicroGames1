using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BouncePaint {
	public class GameUI : MonoBehaviour {
		// Components
		[SerializeField] private Text t_levelName;
		[SerializeField] private GameObject go_debugUI;


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start () {
			HideDebugUI();
		}

		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		public void UpdateLevelName(int levelIndex) {
			t_levelName.text = levelIndex.ToString();
		}

		public void ToggleDebugUI() { go_debugUI.SetActive(!go_debugUI.activeSelf); }
		private void HideDebugUI() { go_debugUI.SetActive(false); }
		private void ShowDebugUI() { go_debugUI.SetActive(true); }


	}
}