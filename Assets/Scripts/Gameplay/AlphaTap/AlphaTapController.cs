using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaTap {
public class AlphaTapController : BaseGameController {
	// Components
//	private List<TapSpace> tapSpaces;

	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	override protected void Start () {
		base.Start();

//		InitializeTapSpaces();
	}
	private void InitializeTapSpaces() {
//		tapSpaces = new List<TapSpace>();

	}

	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	override protected void Update () {
		base.Update();

		RegisterMouseInput();
	}

	private void RegisterMouseInput() {
		if (Input.GetMouseButtonDown(0)) {
			OnMouseDown();
		}
	}


	private void OnMouseDown() {

	}



}
}
