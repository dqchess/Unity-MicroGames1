using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircleGrow {
    public enum GrowerStates { Sleeping, PreGrowing, Growing, Solidified }
    public enum GrowerShapes { Circle, Square, Triangle }

	public class Grower : MonoBehaviour {
        // Constants
        static public readonly Color color_sleeping = new Color(250/255f, 200/255f, 110/255f);
        static public readonly Color color_growing = new Color(250/255f, 200/255f, 110/255f);
		static public readonly Color color_solid = new Color(37/255f, 166/255f, 170/255f);
		static public readonly Color color_illegal = new Color(255/255f, 132/255f, 118/255f);
		// Components
        [SerializeField] private Text t_scoreValue=null;
		[SerializeField] private RectTransform myRectTransform=null;
        private GrowerBody body;
        // Properties
        private GrowerStates currentState;
        private GrowerShapes shape;
        private float growSpeed;
        private float radius;
        private bool doMove;
        private float moveSpeed=1f; // for MOVING Growers.
        private float moveOscVal; // for MOVING Growers.
        private Vector2 posA,posB; // for MOVING Growers.
        // References
        private Level myLevel;

        // Getters (Public)
        public GrowerStates CurrentState { get { return currentState; } }
        public float Radius { get { return radius; } }
		public Vector2 Pos {
			get { return myRectTransform.anchoredPosition; }
			set { myRectTransform.anchoredPosition = value; }
		}
		public int ScoreValue() {
            if (currentState == GrowerStates.Sleeping) { return 0; } // Sleeping? I'm worth 0 points.
            return Mathf.CeilToInt(Area()/100f); // HARDCODED. Bring down the values into a reasonable range.
		}
        // Getters (Private)
        private Color bodyColor {
            get { return body.color; }
            set { body.color = value; }
        }
        private float timeScale { get { return Time.timeScale; } }
        private float Area() { return Mathf.PI * Radius*Radius; }


		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
        public void Initialize(Level _myLevel, Transform tf_parent, Vector2 _pos, GrowerShapes _shape, float _radius) {
            myLevel = _myLevel;

            this.transform.SetParent(tf_parent);
			this.transform.localScale = Vector3.one;
			this.transform.localPosition = Vector3.zero;
			this.transform.localEulerAngles = Vector3.zero;

            posA = posB = _pos; // default BOTH poses to the one provided. Assume we don't move.
            ApplyPos();
            shape = _shape;
            SetGrowSpeed(1);
            SetMoveSpeed(1, 0); // Default my move-speed values.

            MakeBody();
            bodyColor = color_sleeping;
            SetRadius(_radius);
            SetCurrentState(GrowerStates.Sleeping);
		}
        private void MakeBody() {
            GameObject prefabGO;
            switch (shape) {
                case GrowerShapes.Circle:
                    prefabGO = ResourcesHandler.Instance.circleGrow_growerBody_circle;
                    break;
                case GrowerShapes.Square:
                    prefabGO = ResourcesHandler.Instance.circleGrow_growerBody_square;
                    break;
                default:
                    Debug.LogError("No GrowerBody associated with this shape! Gotta add it: " + shape);
                    prefabGO = null;
                    break;
            }
            body = Instantiate(prefabGO).GetComponent<GrowerBody>();
            body.Initialize(this);
        }

        public Grower SetGrowSpeed(float _speed) {
            growSpeed = _speed*0.8f; // awkward scaling the speed here.
            return this;
        }
        public Grower SetPosB(float x,float y) {
            posB = new Vector2(x,y);
            return this;
        }
        public Grower SetMoveSpeed(float _speed, float _startLocOffset=0) {
            moveSpeed = _speed*0.02f; // awkward scaling down the speed here.
            doMove = moveSpeed != 0;
            moveOscVal = _startLocOffset;
            ApplyPos();
            return this;
        }


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		private void SetRadius(float _radius) {
			radius = _radius;
			myRectTransform.sizeDelta = new Vector2(radius*2, radius*2);
            body.SetRadius(radius); // tell my body so it can update its collider!
			// Update text!
            t_scoreValue.text = TextUtils.AddCommas(ScoreValue());
        }

        private void ApplyPos() {
            // Do move?? Lerp me between my two poses.
            if (doMove) {
                float moveLoc = MathUtils.Sin01(moveOscVal);
                Pos = Vector2.Lerp(posA,posB, moveLoc);// + posDipOffset + posDanceOffset;
            }
            // DON'T move? Just put me at my one known pos.
            else {
                Pos = posA;// + posDipOffset + posDanceOffset;
            }
        }


        private void SetCurrentState(GrowerStates _state) {
            currentState = _state;
            // Only show my text if I've at least started growing!
            //t_scoreValue.enabled = _state!=GrowerStates.Sleeping && _state!=GrowerStates.PreGrowing;
            t_scoreValue.enabled = _state==GrowerStates.Solidified;
        }

        public void StartGrowing() {
            StartCoroutine(Coroutine_StartGrowing());
        }
		public void Solidify() {
            bodyColor = color_solid;
            SetCurrentState(GrowerStates.Solidified);
		}
        public void OnBodyTriggerEnter() {
            // Illegal overlap!
            SetCurrentState(GrowerStates.Solidified);
            bodyColor = color_illegal;
            myLevel.OnIllegalOverlap();
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


		// ----------------------------------------------------------------
		//  Update
		// ----------------------------------------------------------------
		public void GrowStep() {
			SetRadius(radius + growSpeed*Time.timeScale);
        }

        private void Update() {
            if (Time.timeScale == 0) { return; } // No time? No dice.
            if (myLevel.IsAnimatingIn) { return; } // Animating in? Don't move.

            UpdateMove();
            ApplyPos();
        }
        private void UpdateMove() {
            if (doMove && currentState!=GrowerStates.Solidified) {
                moveOscVal += moveSpeed * timeScale;
            }
        }

	}
}


/*
//      private static int GetMultiplierForRadius(float _radius) {
    //          //if (_radius <  20) { return 1; }TEMP DISABLED
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
    ////            switch (_mult) {TEMP disabled until we make the color scheme.
    ////            case 0: return new ColorHSB(0,0,0).ToColor(); // Hmm.
    ////            default: return Color.black; // Hmm.
    ////            }
    //  //float h = (0.3f + _mult*0.08f) % 1;TEMP DISABLED this, too
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