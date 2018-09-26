using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WordSearchScroll {
	public class LevelUI : MonoBehaviour {
		// Components
		[SerializeField] private TextMeshProUGUI myText;

		public void OnSetWordsVisible(List<BoardWord> wordsVisible) {
			string str = "";

			for (int i=0; i<wordsVisible.Count; i++) {
				BoardWord w = wordsVisible[i];
				if (w.didFindMe) { str += "<color=#FFFFFF44>"; }
				str += w.word;
				str += "</color>\n";
			}

			myText.text = str;
		}


	}
}