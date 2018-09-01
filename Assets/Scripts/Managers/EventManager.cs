using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager {
	// Actions and Event Variables
	public delegate void NoParamAction ();
	public delegate void BoolAction (bool _bool);
	public delegate void StringAction (string _str);
	public delegate void IntAction (int _int);
	public delegate void AudioClipAction (AudioClip _clip);

	public event NoParamAction ScreenSizeChangedEvent;
	public event NoParamAction RetryButtonClickEvent;
	public event NoParamAction QuitGameplayButtonClickEvent;
	public event BoolAction SetDebugUIVisibleEvent;
	public event AudioClipAction TriggerAudioClipEvent;
	public event IntAction LevelJumpButtonClickEvent;


	// Events
	public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }

	public void OnLevelJumpButtonClick(int levelIndexChange) { if (LevelJumpButtonClickEvent!=null) { LevelJumpButtonClickEvent (levelIndexChange); } }
	public void OnRetryButtonClick() { if (RetryButtonClickEvent!=null) { RetryButtonClickEvent(); } }
	public void OnSetDebugUIVisible(bool isVisible) { if (SetDebugUIVisibleEvent!=null) { SetDebugUIVisibleEvent(isVisible); } }
	public void OnQuitGameplayButtonClick() { if (QuitGameplayButtonClickEvent!=null) { QuitGameplayButtonClickEvent(); } }

	public void TriggerAudioClip (AudioClip _clip) {if (TriggerAudioClipEvent != null) {TriggerAudioClipEvent (_clip);}}

}



