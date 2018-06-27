using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterClear {
    public class WordTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        public List<LetterTile> letterTiles;//QQQ private
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
        public Vector2 Pos {
            get { return pos; }
            set {
                pos = value;
                UpdateLettersPositions();
            }
        }
        public float GetWidth() {
            float total = 0;
            foreach (LetterTile tile in letterTiles) {
                total += tile.Width;
            }
            return total;
        }
        public LetterTile GetLetterAtPoint(Vector2 point) {
            foreach (LetterTile tile in letterTiles) {
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
                tempX += tile.Width;
            }
        }


    }
}