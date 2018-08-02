using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum GrowerStates { Sleeping, PreGrowing, Growing, Solidified }

	abstract public class Grower : Prop {
		// Constants
		protected const float HITBOX_SHRINK = 1; // 1 or 2 pixels is good. How much smaller we make our hit area than our image.
		// Constants
		static public  readonly Color color_sleeping = new Color(250/255f, 200/255f, 110/255f);
		static private readonly Color color_pregrowingA = color_sleeping;
		static private readonly Color color_pregrowingB = new Color(100/255f, 0/255f, 165/255f);
        static public  readonly Color color_growing = new Color(250/255f, 200/255f, 110/255f);
		static public  readonly Color color_solid = new Color(37/255f, 166/255f, 170/255f);
		static public  readonly Color color_illegal = new Color(255/255f, 132/255f, 118/255f);
		// Components
        [SerializeField] private Text t_scoreValue=null;
        // Properties
		private bool didIllegalOverlap=false; // true if we touched, OR were touched by, another Grower.
        private GrowerStates currentState;
        private float growSpeed;
        private float radius;

        // Getters (Public)
        public GrowerStates CurrentState { get { return currentState; } }
        public float Radius { get { return radius; } }
		public int ScoreValue() {
            if (currentState == GrowerStates.Sleeping) { return 0; } // Sleeping? I'm worth 0 points.
            return Mathf.CeilToInt(Area()/100f); // HARDCODED. Bring down the values into a reasonable range.
		}
		// Getters (Protected)
		override protected bool MayMove() {
			return base.MayMove() && currentState!=GrowerStates.Solidified;
		}
		override protected bool MayRotate() {
			return base.MayRotate() && currentState!=GrowerStates.Solidified;
		}
        // Getters (Private)
		private float Area() { return Mathf.PI * Radius*Radius; }
		private Vector2 GetCollisionPoint(Collision2D collision) {
			Vector2 point = collision.contacts[0].point;
            point = transform.parent.InverseTransformPoint(point); // Convert the point from world to Level space.
			return point;
		}


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		public void Initialize(Level _myLevel, Transform tf_parent, Vector2 _pos) {
			float startingRadius = 10; // our default. We can say otherwise while adding these guys (tack on a ".SetStartingRadius(50f)" function).
			Vector2 _size = new Vector2(startingRadius, startingRadius);
			BaseInitialize(_myLevel, tf_parent, _pos, _size);

            SetGrowSpeed(1);

            bodyColor = color_sleeping;
			SetRadius(startingRadius);
            SetCurrentState(GrowerStates.Sleeping);
		}

        public Grower SetGrowSpeed(float _speed) {
            growSpeed = _speed*0.8f; // awkward scaling the speed here.
            return this;
        }


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		virtual protected void SetRadius(float _radius) { // Override this so we can update my collider sizes! :)
			radius = _radius;
			myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
			// Update text!
            t_scoreValue.text = TextUtils.AddCommas(ScoreValue());
        }


        private void SetCurrentState(GrowerStates _state) {
            currentState = _state;
            // Only show my text if I'm a healthy, non-overlapped solid!
			t_scoreValue.enabled = _state==GrowerStates.Solidified && !didIllegalOverlap;
        }

		public void SetAsPreGrowing() {
			// Just set my state, and I'll flash in Update!
			SetCurrentState(GrowerStates.PreGrowing);
		}
		public void StartGrowing() {
			// Set my color, and we're off!
			bodyColor = color_growing;
			SetCurrentState(GrowerStates.Growing);
		}
		public void Solidify() {
			bodyColor = color_solid;
			SetCurrentState(GrowerStates.Solidified);
		}


		// ----------------------------------------------------------------
		//  Events
		// ----------------------------------------------------------------
		private void OnCollisionEnter2D(Collision2D collision) {
			if (!myLevel.IsGameStatePlaying) { return; } // If we're NOT playing (pre-game or level-over), ignore all collisions.
			if (currentState == GrowerStates.Sleeping) { return; } // Ignore sleeping beauties! (At least for now.)
			Prop otherProp = collision.collider.GetComponent<Prop>();
			// Illegal overlap!
			if (otherProp != null) {
				OnIllegalOverlap();
				otherProp.OnIllegalOverlap();
				Vector2 overlapPoint = GetCollisionPoint(collision);
				// Tell my Level!
				myLevel.OnIllegalOverlap(overlapPoint);
			}
		}
		override public void OnIllegalOverlap() {
			didIllegalOverlap = true;
			SetCurrentState(GrowerStates.Solidified);
			bodyColor = color_illegal;
		}


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		override protected void Update() {
			base.Update();
			if (Time.timeScale == 0) { return; } // No time? Do nothin'.

			UpdatePreGrowingFlash();
			UpdateGrowing();
		}
		private void UpdatePreGrowingFlash() {
			if (!myLevel.IsGameStatePlaying) { return; } // Not playing? Do nothing.
			// Am I pre-growing? Then flash!
			if (currentState==GrowerStates.PreGrowing) {
				float colorLoc = MathUtils.SinRange(-0.3f, 1.3f, Time.time*14f);
				bodyColor = Color.Lerp(color_pregrowingA, color_pregrowingB, colorLoc);
//				bodyColor = colorLoc<0.5f ? color_pregrowingA : color_pregrowingB;
			}
		}
		private void UpdateGrowing() {
			if (!myLevel.IsGameStatePlaying) { return; } // Not playing? Do nothing.
			// If I'm growing, then GROW!
			if (currentState==GrowerStates.Growing) {
				GrowStep();
			}
		}
		private void GrowStep() {
			// Grow, and tell my Level I grew (so we can update score)!
			SetRadius(radius + growSpeed*Time.timeScale);
			myLevel.OnGrowerGrowStep();
		}

	}
}



/*
        public void StartGrowing() {
            StartCoroutine(Coroutine_StartGrowing());
        }
        private IEnumerator Coroutine_StartGrowing() {
            // Pre-growing.
            SetCurrentState(GrowerStates.PreGrowing);
            // Flash first.
            bodyColor = color_growing;
            for (int i=0; i<5; i++) {
                bodyColor = i%2==0 ? color_growing : Color.black;
                yield return new WaitForSeconds(0.12f);
            }
            // Set my color, and we're off!
            bodyColor = color_growing;
            SetCurrentState(GrowerStates.Growing);
        }
		*/
/*
//      private static int GetMultiplierForRadius(float _radius) {
    //          //if (_radius <  20) { return 1; }
    //          //if (_radius <  40) { return 2; }
    //          //if (_radius <  60) { return 3; }
    //          //if (_radius <  80) { return 4; }
    //          //if (_radius < 100) { return 5; }
    //          //if (_radius < 120) { return 6; }
    //          //if (_radius < 140) { return 7; }
    //          //if (_radius < 160) { return 8; }
    //          //if (_radius < 180) { return 9; }
    //          return 10;
    //      }
    //      private static Color GetBodyColorFromMultiplier(int _mult) {
    ////            switch (_mult) { disabled until we make the color scheme.
    ////            case 0: return new ColorHSB(0,0,0).ToColor(); // Hmm.
    ////            default: return Color.black; // Hmm.
    ////            }
    //  //float h = (0.3f + _mult*0.08f) % 1; DISABLED this, too
    //  //return new ColorHSB(h, 0.5f, 1).ToColor();
    //          return color_oscillating;
    //}
    */


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