using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveKeys {
	
    // Common
    public const string VOLUME_MUSIC = "VolumeMusic";
    public const string VOLUME_SFX= "VolumeSfx";


	// BouncePaint
	public const string BouncePaint_HighestLevelUnlocked = "BouncePaint_HighestLevelUnlocked";
	public const string BouncePaint_LastLevelPlayed = "BouncePaint_LastLevelPlayed";
	public static string BouncePaint_NumLosses(int levelIndex) {
		return "BouncePaint_NumLosses_Level" + levelIndex;
	}

	// WaveTap
	public const string WaveTap_HighestLevelUnlocked = "WaveTap_HighestLevelUnlocked";
	public const string WaveTap_LastLevelPlayed = "WaveTap_LastLevelPlayed";
	public static string WaveTap_NumLosses(int levelIndex) {
		return "WaveTap_NumLosses_Level" + levelIndex;
	}

}
