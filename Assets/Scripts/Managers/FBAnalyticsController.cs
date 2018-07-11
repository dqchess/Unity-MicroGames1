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
//          if (instance==null) { return this; } // Note: This is only here to prevent errors when recompiling code during runtime.
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
	

    // ----------------------------------------------------------------
    //  Gameplay Events!
    // ----------------------------------------------------------------
    public void BouncePaint_OnWinLevel(int levelIndex) {
        int numLosses = SaveStorage.GetInt(SaveKeys.BouncePaint_NumLosses(levelIndex),0);
        // TODO: Dispatch call specifying: BouncePaint, levelIndex, and numLosses!
        //FB.LogAppEvent(

    }


}
