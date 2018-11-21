using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpoolOut {
    public class RandGenParams : MonoBehaviour {
		[HideInInspector] public int MinPathLength;
    	[HideInInspector] public int NumWalls;
    
    
    	private void Awake() {
			// Load!
			MinPathLength = SaveStorage.GetInt(SaveKeys.SpoolOut_RandGenMinPathLength, 2);
			NumWalls = SaveStorage.GetInt(SaveKeys.SpoolOut_RandGenNumWalls, 0);
    	}
    	private void OnDestroy() {
			// Save!
			SaveStorage.SetInt(SaveKeys.SpoolOut_RandGenMinPathLength, MinPathLength);
			SaveStorage.SetInt(SaveKeys.SpoolOut_RandGenNumWalls, NumWalls);
    	}
    
    }
}
