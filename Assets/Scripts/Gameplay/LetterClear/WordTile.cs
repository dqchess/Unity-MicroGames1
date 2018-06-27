using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterClear {
    public class WordTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        private List<LetterTile> letterTiles;
        // Properties
        private string myWord;
        private Vector2 pos; // NOTE: Not my actual physical pos. Just used for my LetterTiles.
        // References
        private GameController gameController;

        // Getters (Private)
        private ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
        // Getters (Public)
        //public Vector2 Pos {
        //    get { return myRectTransform.anchoredPosition; }
        //    set { myRectTransform.anchoredPosition = value; }
        //}
        public int NumLetters { get { return letterTiles.Count; } }
        public Vector2 Pos {
            get { return pos; }
            set {
                pos = value;
                UpdateLettersPosTargets();
            }
        }
        public float GetWidth() {
            float total = 0;
            foreach (LetterTile tile in letterTiles) {
                total += tile.Width;
            }
            return total;
        }
        public LetterTile GetAvailableLetterAtPoint(Vector2 point) {
            foreach (LetterTile tile in letterTiles) {
                if (!tile.IsOnEnd) { continue; } // Not on the end?? Skip it!
                if (tile.MyRect.Contains(point)) {
                    return tile;
                }
            }
            return null;
        }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(GameController _gameController, RectTransform _myRT, string _myWord) {
            gameController = _gameController;
            this.transform.SetParent(_myRT);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;
            this.transform.localPosition = Vector3.zero;
            myRectTransform.anchoredPosition = Vector2.zero; // top-left align!
            myWord = _myWord;

            // Initialize my tiles!!
            char[] charArray = myWord.ToCharArray();
            letterTiles = new List<LetterTile>();
            foreach (char c in charArray) {
                LetterTile newObj = Instantiate(resourcesHandler.letterClear_letterTile).GetComponent<LetterTile>();
                newObj.Initialize(this, c);
                letterTiles.Add(newObj);
            }
            UpdateLettersOnEnd();
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetFontSize(int fontSize) {
            foreach (LetterTile tile in letterTiles) {
                tile.SetFontSize(fontSize);
            }
            UpdateLettersPosTargets();
        }

        private void UpdateLettersPosTargets() {
            float tempX = Pos.x;
            foreach (LetterTile tile in letterTiles) {
                tile.PosTarget = new Vector2(tempX, Pos.y);
                tempX += tile.Width;
            }
        }

        public void RemoveLetter(LetterTile letter) {
            letterTiles.Remove(letter);
            Destroy(letter.gameObject);
            UpdateLettersOnEnd();
        }
        private void UpdateLettersOnEnd() {
            for (int i=0; i<letterTiles.Count; i++) {
                letterTiles[i].SetIsOnEnd(i==0 || i==letterTiles.Count-1);
            }
        }


    }
}