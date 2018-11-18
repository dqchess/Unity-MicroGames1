using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    public class RandGenParams : MonoBehaviour {
    	[HideInInspector] public int NumWalls;
    
    
    	private void Awake() {
    		// Load!
    		NumWalls = SaveStorage.GetInt(SaveKeys.SpoolOut_RandGenNumWalls, 0);
    	}
    	private void OnDestroy() {
    		// Save!
    		SaveStorage.SetInt(SaveKeys.SpoolOut_RandGenNumWalls, NumWalls);
    	}
    
    }
}
