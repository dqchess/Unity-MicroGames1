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
    static public string LevelsFilePath(string gameName, bool useFullPath=false) {
        string returnPath = "Games/" + gameName + "/Levels";
        if (useFullPath) {
            returnPath = Application.dataPath + "/Resources/" + returnPath;
        }
        return returnPath;
    }
	public string GetLevelString(int levelIndex) {
		if (levelIndex<0 || levelIndex>=levelStrings.Length) {
			Debug.LogError("LevelIndex out of range: " + levelIndex);
			return "LEVEL - \n."; // just an empty level.
		}
		return levelStrings[levelIndex];
	}


	// ----------------------------------------------------------------
	//  Loading
	// ----------------------------------------------------------------
    public void ReloadLevelsFile(string gameName) {
        string localPath = LevelsFilePath(gameName);
        TextAsset textAsset = Resources.Load<TextAsset>(localPath);

        if (textAsset != null) {
            string levelsFile = textAsset.text;
            levelsFile = TextUtils.RemoveCommentedLines(levelsFile);
            levelStrings = levelsFile.Split(levelHeader, System.StringSplitOptions.None);
        }
        else {
            levelStrings = new string[0];
            Debug.LogError("Levels TextAsset not found! Resources local path: \"" + localPath + "\"");
        }

        //string filePath = Application.streamingAssetsPath + "/" + gameName + "/Levels.txt";
        //if (BetterStreamingAssets.FileExists(filePath)) {
        //    string levelsFile = BetterStreamingAssets.ReadAllText(filePath);
        //    levelsFile = TextUtils.RemoveCommentedLines(levelsFile);
        //    levelStrings = levelsFile.Split(levelHeader, System.StringSplitOptions.None);
        //}
        //else {
        //    levelStrings = new string[0];
        //    Debug.LogError("Levels file not found! filePath: \"" + filePath + "\"");
        //}

        //yield return null;


        //var www = new WWW(filePath);
        //yield return www;
        //if (!string.IsNullOrEmpty(www.error)) {
        //    Debug.LogError ("Can't read file! Full path: \"" + filePath + "\"");
        //}


		//string filePath = Application.streamingAssetsPath + "/" + gameName + "/Levels.txt";
        //if (File.Exists(filePath)) {
        //    StreamReader file = File.OpenText(filePath);
        //    string levelsFile = file.ReadToEnd();
        //    file.Close();
        //    levelsFile = TextUtils.RemoveCommentedLines(levelsFile);
        //    levelStrings = levelsFile.Split(levelHeader, System.StringSplitOptions.None);
        //}
        //else {
        //    levelStrings = new string[0];
        //    Debug.LogError("Levels file not found! filePath: \"" + filePath + "\"");
        //}
    }




}


