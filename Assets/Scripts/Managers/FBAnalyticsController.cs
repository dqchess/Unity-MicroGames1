using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Facebook.Unity;QQQ Commented out everything for analytics!

public class FBAnalyticsController : MonoBehaviour {
	// Constants
	private const string AppEventName_LevelLose = "LevelLose";
    // Instance
    static private FBAnalyticsController instance=null;

    // Getters
    static public FBAnalyticsController Instance {
        get {
            // Safety check for runtime compile.
            if (instance == null) { instance = FindObjectOfType<FBAnalyticsController>(); }
            return instance;
        }
    }



    // ----------------------------------------------------------------
    //  Awake
    // ----------------------------------------------------------------
    private void Awake () {
        // There can only be one (instance)!!
        if (instance != null) {
            Destroy (this.gameObject);
            return;
        }
        instance = this;
    }
	private void Start () {
  //      if (FB.IsInitialized) {
  //          FB.ActivateApp();
  //      }
		//else {
        //    //Handle FB.Init
        //    FB.Init( () => {
        //        FB.ActivateApp();
        //    });
        //}
	}
    // Unity will call OnApplicationPause(false) when an app is resumed from the background
    void OnApplicationPause (bool pauseStatus) {
        //// Check the pauseStatus to see if we are in the foreground or background
        //if (!pauseStatus) {
        //    // app resume
        //    if (FB.IsInitialized) {
        //        FB.ActivateApp();
        //    }
        //    else {
        //        // Handle FB.Init
        //        FB.Init( () => {
        //            FB.ActivateApp();
        //        });
        //    }
        //}
    }
	

    // ----------------------------------------------------------------
    //  Gameplay Events!
	// ----------------------------------------------------------------
	public void OnLoseLevel(string gameName, int levelIndex, float timeSpentThisPlay) {
		//var parameters = new Dictionary<string, object>();
		//parameters["Game"] = gameName;
		//parameters[AppEventParameterName.Level] = levelIndex;
		//parameters["timeSpentThisPlay"] = timeSpentThisPlay;
		//FB.LogAppEvent(
		//	AppEventName_LevelLose,
		//	null,
		//	parameters
		//);
	}
    public void OnWinLevel(string gameName, int levelIndex) {
        //// If we've already won before, do NOTHING: Only send win analytic on the FIRST win.
        //int numWins = SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelIndex));
        //if (numWins > 1) { return; }

        //int numLosses = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelIndex), 0);
        //float timeSpentTotal = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelIndex), 0);

        //var parameters = new Dictionary<string, object>();
        //parameters["Game"] = gameName;
        //parameters[AppEventParameterName.Level] = levelIndex;
        //parameters["numLosses"] = numLosses;
        //parameters["timeSpentTotal"] = timeSpentTotal;
        //FB.LogAppEvent(
        //    AppEventName.AchievedLevel,
        //    null,
        //    parameters
        //);
    }
    public void OnWinLevel(string gameName, LevelAddress levelAddress) {
        //// If we've already won before, do NOTHING: Only send win analytic on the FIRST win.
        //int numWins = SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelAddress));
        //if (numWins > 1) { return; }

        ////int numLosses = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelAddress), 0);
        //float timeSpentTotal = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelAddress), 0);

        //var parameters = new Dictionary<string, object>();
        //parameters["Game"] = gameName;
        //parameters[AppEventParameterName.Level] = levelAddress.level;
        //parameters["Collection"] = levelAddress.collection;
        ////parameters["numLosses"] = numLosses;
        //parameters["timeSpentTotal"] = timeSpentTotal;
        //FB.LogAppEvent(
        //    AppEventName.AchievedLevel,
        //    null,
        //    parameters
        //);
    }


}
