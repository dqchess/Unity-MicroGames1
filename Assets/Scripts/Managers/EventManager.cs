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
	public event BoolAction SetIsLevelCompletedEvent;
	public event AudioClipAction TriggerAudioClipEvent;


	// Events
	public void OnScreenSizeChanged () { if (ScreenSizeChangedEvent!=null) { ScreenSizeChangedEvent (); } }

	public void OnSetIsLevelCompleted (bool isLevelComplete) { if (SetIsLevelCompletedEvent!=null) { SetIsLevelCompletedEvent (isLevelComplete); } }


	public void TriggerAudioClip (AudioClip _clip) {if (TriggerAudioClipEvent != null) {TriggerAudioClipEvent (_clip);}}

}



