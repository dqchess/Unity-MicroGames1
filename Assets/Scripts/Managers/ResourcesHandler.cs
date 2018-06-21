using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [SerializeField] public GameObject alphaTapOrder_tapSpace;
    [SerializeField] public GameObject bouncePaint_paintSpace;

	// Instance
	static private ResourcesHandler instance;
	static public ResourcesHandler Instance { get { return instance; } }

	// Awake
	private void Awake () {
		// There can only be one (instance)!
		if (instance == null) {
			instance = this;
		}
		else {
			GameObject.Destroy (this);
		}
	}
}
