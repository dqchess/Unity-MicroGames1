using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveKeys {
	
    // Common
    public const string VOLUME_MUSIC = "VolumeMusic";
    public const string VOLUME_SFX= "VolumeSfx";


	// BaseLevelGames
	static public string HighestLevelUnlocked(string gameName) {
		return gameName + "_HighestLevelUnlocked";
	}
	static public string LastLevelPlayed(string gameName) {
		return gameName + "_LastLevelPlayed";
	}
	static public string NumLosses(string gameName, int levelIndex) {
		return gameName + "_NumLosses_Level" + levelIndex;
	}

    // ExtrudeMatch
    public const string ExtrudeMatch_BestScore = "ExtrudeMatch_BestScore";


//	// CircleGrow
//	public const string CircleGrow_HighestLevelUnlocked = "CircleGrow_HighestLevelUnlocked";
//	public const string CircleGrow_LastLevelPlayed = "CircleGrow_LastLevelPlayed";
//	public static string CircleGrow_NumLosses(int levelIndex) {
//		return "CircleGrow_NumLosses_Level" + levelIndex;
//	}
//	// WaveTap
//	public const string WaveTap_HighestLevelUnlocked = "WaveTap_HighestLevelUnlocked";
//	public const string WaveTap_LastLevelPlayed = "WaveTap_LastLevelPlayed";
//	public static string WaveTap_NumLosses(int levelIndex) {
//		return "WaveTap_NumLosses_Level" + levelIndex;
//	}

}
