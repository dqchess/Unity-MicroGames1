using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class WordManager {
		// Properties
		private string[] words_common10000; // 10,000 common words.
		private HashSet<string> allWords; // the official dictionary to check if a string is a word.

		// Getters (Public)
		public bool IsRealWord(string word) { return allWords.Contains(word); }
		public string[] GetRandomWords(int numWords) {
			List<string> words = new List<string>();
			int safetyCount=0;
			while (true) {
				string randWord = words_common10000[Random.Range(0,words_common10000.Length)];
				if (!words.Contains(randWord)) { // If we didn't already add it, do!
					words.Add(randWord);
				}
				// Got all the words? Break!
                if (words.Count >= numWords) {
                    break;
                }
				// Too many checks?? Break.
				if (safetyCount++ > 1000) {
                    Debug.LogError("Somehow can't find enough random words. Hmmmm.");
                    break;
				}
			}
            return words.ToArray();
		}
		static public bool IsCharInAlphabet(char c) {
			if (c >= 'a' && c <= 'z') { return true; }
			if (c >= 'A' && c <= 'Z') { return true; }
			return false;
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public WordManager() {
			words_common10000 = TextUtils.GetStringArrayFromResourcesTextFile("Games/WordSearchScroll/words_common10000");
			allWords = new HashSet<string>(TextUtils.GetStringArrayFromResourcesTextFile("Games/WordSearchScroll/words_scrabble"));
		}



	}


}