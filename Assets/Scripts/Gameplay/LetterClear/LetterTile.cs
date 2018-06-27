using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterClear {
    public class LetterTile : MonoBehaviour {
        // Components
        [SerializeField] private RectTransform myRectTransform;
        [SerializeField] private Text textField;
        // Properties
        private bool isVowel;
        private char myChar;
        private Color myColor;
        private int fontSize;
        private Rect myRect;
        // References
        private WordTile myWordTile;

        // Getters (Public)
        public Vector2 Pos {
            get { return myRectTransform.anchoredPosition; }
            set {
                myRectTransform.anchoredPosition = value;
                UpdateMyRect();
            }
        }
        public Rect MyRect { get { return myRect; } }
        public float Width { get { return myRect.width; } }


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
        public void Initialize(WordTile _myWordTile, char _myChar) {
            myWordTile = _myWordTile;
            myChar = _myChar;
            isVowel = WordUtils.IsVowel(myChar);
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


    }
}