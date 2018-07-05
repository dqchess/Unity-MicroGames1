using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BouncePaint {
    public class Block : MonoBehaviour {
        // Components
        [SerializeField] private Image i_body=null;
		[SerializeField] private Image i_hitBox=null;
		[SerializeField] private RectTransform myRectTransform=null;
		[SerializeField] private TextMeshProUGUI t_numHitsReq=null;
        // Properties
		private bool isPainted=false;
		private Rect hitRect;
		private Rect bodyRect;

		private Vector2 posOnTrack;
		private Vector2 posDipOffset;

//		private Vector2 centerTarget;
//		private Vector2 dipOffset; // when we get bounced on, this is set to like (0,-16). Eases back to (0,0). Added to our TODO finish this comment
		// Properties (Specials)
		private bool doTap; // only FALSE for the black Blocks we DON'T wanna tap on!
		private bool doTravel;
		private float travelSpeed; // for TRAVELING Blocks.
		private float travelOscVal; // for TRAVELING Blocks.
		private int numHitsReq;
		private Vector2 centerA,centerB; // for TRAVELING Blocks.
        // References
        private GameController gameController;

		// Getters (Public)
		public bool DoTap { get { return doTap; } }
		public bool IsPainted { get { return isPainted; } }
        public Rect HitRect { get { return hitRect; } }
		public Vector2 GetPredictedPos(float timeFromNow) {
			if (!doTravel) { return HitRect.center; } // Oh, if I'm NOT a traveler, just return my current position.
			float predTravelOscVal = travelOscVal + travelSpeed*timeFromNow; // we use FixedUpdate, so this is actually reliable.
			float predTravelLoc = MathUtils.Sin01(predTravelOscVal);
//			Debug.Log("predTravelLoc: " + predTravelLoc + "  predTravelOscVal: " + predTravelOscVal);
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
				UpdateHitRectCenter();
			}
		}
		private void UpdateHitRectCenter() {
			hitRect.center = center + new Vector2(0, 52);
		}


        // ----------------------------------------------------------------
        //  Initialize
        // ----------------------------------------------------------------
		public void Initialize(GameController _gameController, RectTransform rt_parent,
			Vector2 _size,
			Vector2 _centerA,Vector2 _centerB,
			float _travelSpeed,
			int _numHitsReq,
			bool _doTap
		) {
            gameController = _gameController;
            this.transform.SetParent(rt_parent);
            this.transform.localScale = Vector3.one;
            this.transform.localEulerAngles = Vector3.zero;
			myRectTransform.anchoredPosition = Vector2.zero;
			// Assign properties
			centerA = _centerA;
			centerB = _centerB;
			doTap = _doTap;
			numHitsReq = _numHitsReq;
			travelSpeed = _travelSpeed*0.03f; // awkward scaling down the speed here.
			doTravel = travelSpeed != 0;
			travelOscVal = 0;
			posDipOffset = Vector2.zero;
			ApplyTravelOscVal();
//			center = centerTarget;

			bodyRect = new Rect(center-_size*0.5f, _size);
            hitRect = new Rect(bodyRect);
            // Offset and shrink the hitRect!
            hitRect.size += new Vector2(0, -12);
			UpdateHitRectCenter();
            // Bloat the hitRect's width a bit (in case of high-speed x-movement).
            hitRect.size += new Vector2(20, 0);
			hitRect.center += new Vector2(-10, 0);

			// Now put/size me where I belong!
			myRectTransform.sizeDelta = bodyRect.size;
//			centerTarget = bodyRect.center; // if we get pushed, we'll always ease back to this pos. TODO: Delete these two lines (unless we wanna resurrect them)
//			center = centerTarget;
            i_hitBox.rectTransform.sizeDelta = hitRect.size;
			i_hitBox.rectTransform.anchoredPosition = hitRect.center - myRectTransform.anchoredPosition;

			// Body color
			if (doTap) {
				bodyColor = new Color(0,0,0, 0.4f);
			}
			else {
				bodyColor = new Color(0,0,0, 0.95f); // about black!
			}
			// Hits-required text
			t_numHitsReq.color = new Color(0,0,0, 0.5f);
			if (numHitsReq <= 1) { // Don't need the text? Destroy it.
				Destroy(t_numHitsReq.gameObject);
				t_numHitsReq = null;
			}
			UpdateNumHitsReqText();
        }



        // ----------------------------------------------------------------
        //  Doers
        // ----------------------------------------------------------------
        /// Paints me! :)
		public void OnPlayerBounceOnMe(Color playerColor) {
			// If I'm not yet painted...!
			if (!isPainted) {
				// Hit me, Paul!
				numHitsReq --;
				UpdateNumHitsReqText();
				// That was the last straw, Cady??
				if (numHitsReq <= 0) {
					// Paint me!
	                isPainted = true;
	                bodyColor = playerColor;
				}
            }
            // Push me down!
			posDipOffset += new Vector2(0, -16f);
        }
		private void UpdateNumHitsReqText() {
			if (t_numHitsReq != null) {
				t_numHitsReq.text = numHitsReq.ToString();
				if (numHitsReq <= 0) {
					t_numHitsReq.color = new Color(0,0,0, 0.3f);
				}
			}
		}
		private void ApplyTravelOscVal() {
			float travelLoc = MathUtils.Sin01(travelOscVal);
			center = Vector2.Lerp(centerA,centerB, travelLoc) + posDipOffset;
		}



        // ----------------------------------------------------------------
        //  FixedUpdate
        // ----------------------------------------------------------------
        private void FixedUpdate() {
			UpdateTravel();
            UpdatePosDipOffset();
        }
		private void UpdateTravel() {
			if (doTravel) {
				travelOscVal += travelSpeed;
				ApplyTravelOscVal();
			}
		}
        private void UpdatePosDipOffset() {
			// Update.
			Vector2 posDipTarget = Vector2.zero;
            if (gameController.IsLevelComplete) {
				posDipOffset = new Vector2(0, Mathf.Sin(Time.time*4f+center.x*0.016f) * 9f);
            }
			if (posDipOffset != posDipTarget) {
				posDipOffset += new Vector2(
					(posDipTarget.x-posDipOffset.x) * 0.3f,
					(posDipTarget.y-posDipOffset.y) * 0.3f);
			}
        }

    }
}