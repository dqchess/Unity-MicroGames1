using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLevelJumpButton : MonoBehaviour {
	// Properties
	[SerializeField] private int levelIndexChange; // how much LevelIndex changes when we're clicked! Sensible values: -10, -1, 1, 10.


	public void OnClick() {
		GameManagers.Instance.EventManager.OnLevelJumpButtonClick(levelIndexChange);
	}
}
