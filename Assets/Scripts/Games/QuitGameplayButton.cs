using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameplayButton : MonoBehaviour {

	public void OnClick() {
		GameManagers.Instance.EventManager.OnQuitGameplayButtonClick();
	}

}
