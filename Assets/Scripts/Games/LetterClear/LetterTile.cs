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
        private bool isWild;
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
        public bool IsWild { get { return isWild; } }
        public Vector2 PosTarget {
            get { return posTarget; }
			set { posTarget = value; 
				UpdateMyRect();}
        }
        public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
            set {
                myRectTransform.anchoredPosition = value;
                UpdateMyRect();
            }
        }
        public char MyCharLower { get { return myCharLower; } }
        public Rect MyRect { get { return myRect; } }
        public float Width { get { return myRect.width; } }
        public float Height { get { return myRect.height; } }
		public WordTile MyWordTile { get { return myWordTile; } }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(WordTile _myWordTile, char _myChar) {
            myWordTile = _myWordTile;
            myChar = _myChar;
            myCharLower = myChar.ToString().ToLower().ToCharArray()[0];
            // TEST everything's lowercase
            myChar = myCharLower;
			isWild = false;//myCharLower == 'e';// WordUtils.IsVowel(myCharLower);
            textField.text = myChar.ToString();

            this.transform.SetParent(myWordTile.transform);
            this.transform.localScale = Vector3.one;
            this.transform.localPosition = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;

            // Set my color by my vowel status!
            if (isWild) {
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
			float width = textField.preferredWidth;
			float height = textField.preferredHeight;
//			myRect = new Rect(Pos.x,Pos.y-height, width,height); // convert top-left alignment to standard bottom-left rect.
			myRect = new Rect(PosTarget.x,PosTarget.y-height, width,height); // convert top-left alignment to standard bottom-left rect.
        }

        public void SetFontSize(int _fontSize) {
            fontSize = _fontSize;
            textField.fontSize = fontSize;
            UpdateMyRect();
        }

        public void TransferToWord(WordTile newWord, int insertIndex) {
            myWordTile.RemoveLetterFromList(this);
            newWord.InsertLetterInList(insertIndex, this);
            myWordTile = newWord;
        }


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        public void OnMouseOver() {
//            textField.fontSize = (int)(fontSize*1.2f);
        }
        public void OnMouseOut() {
//            textField.fontSize = fontSize;
        }
        public void OnMatched() {
            myWordTile.DestroyLetter(this);
        }



        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate() {
            // Ease to target!
    //        if (Pos != posTarget) {
				//Pos += (posTarget-Pos) * 0.3f;
            //}
            // Hack: Just go to target for now.
            Pos = posTarget;
        }


    }
}