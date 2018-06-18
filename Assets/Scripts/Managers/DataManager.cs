using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager {
	// Properties


	// ----------------------------------------------------------------
	//  Getters
	// ----------------------------------------------------------------


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	public DataManager() {
		Reset ();
	}
	private void Reset () {
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void ClearAllSaveData() {
		// NOOK IT
		SaveStorage.DeleteAll ();
		Reset ();
		Debug.Log ("All SaveStorage CLEARED!");
	}




}


