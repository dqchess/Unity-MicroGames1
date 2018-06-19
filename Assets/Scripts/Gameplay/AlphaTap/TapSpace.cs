using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaTap {
public class TapSpace : MonoBehaviour {
	// Components
	[SerializeField] private SpriteRenderer bodySprite;
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
//		oscillValue = 0; // reset this too!
		oscillVel = Random.Range(0.5f, 1.9f);
	}


	// ----------------------------------------------------------------
	//  FixedUpdate
	// ----------------------------------------------------------------
	private void FixedUpdate () {
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
