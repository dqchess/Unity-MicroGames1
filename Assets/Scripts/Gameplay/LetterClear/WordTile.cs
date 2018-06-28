using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterClear {
    public class WordTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        private List<LetterTile> letterTiles;
        // Properties
        private Rect myRect;
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
        //public List<LetterTile> LetterTiles { get { return letterTiles; } }
        public int NumLetters { get { return letterTiles.Count; } }
        public float Width { get { return myRect.width; } }
        public Rect MyRect { get { return myRect; } }
        public Vector2 Pos {
            get { return pos; }
            set {
                pos = value;
                UpdateLettersPosTargets();
            }
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
            UpdateMyRect();
        }

        private void UpdateLettersPosTargets() {
            float tempX = Pos.x;
            foreach (LetterTile tile in letterTiles) {
                tile.PosTarget = new Vector2(tempX, Pos.y);
                tempX += tile.Width;
            }
        }

        public void InsertLetterInList(int insertIndex, LetterTile letter) {
            letterTiles.Insert(insertIndex, letter);
            OnLettersChanged();
        }
        public void RemoveLetterFromList(LetterTile letter) {
            if (letterTiles.Contains(letter)) {
                letterTiles.Remove(letter);
            }
            OnLettersChanged();
        }
        public void DestroyLetter(LetterTile letter) {
            letterTiles.Remove(letter);
            Destroy(letter.gameObject);
            OnLettersChanged();
        }

        private void OnLettersChanged() {
            UpdateLettersOnEnd();
            UpdateMyRect();
        }

        private void UpdateMyRect() {
            myRect = new Rect();
            foreach (LetterTile tile in letterTiles) {
                myRect.size = new Vector2(myRect.size.x+tile.Width, Mathf.Max(myRect.size.y, tile.Height));
            }
            myRect.position = Pos;
        }
        private void UpdateLettersOnEnd() {
            for (int i=0; i<letterTiles.Count; i++) {
                letterTiles[i].SetIsOnEnd(i==0 || i==letterTiles.Count-1);
            }
        }

        public void InsertLetter(LetterTile newLetter, Vector2 insertPos) {
            // Decide where to put de lime.
            int insertIndex = 0;
            for (int i=0; i<letterTiles.Count; i++) {
                if (insertPos.x > letterTiles[i].PosTarget.x) { insertIndex = i; }
            }
            // Put de lime in de coconut.
            newLetter.TransferToWord(this, insertIndex);
        }


    }
}