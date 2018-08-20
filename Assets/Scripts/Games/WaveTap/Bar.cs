using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class Bar : Prop {
        // Constants
        private const float Width = 700;
        private const float Height = 4f;
		// Components
		[SerializeField] private BoxCollider2D myCollider=null;
		[SerializeField] private Image i_body=null;
		[SerializeField] private Text t_numKnocksLeft=null;
		// Properties
		private int numKnocksLeft;

		// Getters (Public)
		public bool DidRapDuringContact { get; set; }
		public bool IsDone { get { return numKnocksLeft <= 0; } }
		// Getters (Private)
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
        private static Color GetBodyColor(int _numKnocksLeft) {
            switch (_numKnocksLeft) {
                case 0: return Color.clear;
                case 1: return new Color(121/255f, 228/255f, 246/255f);
                case 2: return new Color(154/255f, 212/255f, 90/255f);
                case 3: return new Color(253/255f, 211/255f, 92/255f);
                case 4: return new Color(239/255f, 101/255f, 111/255f);
                case 5: return new Color(220/255f, 116/255f, 197/255f);
                case 6: return new Color(122/255f, 100/255f, 189/255f);
                default: return Color.red; // Oops, haven't defined this color. :P
            }
        }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Level _level, Transform tf_parent, BarData _data) {
			InitializeAsProp(_level, tf_parent, _data);

			myRectTransform.offsetMin = myRectTransform.offsetMax = Vector2.zero;
			myRectTransform.anchoredPosition = _data.pos;
            i_body.rectTransform.sizeDelta = new Vector2(Width, Height);
            myCollider.size = new Vector2(Width, Height);//myRectTransform.sizeDelta; // set my "hitbox"

			SetNumKnocksLeft(_data.numKnocksReq);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetNumKnocksLeft(int _numKnocksLeft) {
			numKnocksLeft = _numKnocksLeft;
			t_numKnocksLeft.text = numKnocksLeft.ToString();
            bodyColor = GetBodyColor(numKnocksLeft);
            //float h = Random.Range(0f, 1f);
            //bodyColor = new ColorHSB(h, 0.8f, 1f).ToColor();
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		public void RapMe() {
			DidRapDuringContact = true;
			SetNumKnocksLeft(numKnocksLeft - 1);
		}
		public void OnMissMe() {
			bodyColor = Color.red;
		}

	}
}