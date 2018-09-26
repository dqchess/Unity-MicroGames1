using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearchScroll {
	public class WordManager {
		// Properties
		private string[] words_common10000; // 10,000 common words.

		// Getters (Public)
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


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public WordManager() {
			words_common10000 = TextUtils.GetStringArrayFromResourcesTextFile("Games/WordSearchScroll/wordsCommon10000");
		}



	}


}