using UnityEngine;
using System.Collections;

public static class SceneNames {
	public const string GameSelect = "GameSelect";

	static public string Gameplay(string gameName) {
		return gameName + ""; // e.g. "BouncePaint".
	}
	static public string LevelSelect(string gameName) {
		return gameName + "LevelSelect"; // e.g. "BouncePaintLevelSelect".
	}
	static public string MainMenu(string gameName) {
		return gameName + "MainMenu"; // e.g. "BouncePaintMainMenu".
	}

	public const string AlphaTapMatch = "AlphaTapMatch";
	public const string AlphaTapMost = "AlphaTapMost";
	public const string AlphaTapOrder = "AlphaTapOrder";

//    public const string BouncePaint_LevelSelect = "BouncePaintLevelSelect";
//    public const string BouncePaint_MainMenu = "BouncePaintMainMenu";
//    public const string BouncePaint_Gameplay = "BouncePaint";

    public const string ExtrudeMatch = "ExtrudeMatch";

    public const string LetterClear = "LetterClear";

    public const string WaveTap = "WaveTap";
}
