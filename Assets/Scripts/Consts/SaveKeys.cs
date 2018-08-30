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
	static public string TimeSpentTotal(string gameName, int levelIndex) { // The CUMULATIVE time a player's spent in THIS Level. 
		return gameName + "_TimeSpentTotal_Level" + levelIndex;
	}
	static public string BestScore(string gameName, int levelIndex) {
		return gameName + "_BestScore_Level" + levelIndex;
	}

    // ExtrudeMatch
    public const string ExtrudeMatch_BestScore = "ExtrudeMatch_BestScore";


}
