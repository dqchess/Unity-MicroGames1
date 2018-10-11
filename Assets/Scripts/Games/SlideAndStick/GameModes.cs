static public class GameModes {
	public const string Tutorial = "Tutorial";
	public const string FindTheWord = "FindTheWord"; // Discrete, winnable levels. Classic, original mode. Use all letters in one word to win the level.
	public const string FindTheWordMulti = "FindTheWordMulti"; // FindTheWord, but with MULTIPLE words!
    public const string LetterSmash = "LetterSmash"; // Now this is just finding all wordsToMake! (USED to be: Discrete, winnable levels. The goal here is to use one or more letters X times!)
    public const string WordExhaust = "WordExhaust"; // Like FindTheWord, but we also encourage finding all OTHER words within the board!
    public const string PuzzlePath = "PuzzlePath";
	public const string Endless = "Endless"; // After each move, new letters are added.

	public const int TutorialIndex = 0;
	public const int FindTheWordIndex = 1;
    public const int FindTheWordMultiIndex = 2;
    public const int LetterSmashIndex = 3;
    public const int PuzzlePathIndex = 4;

//	static public string FromString(string str) {
//		switch (str) {
//			case FindTheWord: return FindTheWord;
//			case FindTheWord: return FindTheWord;
//			case FindTheWord: return FindTheWord;
//			case FindTheWord: return FindTheWord;
//			default: Debug.LogError("GameMode unrecognized: \"" + str + "\""); return str;
//		}
//	}
}