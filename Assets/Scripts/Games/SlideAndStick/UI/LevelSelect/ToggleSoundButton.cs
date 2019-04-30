using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlideAndStick {
	public class ToggleSoundButton : MonoBehaviour {
		// Components
		[SerializeField] private Image i_body=null;
		// References
		[SerializeField] private Sprite s_bodyOn=null;
		[SerializeField] private Sprite s_bodyOff=null;

		// Getters (Private)
		private SoundManager sm { get { return GameManagers.Instance.SoundManager; } }


		// ----------------------------------------------------------------
		//  Start
		// ----------------------------------------------------------------
		private void Start() {
			UpdateBodySprite();
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void UpdateBodySprite() {
			i_body.sprite = sm.IsSfx ? s_bodyOn : s_bodyOff;
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void OnButtonClick() {
			sm.ToggleIsSound();
			UpdateBodySprite();
            GameManagers.Instance.EventManager.OnAnyButtonClick();
		}


	}
}