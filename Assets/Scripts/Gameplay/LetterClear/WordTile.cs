using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterClear {
    public class WordTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform;
        private List<LetterTile> letterTiles;
        // Properties
        private string myWord;
        // References
        private GameController gameController;

        // Getters (Private)
        private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
        // Getters (Public)
        public Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
            set { myRectTransform.anchoredPosition = value; }
        }
        public float GetWidth() {
            float total = 0;
            foreach (LetterTile tile in letterTiles) {
                total += tile.GetWidth();
            }
            return total;
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(GameController _gameController, RectTransform _myRT, string _myWord) {
            gameController = _gameController;
            this.transform.SetParent(_myRT);
            this.transform.localScale = Vector3.one;
            this.transform.localPosition = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;
            myWord = _myWord;

            // Initialize my tiles!!
            char[] charArray = myWord.ToCharArray();
            letterTiles = new List<LetterTile>();
            foreach (char c in charArray) {
                LetterTile newObj = Instantiate(resourcesHandler.letterClear_letterTile).GetComponent<LetterTile>();
                newObj.Initialize(this, c);
                letterTiles.Add(newObj);
            }

        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetFontSize(int fontSize) {
            foreach (LetterTile tile in letterTiles) {
                tile.SetFontSize(fontSize);
            }
            UpdateLettersPositions();
        }


        private void UpdateLettersPositions() {
            float tempX = Pos.x;
            foreach (LetterTile tile in letterTiles) {
                tile.Pos = new Vector2(tempX, Pos.y);
                tempX += tile.GetWidth();
            }
        }


    }
}