﻿using System.Collections;
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
        return gameName + "_NumLosses_Level_" + levelIndex;
    }
    static public string NumLosses(string gameName, LevelAddress address) {
        return gameName + "_NumLosses_Level_" + address.ToString();
    }
    static public string NumWins(string gameName, int levelIndex) {
        return gameName + "_NumWins_Level_" + levelIndex;
    }
    static public string NumWins(string gameName, LevelAddress address) {
        return gameName + "_NumWins_Level_" + address.ToString();
    }
    static public string TimeSpentTotal(string gameName, int levelIndex) { // The CUMULATIVE time a player's spent in THIS Level. 
        return gameName + "_TimeSpentTotal_Level_" + levelIndex;
    }
    static public string TimeSpentTotal(string gameName, LevelAddress address) { // The CUMULATIVE time a player's spent in THIS Level. 
        return gameName + "_TimeSpentTotal_Level_" + address.ToString();
    }
	static public string BestScore(string gameName, int levelIndex) {
		return gameName + "_BestScore_Level_" + levelIndex;
	}
    
    

    // ExtrudeMatch
    public const string ExtrudeMatch_BestScore = "ExtrudeMatch_BestScore";
    
    // SlideAndStick
    public const string SlideAndStick_RandGenPercentTiles = "SlideAndStick_RandGenPercentTiles";
    public const string SlideAndStick_NumColors = "SlideAndStick_NumColors";
    public const string SlideAndStick_RandGenStickiness = "SlideAndStick_RandGenStickiness";
    static public string SlideAndStick_LastPlayedLevelAddress(LevelAddress curAdd) {
        return "SlideAndStick_LastPlayedLevelAddress_" + curAdd.mode + "," + curAdd.collection + "," + curAdd.pack;
    }
    static public string SlideAndStick_DidCompleteLevel(LevelAddress address) {
        return "SlideAndStick_DidCompleteLevel_" + address.ToString();
    }

}
