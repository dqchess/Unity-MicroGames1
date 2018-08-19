using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTap {
	public class Bar : Prop {
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


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Level _level, Transform tf_parent, BarData _data) {
			InitializeAsProp(_level, tf_parent, _data);

			myRectTransform.offsetMin = myRectTransform.offsetMax = Vector2.zero;
			myRectTransform.anchoredPosition = _data.pos;
			myCollider.size = new Vector2(800, 2);//myRectTransform.sizeDelta; // set my "hitbox"

			SetNumKnocksLeft(_data.numKnocksReq);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetNumKnocksLeft(int _numKnocksLeft) {
			numKnocksLeft = _numKnocksLeft;
			t_numKnocksLeft.text = numKnocksLeft.ToString();
//			myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
			if (numKnocksLeft > 0) { // Still more knocks left? Rando my colo!
				float h = Random.Range(0f, 1f);
				bodyColor = new ColorHSB(h, 0.8f, 1f).ToColor();
			}
			else { // We're done being knocked! Go green!
				bodyColor = Color.green;
			}
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