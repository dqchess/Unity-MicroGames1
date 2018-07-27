using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BouncePaint {
    public class Block : MonoBehaviour {
        // Components
        [SerializeField] private BoxCollider2D myCollider=null;
        [SerializeField] private Image i_body=null;
        [SerializeField] private Image i_hitBox=null;
        [SerializeField] private ParticleSystem ps_hit=null;
        [SerializeField] private RectTransform myRectTransform=null;
		private TextMeshProUGUI t_numHitsReq=null; // This is added from a prefab if needed!
        // Properties
        private bool isPainted=false;
        private bool isPaintable=true; // set to FALSE for Blocks that can be bounced on but NOT painted (and they're always satisfied, of course).
        private Player ballTargetingMe=null;
		private Rect hitBox;
        private Vector2 posDipOffset; // when we get bounced on, this is set to like (0,-16). Eases back to (0,0). Added to our center pos.
        private Vector2 posDipOffsetVel;
        private Vector2 posDanceOffset=Vector2.zero; // for post-level celebration.
        private Vector2 size;
		// Properties (Specials)
		private bool doTap=true; // only FALSE for the black Blocks we DON'T wanna tap on!
		private bool doTravel;
		private float travelSpeed=1f; // for TRAVELING Blocks.
		private float travelOscVal; // for TRAVELING Blocks.
		private int numHitsReq;
		private Vector2 centerA,centerB; // for TRAVELING Blocks.
        // References
        [SerializeField] private Sprite s_bodyDontTap=null;
        private GameController gameController;
        private Level myLevel;
        // Editor-Settable Properties
        [Header("Hitbox")]
        public float hitboxYOffset;
        public float hitboxHeight=38f;
        /// For when we make hitbox changes in the inspector! Updates us in real-time. :)
        void OnValidate() {
            UpdateHitBox();
        }


		// Getters (Public)
        public bool DoTap { get { return doTap; } }
        public bool IsPainted { get { return isPainted; } }
        public bool IsSatisfied { get { return isPainted || !isPaintable; } }
        public bool IsAvailable { get { return !isPainted && ballTargetingMe==null; } } // I'm available if A) I'm unpainted, and B) Nobody's planning to hit me!
        public float BlockTop { get { return center.y + size.y*0.5f; } } // My VISUAL top.
        public float BlockCenterY { get { return center.y; } } // My VISUAL center.
        public float BlockHeight { get { return center.y; } } // My VISUAL height.
        public int NumHitsReq { get { return numHitsReq; } }
        public Rect HitBox { get { return hitBox; } }
        public Player BallTargetingMe {
            get { return ballTargetingMe; }
            set { ballTargetingMe = value; }
        }
		public Vector2 GetPredictedPos(float timeFromNow) {
			if (!doTravel) { return HitBox.center; } // Oh, if I'm NOT a traveler, just return my current position.
			float predTravelOscVal = travelOscVal + travelSpeed*timeFromNow; // we use FixedUpdate, so this is actually reliable.
			float predTravelLoc = MathUtils.Sin01(predTravelOscVal);
			return Vector2.Lerp(centerA,centerB, predTravelLoc);
		}

        // Getters (Private)
        private Color bodyColor {
            get { return i_body.color; }
            set { i_body.color = value; }
		}
		private Vector2 center {
			get { return myRectTransform.anchoredPosition; }
			set {
				myRectTransform.anchoredPosition = value;
				UpdateHitBox();
			}
        }
        //private float fts { get { return TimeController.FrameTimeScale; } }
        private float timeScale { get { return Time.timeScale; } }
        private void UpdateHitBox() {
            // Update size and center!
            hitBox.size = new Vector2(size.x+40, hitboxHeight); // Fudgily bloat the hitBox's width a bit (in case the block or ball are moving fast horizontally).
			hitBox.center = new Vector2(
                center.x,
                center.y + (size.y+hitBox.height)*0.5f + hitboxYOffset);
            // Apply to my debug image.
            i_hitBox.rectTransform.sizeDelta = hitBox.size;
            i_hitBox.rectTransform.anchoredPosition = hitBox.center - myRectTransform.anchoredPosition; // note: awkward my-whole-position offset.
            // Apply to my collider.
            myCollider.size = hitBox.size;
            myCollider.offset = hitBox.center - myRectTransform.anchoredPosition; // note: awkward my-whole-position offset.
		}


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
		public void Initialize(GameController _gameController, Level _myLevel,
			Vector2 _size,
			Vector2 _centerA,Vector2 _centerB
		) {
            gameController = _gameController;
            myLevel = _myLevel;
            this.transform.SetParent(myLevel.transform);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;
			myRectTransform.anchoredPosition = Vector2.zero;
			// Assign properties
            size = _size;
            centerA = _centerA;
            centerB = _centerB;
            posDipOffset = posDipOffsetVel = Vector2.zero;
            ApplyPos();

            // Make hitBox!
            hitBox = new Rect();
            UpdateHitBox();

			// Now put/size me where I belong!
            myRectTransform.sizeDelta = size;
            ps_hit.transform.localPosition = new Vector3(0, size.y*0.5f, 0);
            GameUtils.SetParticleSystemShapeRadius(ps_hit, size.x/20f); // Make sure to size the shape to match my width. HACK with /20f. Idk why.

            // Default my speed values for safety.
            SetSpeed(1, 0);
        }
        public Block SetHitsReq(int _numHitsReq) {
            numHitsReq = _numHitsReq;
            // Hits-required text
            if (numHitsReq > 1) {
                t_numHitsReq = Instantiate(ResourcesHandler.Instance.bouncePaint_blockNumHitsReqText).GetComponent<TextMeshProUGUI>();
                t_numHitsReq.rectTransform.SetParent(this.myRectTransform);
                t_numHitsReq.rectTransform.localScale = Vector2.one;
                t_numHitsReq.rectTransform.localEulerAngles = Vector2.zero;
                t_numHitsReq.rectTransform.offsetMin = t_numHitsReq.rectTransform.offsetMax = Vector2.zero;
                t_numHitsReq.color = new Color(0,0,0, 0.5f);
            }
            UpdateNumHitsReqText();
            return this;
        }
        public Block SetDontTap() {
            doTap = false;
             if (!doTap) {
              i_body.sprite = s_bodyDontTap;
             }
            SetIntentionVisuals(false);
            return this;
        }
        public Block SetUnpaintable() {
            isPaintable = false;
            return this;
        }
        public Block SetSpeed(float _speed, float _startLocOffset=0) {
            travelSpeed = _speed*0.03f; // awkward scaling down the speed here.
            doTravel = travelSpeed != 0;
            travelOscVal = _startLocOffset;
            ApplyPos();
            return this;
        }


        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
		public void SetIntentionVisuals(bool isPlayerComingToMe) {
			if (isPainted) { return; } // Already painted? We don't have to do anything.

			// if (isPlayerComingToMe) {
			// 	if (doTap) {
			// 		bodyColor = new Color(0.7f,0.7f,0.7f, 0.6f);
			// 	}
			// 	// else {
			// 	// 	bodyColor = new Color(0,0,0, 0.8f);
			// 	// }
			// }
			// else {
				if (doTap) {
					//bodyColor = new Color(0,0,0, 0.4f); // light gray
                bodyColor = new Color(1, 1, 1, 0.6f);
				}
				else {
                //bodyColor = new Color(0,0,0, 0.99f); // Black.
                //bodyColor = Color.white;
                bodyColor = new Color(1,1,1, 0.2f);
				}
			// }
		}
		private void UpdateNumHitsReqText() {
			if (t_numHitsReq != null) {
                t_numHitsReq.text = numHitsReq.ToString();
                t_numHitsReq.enabled = numHitsReq > 0;
				//if (numHitsReq <= 0) {
				//	t_numHitsReq.color = new Color(0,0,0, 0.3f);
				//}
			}
		}
		private void ApplyPos() {
			// Do travel?? Lerp me between my two center poses.
			if (doTravel) {
				float travelLoc = MathUtils.Sin01(travelOscVal);
                center = Vector2.Lerp(centerA,centerB, travelLoc) + posDipOffset + posDanceOffset;
			}
			// DON'T travel? Just put me at my one known center pos.
			else {
                center = centerA + posDipOffset + posDanceOffset;
			}
		}


        // ----------------------------------------------------------------
        //  Events
        // ----------------------------------------------------------------
        /// Paints me! :)
        public void OnPlayerBounceOnMe(Color playerColor, Vector2 playerVel) {
            // If I'm paintable AND not yet painted...!
            if (isPaintable && !isPainted) {
                // Hit me, Paul!
                numHitsReq --;
                UpdateNumHitsReqText();
                // That was the last straw, Cady?? Paint me!
                if (numHitsReq <= 0) {
                    PaintMe(playerColor);
                }
                // Particle burst!
                GameUtils.SetParticleSystemColor(ps_hit, bodyColor);
                ps_hit.Emit(12);
            }
            // Push me down AND horizontally (based on Player's x vel)!
            //posDipOffset += new Vector2(playerVel.x*1.2f, -24f);//-16
            posDipOffsetVel += new Vector2(playerVel.x*0.6f, -10f);
            ApplyPos(); // apply pos immediately, so Player knows where we actually are.
        }
        /// Player tells us we're the winning bounce, and to dip down extra.
        public void DipExtraFromWinningBounce(Vector2 playerVel) {
            posDipOffsetVel += new Vector2(playerVel.x*1.2f, -20f);
            ApplyPos();
        }
        private void PaintMe(Color playerColor) {
            isPainted = true;
            // If I'm a DO-tap, then color me and do da burst!
            if (DoTap) {
                bodyColor = playerColor;
            }
        }
        public void OnPlayerBounceUpOffscreenFromMe() {
            // Push me down extra!
            //posDipOffset += new Vector2(0, -42f);
            posDipOffsetVel += new Vector2(0, -30f);
        }
		/// Called when Player taps to jump while in me, BUT I'm a don't-tap Block!
		public void OnPlayerPressJumpOnMeInappropriately() {
			bodyColor = new Color(0.6f,0f,0.06f, 1f);
		}



        // ----------------------------------------------------------------
        //  Update
        // ----------------------------------------------------------------
        private void Update() {
            if (Time.timeScale == 0) { return; } // No time? No dice.
            if (myLevel.IsAnimatingIn) { return; } // Animating in? Don't move.

			UpdateTravel();
            UpdatePosOffsets();
			ApplyPos();
        }
		private void UpdateTravel() {
			if (doTravel) {
				travelOscVal += travelSpeed * timeScale;
			}
		}
        private void UpdatePosOffsets() {
            if (posDipOffsetVel != Vector2.zero) { // Mild optimization.
                // Apply vel
                posDipOffset += posDipOffsetVel;
                // Update vel
                posDipOffsetVel *= 0.8f;
            }
            // Dip
			Vector2 posDipTarget = Vector2.zero;
			if (posDipOffset != posDipTarget) {
				posDipOffset += new Vector2(
					(posDipTarget.x-posDipOffset.x) * 0.24f,
                    (posDipTarget.y-posDipOffset.y) * 0.24f);
			}
            // Dance
            if (gameController.IsLevelComplete) {
                posDanceOffset = new Vector2(0, Mathf.Sin(Time.time*4f+center.x*0.016f) * 26f);
            }
        }

    }
}