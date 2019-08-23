using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Facebook.Unity;


public class AnalyticsController : MonoBehaviour {
	// Constants
    private const string AppEventName_LevelLose = "LevelLose";
    private const string AppEventName_LevelWin = "WinLevel";

    // Instance
    static private AnalyticsController instance=null;

    // Getters
    static public AnalyticsController Instance {
        get {
            // Safety check for runtime compile.
            if (instance == null) { instance = FindObjectOfType<AnalyticsController>(); }
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
	

    private void ReportCustomEvent(string title, Dictionary<string, object> myParams) {
        Analytics.CustomEvent(title, myParams);
    }
    private int NumTimesWonLevel(string gameName, int levelIndex) {
        return SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelIndex));
    }
    
    // ----------------------------------------------------------------
    //  Gameplay Events!
	// ----------------------------------------------------------------
	public void OnLoseLevel(string gameName, int levelIndex, float timeSpentThisPlay) {
        var parameters = new Dictionary<string, object> {
            ["Game"] = gameName,
            ["Level"] = levelIndex,
            ["timeSpentThisPlay"] = timeSpentThisPlay
        };

        ReportCustomEvent(AppEventName_LevelLose, parameters);
	}
    public void OnWinLevel(string gameName, int levelIndex) {
        if (NumTimesWonLevel(gameName, levelIndex) > 1) { return; } // Already won? Do nothing; only send win analytics on FIRST win.

        var parameters = new Dictionary<string, object> {
            ["Game"] = gameName,
            ["Level"] = levelIndex,
            ["numLosses"] = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelIndex), 0),
            ["timeSpentTotal"] = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelIndex), 0)
        };
        ReportCustomEvent(AppEventName_LevelWin, parameters);
    }
    public void OnWinLevel(string gameName, LevelAddress levelAddress, Dictionary<string,object> customParams=null) {
        //// If we've already won before, do NOTHING: Only send win analytic on the FIRST win.
        int numWins = SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelAddress));
        if (numWins > 1) { return; }

        var parameters = new Dictionary<string, object> {
            ["Game"] = gameName,
            ["Mode"] = levelAddress.mode,
            ["Collection"] = levelAddress.collection,
            ["Pack"] = levelAddress.pack,
            ["Level"] = levelAddress.level,
            ["numLosses"] = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelAddress), 0),
            ["timeSpentTotal"] = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelAddress), 0)
        };
        // We have custom params? Add 'em to parameters!
        if (customParams != null) {
            parameters.AddAllKVPFrom(customParams);
        }

        ReportCustomEvent("WinLevel", parameters);
    }


}
//*/


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Facebook.Unity;


//using Facebook.Unity;NOTE Commented out everything for analytics!

public class FBAnalyticsController : MonoBehaviour {
    // Constants
    private const string AppEventName_LevelLose = "LevelLose";
    public string fbAppID;

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

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
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
    

    void reportCustomEvent(string title, Dictionary<string, object> myParams)
    {
        Analytics.CustomEvent(title, myParams);
    }

    // ----------------------------------------------------------------
    //  Gameplay Events!
    // ----------------------------------------------------------------
    public void OnLoseLevel(string gameName, int levelIndex, float timeSpentThisPlay) {
        Debug.Log("reporting lose level...");
        var parameters = new Dictionary<string, object>();
        parameters["Game"] = gameName;
        parameters["Level"] = levelIndex;
        parameters["timeSpentThisPlay"] = timeSpentThisPlay;

        reportCustomEvent(AppEventName_LevelLose, parameters);

        //FB.LogAppEvent(
        //  AppEventName_LevelLose,
        //  null,
        //  parameters
        //);
    }
    public void OnWinLevel(string gameName, int levelIndex) {
        Debug.Log("reporting win level...");
        // If we've already won before, do NOTHING: Only send win analytic on the FIRST win.
        int numWins = SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelIndex));
        if (numWins > 1) { return; }

        int numLosses = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelIndex), 0);
        float timeSpentTotal = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelIndex), 0);

        var parameters = new Dictionary<string, object>();
        parameters["Game"] = gameName;
        parameters["Level"] = levelIndex;
        parameters["numLosses"] = numLosses;
        parameters["timeSpentTotal"] = timeSpentTotal;

        reportCustomEvent("WinLevel", parameters);


        //FB.LogAppEvent(
        //    AppEventName.AchievedLevel,
        //    null,
        //    parameters
        //);
    }
    public void OnWinLevel(string gameName, LevelAddress levelAddress, Dictionary<string,object> customParams=null) {
        Debug.Log("reporting win level...");
        //// If we've already won before, do NOTHING: Only send win analytic on the FIRST win.
        int numWins = SaveStorage.GetInt(SaveKeys.NumWins(gameName,levelAddress));
        if (numWins > 1) { return; }

        int numLosses = SaveStorage.GetInt(SaveKeys.NumLosses(gameName,levelAddress), 0);
        float timeSpentTotal = SaveStorage.GetFloat(SaveKeys.TimeSpentTotal(gameName,levelAddress), 0);

        var parameters = new Dictionary<string, object>();
        parameters["Game"] = gameName;
        parameters["Mode"] = levelAddress.mode;
        parameters["Collection"] = levelAddress.collection;
        parameters["Pack"] = levelAddress.pack;
        parameters["Level"] = levelAddress.level;
        parameters["numLosses"] = numLosses;
        parameters["timeSpentTotal"] = timeSpentTotal;
        // We have custom params? Add 'em to parameters!
        if (customParams != null) {
            parameters.AddAllKVPFrom(customParams);
        }

        reportCustomEvent("WinLevel", parameters);

        //FB.LogAppEvent(
        //    AppEventName.AchievedLevel,
        //    null,
        //    parameters
        //);
    }


}

//*/