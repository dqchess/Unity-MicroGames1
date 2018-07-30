using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum CircleStates {
        Sleeping, Growing, Solidified
    }

	public class Circle : MonoBehaviour {
		// Constants
		static public readonly Color color_oscillating = new Color(250/255f, 200/255f, 110/255f);
		static public readonly Color color_solid = new Color(37/255f, 166/255f, 170/255f);
		static public readonly Color color_illegal = new Color(255/255f, 132/255f, 118/255f);
		// Components
		[SerializeField] private Image i_body=null;
		[SerializeField] private Text t_multiplier=null;
		[SerializeField] private RectTransform myRectTransform=null;
        // Properties
        private CircleStates currentState;
        private float growSpeed;
        private float radius;
		private int multiplier;

        // Getters (Public)
        public CircleStates CurrentState { get { return currentState; } }
        public float Radius { get { return radius; } }
		public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		public int ScoreValue() {
   //         float area = ;
			//return Mathf.CeilToInt(area * multiplier);
            return Mathf.CeilToInt(Area()/100f);
		}
		// Getters (Private)
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}
        private float Area() { return Mathf.PI * Radius*Radius; }
		private static int GetMultiplierForRadius(float _radius) {
			//if (_radius <  20) { return 1; }TEMP DISABLED
			//if (_radius <  40) { return 2; }
			//if (_radius <  60) { return 3; }
			//if (_radius <  80) { return 4; }
			//if (_radius < 100) { return 5; }
			//if (_radius < 120) { return 6; }
			//if (_radius < 140) { return 7; }
			//if (_radius < 160) { return 8; }
			//if (_radius < 180) { return 9; }
			return 10;
		}
		private static Color GetBodyColorFromMultiplier(int _mult) {
//			switch (_mult) {TEMP disabled until we make the color scheme.
//			case 0: return new ColorHSB(0,0,0).ToColor(); // Hmm.
//			default: return Color.black; // Hmm.
//			}
			//float h = (0.3f + _mult*0.08f) % 1;TEMP DISABLED this, too
			//return new ColorHSB(h, 0.5f, 1).ToColor();
            return color_oscillating;
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Transform tf_parent, Vector2 _pos, float _radius, float _growSpeed) {
			this.transform.SetParent(tf_parent);
			this.transform.localScale = Vector3.one;
			this.transform.localPosition = Vector3.zero;
			this.transform.localEulerAngles = Vector3.zero;

			Pos = _pos;
            growSpeed = _growSpeed;
			SetRadius(_radius);
            SetCurrentState(CircleStates.Sleeping);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
			// Update multiplier!
            multiplier = GetMultiplierForRadius(radius);
            //t_multiplier.text = "x" + multiplier.ToString();
            t_multiplier.text = TextUtils.AddCommas(ScoreValue());
			i_body.color = GetBodyColorFromMultiplier(multiplier);
        }
        private void SetCurrentState(CircleStates _state) {
            currentState = _state;
            // Only show my text if I'm NOT sleeping!
            t_multiplier.enabled = _state != CircleStates.Sleeping;
        }

        public void OnStartGrowing() {
            SetCurrentState(CircleStates.Growing);
        }
		public void OnSolidify() {
            bodyColor = color_solid;
            SetCurrentState(CircleStates.Solidified);
		}
		public void OnIllegalOverlap() {
			bodyColor = color_illegal;
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		public void GrowStep() {
			SetRadius(radius + growSpeed*Time.timeScale);
		}

	}
}
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
	public class Circle : MonoBehaviour {
		// Constants
		static public readonly Color color_oscillating = new Color(250/255f, 200/255f, 110/255f);
		static public readonly Color color_solid = new Color(37/255f, 166/255f, 170/255f);
		static public readonly Color color_illegal = new Color(255/255f, 132/255f, 118/255f);
		// Components
		[SerializeField] private Image i_body=null;
		[SerializeField] private RectTransform myRectTransform=null;
		// Properties
		private bool isOscillating;
		private float growSpeed = 0.8f;
		private float radius;
		//private Rect levelBounds;

		// Getters (Public)
		public bool IsOscillating { get { return isOscillating; } }
		public float Radius { get { return radius; } }
		public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		// Getters (Private)
		private Color bodyColor {
			get { return i_body.color; }
			set { i_body.color = value; }
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Transform tf_parent, Vector2 _pos, float _radius) {
			this.transform.SetParent(tf_parent);
			this.transform.localScale = Vector3.one;
			this.transform.localPosition = Vector3.zero;
			this.transform.localEulerAngles = Vector3.zero;

			Pos = _pos;
			SetRadius(_radius);
		}


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
		}
		public void SetIsOscillating(bool _isOscillating) {
			isOscillating = _isOscillating;
			bodyColor = isOscillating ? color_oscillating : color_solid;
		}

		public void OnIllegalOverlap() {
			SetIsOscillating(false);
			bodyColor = color_illegal;
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		private void Update() {
			if (isOscillating) {
				SetRadius(radius + growSpeed*Time.timeScale);
			}
		}

	}
}
*/