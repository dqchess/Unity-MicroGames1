using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandGenParams : MonoBehaviour {
	//		[Header ("Random Tile Params")]
	//		public float MinPercentTiles = 0.4f;
	//		public float MinPercentTiles = 0.4f;
	public float PercentTiles;
	public int NumColors;
	public int NumWalls;
	public int Stickiness;


	private void Awake() {
		// Load!
		PercentTiles = SaveStorage.GetFloat(SaveKeys.SlideAndStick_RandGenPercentTiles, 0.8f);
		NumColors = SaveStorage.GetInt(SaveKeys.SlideAndStick_RandGenNumColors, 3);
		NumWalls = SaveStorage.GetInt(SaveKeys.SlideAndStick_RandGenNumWalls, 0);
		Stickiness = SaveStorage.GetInt(SaveKeys.SlideAndStick_RandGenStickiness, 1);
	}
	private void OnDestroy() {
		// Save!
		SaveStorage.SetFloat(SaveKeys.SlideAndStick_RandGenPercentTiles, PercentTiles);
		SaveStorage.SetInt(SaveKeys.SlideAndStick_RandGenNumColors, NumColors);
		SaveStorage.SetInt(SaveKeys.SlideAndStick_RandGenNumWalls, NumWalls);
		SaveStorage.SetInt(SaveKeys.SlideAndStick_RandGenStickiness, Stickiness);
	}

}
