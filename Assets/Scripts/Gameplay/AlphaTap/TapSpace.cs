using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaTap {
public class TapSpace : MonoBehaviour {
	// Components
	[SerializeField] private SpriteRenderer bodySprite;
	// Properties
	private float alpha;
	private float oscillValue; // this is what's pumped in to the sin function!
	private float oscillVel; // this also oscillates for anticipatable randomness



	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	private void Start () {
		oscillValue = Random.Range(0, 99);
		oscillVel = Random.Range(0.7f, 1.5f);
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		UpdateAndApplyAlpha();

	}
	private void UpdateAndApplyAlpha() {
		oscillValue += Time.deltaTime * oscillVel;

		alpha = 0.5f + Mathf.Sin (oscillValue)*0.5f;
		GameUtils.SetSpriteAlpha (bodySprite, alpha);
	}

}
}
