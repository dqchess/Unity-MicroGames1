using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using System.Reflection;


public class BuildInfo {
    static public System.DateTime BuildTime() {
        #if UNITY_EDITOR
        return System.DateTime.Now;
        #else
        //return System.DateTime.Parse(Date.ToLongDateString());
        //return System.DateTime.Parse(BuildtimeInfo.DateTimeString());NOTE: DISABLED this! (Not worth my time right now.)
        return System.DateTime.MinValue;
        #endif
    }
    //public static System.Version Version() {
    //    return Assembly.GetExecutingAssembly().GetName().Version;
    //}
    
    //public static System.DateTime BuildDate() {
    //    System.Version version = Version();
    //    System.DateTime startDate = new System.DateTime( 2000, 1, 1, 0, 0, 0 );
    //    System.TimeSpan span = new System.TimeSpan( version.Build, 0, 0, version.Revision * 2 );
    //    System.DateTime buildDate = startDate.Add( span );
    //    return buildDate;
    //}
    
    //public static string ToString(string format=null) {
    //    System.DateTime date = Date();
    //    return date.ToString(format);
    //}
}
public class DebugController : MonoBehaviour {
	// Constants
	private const int NumTapsReq = 5;
	// Properties
	private int numCorrectTaps=0; // if we tap 5 times in the top-left of the screen (consecutively), we'll unlock Debug features!
	// Components
	[SerializeField] private TMPro.TextMeshProUGUI t_buildDate=null;
	// References
	[SerializeField] private GameObject go_toggleDebugUI=null; // the BUTTON that does the visibility-toggling.


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start() {
		UpdateToggleDebugUIActive();

		t_buildDate.text = BuildInfo.BuildTime().ToString();
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void UnlockDebug() {
		GameProperties.IsDebugFeatures = true;
		UpdateToggleDebugUIActive();
	}
	private void UpdateToggleDebugUIActive() {
		go_toggleDebugUI.SetActive(GameProperties.IsDebugFeatures);
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
		RegisterMouseInput();
	}
	private void RegisterMouseInput() {
		if (Input.GetMouseButtonDown(0)) {
			// Correct tap!
			if (Input.mousePosition.x<80 && Input.mousePosition.y<80) {
				numCorrectTaps ++;
				if (numCorrectTaps >= NumTapsReq) {
					UnlockDebug();
				}
			}
			// Incorrect tap.
			else {
				numCorrectTaps = 0;
			}
		}
	}


}
