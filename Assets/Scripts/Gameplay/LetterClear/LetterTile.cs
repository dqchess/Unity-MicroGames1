using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterClear {
    public class LetterTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform=null;
        [SerializeField] private Text textField=null;
        // Properties
        private bool isVowel;
        private bool isOnEnd; // true if I'm the first or last letter of a word.
        private char myChar;
        private char myCharLower; // we use THIS to compare letters, to make playing easier.
        private Color myColor;
        private int fontSize;
        private Rect myRect;
        private Vector2 posTarget;
        // References
        private WordTile myWordTile;

        // Getters (Public)
        public bool IsOnEnd { get { return isOnEnd; } }
        public bool IsVowel { get { return isVowel; } }
        public Vector2 PosTarget {
            get { return posTarget; }
            set { posTarget = value; }
        }
        public Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
        }
        private Vector2 pos {
            get { return Pos; }
            set {
                myRectTransform.anchoredPosition = value;
                UpdateMyRect();
            }
        }
        public char MyCharLower { get { return myCharLower; } }
        public Rect MyRect { get { return myRect; } }
        public float Width { get { return myRect.width; } }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(WordTile _myWordTile, char _myChar) {
            myWordTile = _myWordTile;
            myChar = _myChar;
            myCharLower = myChar.ToString().ToLower().ToCharArray()[0];
            // TEMP TEST everything's lowercase
            myChar = myCharLower;
            isVowel = WordUtils.IsVowel(myCharLower);
            textField.text = myChar.ToString();

            this.transform.SetParent(myWordTile.transform);
            this.transform.localScale = Vector3.one;
            this.transform.localPosition = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;

            // Set my color by my vowel status!
            if (isVowel) {
                myColor = new ColorHSB(0.3f, 1f, 0.7f, 0.95f).ToColor();;
            }
            else {
                myColor = new ColorHSB(0.5f, 1f, 0.7f, 0.95f).ToColor();;
            }
            textField.color = myColor;
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        public void SetIsOnEnd(bool _isOnEnd) {
            isOnEnd = _isOnEnd;
            if (isOnEnd) { textField.color = myColor; }
            else { textField.color = new Color(0,0,0, 0.8f); }
        }
        private void UpdateMyRect() {
            myRect = new Rect(Pos.x,Pos.y, textField.preferredWidth,textField.preferredHeight);
        }

        public void SetFontSize(int _fontSize) {
            fontSize = _fontSize;
            textField.fontSize = fontSize;
            UpdateMyRect();
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnMouseOver() {
            textField.fontSize = (int)(fontSize*1.2f);
        }
        public void OnMouseOut() {
            textField.fontSize = fontSize;
        }
        public void OnMatched() {
            myWordTile.RemoveLetter(this);
        }



        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate() {
            // Ease to target!
            if (pos != posTarget) {
                pos += (posTarget-pos) * 0.3f;
            }
        }


    }
}