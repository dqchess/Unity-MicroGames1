using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class LevelLoader : MonoBehaviour {
    // Properties
    private static string[] levelHeader  = new string[] { "LEVEL" };
    private string[] levelStrings; // The one Levels file is made up of these. Each starts at "LEVEL" header and ends at the next "LEVEL".


	// ----------------------------------------------------------------
	//  Getters
	// ----------------------------------------------------------------
	public string GetLevelString(int levelIndex) {
		if (levelIndex<0 || levelIndex>=levelStrings.Length) {
			Debug.LogError("LevelIndex out of range: " + levelIndex);
			return "";
		}
		return levelStrings[levelIndex];
	}


	// ----------------------------------------------------------------
	//  Loading
	// ----------------------------------------------------------------
	public void ReloadLevelsFile(string gameName) {
		string filePath = Application.streamingAssetsPath + "/" + gameName + "/Levels.txt";

        if (File.Exists(filePath)) {
            StreamReader file = File.OpenText(filePath);
            string levelsFile = file.ReadToEnd();
            file.Close();
            levelsFile = TextUtils.RemoveCommentedLines(levelsFile);
            levelStrings = levelsFile.Split(levelHeader, System.StringSplitOptions.None);
        }
        else {
            levelStrings = new string[0];
            Debug.LogError("Levels file not found! filePath: \"" + filePath + "\"");
        }
	}




}


