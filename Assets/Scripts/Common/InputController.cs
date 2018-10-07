using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	// Instance
	static private InputController instance;
	// Properties
	private Vector2 player0AxisInput;
	private Vector2 player0AxisInputRaw; // this ISN'T rotated to match the camera. It's raw, baby. Raw.

	// Getters
	static public InputController Instance {
		get {
//			if (instance==null) { return this; } // Note: This is only here to prevent errors when recompiling code during runtime.
			return instance;
		}
	}
	public bool IsTouchHold() { return Input.GetMouseButton(0); }
	public bool IsTouchDown() { return Input.GetMouseButtonDown(0); }
	public bool IsTouchUp() { return Input.GetMouseButtonUp(0); }




	// ----------------------------------------------------------------
	//  Awake
	// ----------------------------------------------------------------
	private void Awake () {
		// There can only be one (instance)!!
		if (instance != null) {
			Destroy (this.gameObject);
			return;
		}
		instance = this;
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		RegisterButtonInputs ();
	}

	private void RegisterButtonInputs () {
		player0AxisInputRaw = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player0AxisInput = player0AxisInputRaw;

		// Scale playerAxisInput so it's normalized, and easier to control. :)
		if (player0AxisInput != Vector2.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = player0AxisInput.magnitude;
			player0AxisInput = player0AxisInput / directionLength;

			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min (1, directionLength);

			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;

			// Multiply the normalized direction vector by the modified length
			player0AxisInput = player0AxisInput * directionLength;
		}

		// Rotate input to match any camera rotation
		float camAngle = Camera.main.transform.localEulerAngles.z * Mathf.Deg2Rad;
		player0AxisInput = new Vector2 (Mathf.Cos (camAngle)*player0AxisInput.x+Mathf.Sin (camAngle)*player0AxisInput.y, Mathf.Cos (camAngle)*player0AxisInput.y+Mathf.Sin (camAngle)*player0AxisInput.x);
	}


}


