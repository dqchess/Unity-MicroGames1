using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour {
	// Components
//	[SerializeField] private Button b_quit=null;
	[SerializeField] private GameObject go_loadingOverlay=null;
//	[SerializeField] private Text t_levelName=null;


	// ----------------------------------------------------------------
	//  Awake / Destroy
	// ----------------------------------------------------------------
	private void Awake () {
		// Add event listeners!
		GameManagers.Instance.EventManager.SetIsLevelCompletedEvent += OnSetIsLevelCompleted;
	}
	private void OnDestroy () {
		// Remove event listeners!
		GameManagers.Instance.EventManager.SetIsLevelCompletedEvent -= OnSetIsLevelCompleted;
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void ShowLoadingOverlay() {
		go_loadingOverlay.SetActive(true);
	}


	// ----------------------------------------------------------------
	//  Game Events
	// ----------------------------------------------------------------
	private void OnSetIsLevelCompleted (bool isLevelCompleted) {
//		if (isLevelCompleted) {
//			// Update hint text to reveal the intended word!
//			string revealedWords = GetTargetWordsString(currentLevel.TargetWords);
//			tmp_levelHint.text = currentLevel.Hint + ":   " + revealedWords.ToUpper();
//			// Show dat popup!
//			levelCompletePopup.OnCompletedLevel(currentLevel, revealedWords);
//		}
	}





}









