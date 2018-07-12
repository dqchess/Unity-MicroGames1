using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBAnalyticsController : MonoBehaviour {
    // Instance
    static private FBAnalyticsController instance=null;

    // Getters
    static public FBAnalyticsController Instance {
        get {
            // Safety check for runtime compile.
            if (instance == null) { instance = GameObject.FindObjectOfType<FBAnalyticsController>(); }
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
        if (FB.IsInitialized) {
            FB.ActivateApp();
        }
		else {
            //Handle FB.Init
            FB.Init( () => {
                FB.ActivateApp();
            });
        }
	}
    // Unity will call OnApplicationPause(false) when an app is resumed from the background
    void OnApplicationPause (bool pauseStatus) {
        // Check the pauseStatus to see if we are in the foreground or background
        if (!pauseStatus) {
            // app resume
            if (FB.IsInitialized) {
                FB.ActivateApp();
            }
            else {
                // Handle FB.Init
                FB.Init( () => {
                    FB.ActivateApp();
                });
            }
        }
    }
	

    // ----------------------------------------------------------------
    //  Gameplay Events!
    // ----------------------------------------------------------------
    public void BouncePaint_OnWinLevel(int levelIndex) {
        int numLosses = SaveStorage.GetInt(SaveKeys.BouncePaint_NumLosses(levelIndex),0);

        var parameters = new Dictionary<string, object>();
        parameters["Game"] = "BouncePaint";
        parameters["numLosses"] = numLosses;
        parameters[AppEventParameterName.Level] = levelIndex;
        FB.LogAppEvent(
            AppEventName.AchievedLevel,
            null,
            parameters
        );
    }


}
