using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WordSearchScroll {
	public class LevelUI : MonoBehaviour {
		// Components
		[SerializeField] private TextMeshProUGUI t_wordsVisible0; // left column
		[SerializeField] private TextMeshProUGUI t_wordsVisible1; // right column

		public void OnSetWordsVisible(List<BoardWord> wordsVisible) {
			const int numWordsPerCol = 16;

			t_wordsVisible0.text = "";
			t_wordsVisible1.text = "";

			for (int i=0; i<wordsVisible.Count; i++) {
				BoardWord w = wordsVisible[i];
				TextMeshProUGUI text = i < numWordsPerCol ? t_wordsVisible0 : t_wordsVisible1;
				if (w.didFindMe) { text.text += "<color=#FFFFFF44>"; }
				text.text += w.word;
				text.text += "</color>\n";
			}
		}


	}
}