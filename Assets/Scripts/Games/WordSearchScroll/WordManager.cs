using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class WordManager {
		// Constants
		public static char[] alphabetChars = new char[]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
		// Properties
		private string[] words_common10000; // 10,000 common words.
		private HashSet<string> allWords; // the official dictionary to check if a string is a word.

		// Getters (Public)
		public bool IsRealWord(string word) { return word!=null && allWords.Contains(word.ToUpper()); }
		public string[] GetRandomWords(int numWords, int minWordLength=-1) {
			List<string> words = new List<string>();
			int safetyCount=0;
			while (true) {
				// Too many checks?? Break.
				if (safetyCount++ > 1000) {
					Debug.LogError("Somehow can't find enough random words. Hmmmm.");
					break;
				}
				string randWord = words_common10000[Random.Range(0,words_common10000.Length)];
				if (minWordLength>0 && randWord.Length<minWordLength) { // Too small? Keep lookin'.
					continue;
				}
				if (!words.Contains(randWord)) { // If we didn't already add it, do!
					words.Add(randWord);
				}
				// Got all the words? Break!
                if (words.Count >= numWords) {
                    break;
                }
			}
            return words.ToArray();
		}
		static public char RandAlphabetChar() { return alphabetChars[Random.Range(0,alphabetChars.Length)]; }
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