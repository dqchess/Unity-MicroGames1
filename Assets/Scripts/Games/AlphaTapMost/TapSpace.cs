using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaTapMost {
	public class TapSpace : MonoBehaviour {
		// Components
		[SerializeField] private SpriteRenderer bodySprite=null;
		// Properties
		private bool wasRecentCorrectTap; // sets me to green. Set to false when my alpha is about 0.
		private Color bodyColor;
		private float alpha;
		private float oscillValue; // this is what's pumped in to the sin function!
		private float oscillVel; // this also oscillates for anticipatable randomness

		// Getters
		public bool CanTapMe { get { return !wasRecentCorrectTap; } }
		public float Alpha { get { return alpha; } }



		// ----------------------------------------------------------------
		//  Initialize
		// ----------------------------------------------------------------
		private void Start () {
			oscillValue = Random.Range(0, 99);
			ResetRecentCorrectTap();
		}



		// ----------------------------------------------------------------
		//  Game Events
		// ----------------------------------------------------------------
		public void OnCorrectTap() {
			wasRecentCorrectTap = true;
			bodyColor = Color.green;
		}
		private void ResetRecentCorrectTap() {
			wasRecentCorrectTap = false;
			bodyColor = Color.white;
			oscillValue = Random.Range(0f, 1.1f); // reset this too! BUT don't reset to 0 (user can keep clicking tiles in order), and don't leave it what it was (if we appear a moment before user clicks, they'd lose bc we just appeared all bright).
			oscillVel = Random.Range(0.5f, 1.9f); // randomize our oscillation speed when we come back, so user can't keep up a consistent pattern.
		}


		// ----------------------------------------------------------------
		//  FixedUpdate
		// ----------------------------------------------------------------
		private void Update () {
			UpdateAndApplyAlpha();
			UpdateRecentCorrectTap();
		}
		private void UpdateAndApplyAlpha() {
			if (wasRecentCorrectTap) {
				alpha += (0-alpha) / 8f; // rocket down to 0.
			}
			else {
				oscillValue += Time.deltaTime * oscillVel;
				alpha = 1-MathUtils.Cos01 (oscillValue);
			}

			bodySprite.color = new Color(bodyColor.r,bodyColor.g,bodyColor.b, alpha);
		}
		private void UpdateRecentCorrectTap() {
			if (wasRecentCorrectTap) {
				if (alpha < 0.05f) {
					ResetRecentCorrectTap();
				}
			}
		}

	}
}
