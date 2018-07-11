using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveKeys {
	
	public const string BouncePaint_LastLevelPlayed = "BouncePaint_LastLevelPlayed";
    public static string BouncePaint_NumLosses(int levelIndex) {
        return "BouncePaint_NumLosses_Level" + levelIndex;
    }

}
