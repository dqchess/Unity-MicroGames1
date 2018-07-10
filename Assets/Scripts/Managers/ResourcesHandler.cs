using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [SerializeField] public GameObject alphaTapOrder_tapSpace;

    [SerializeField] public GameObject bouncePaint_block;
    [SerializeField] public GameObject bouncePaint_blockNumHitsReqText;
    [SerializeField] public GameObject bouncePaint_level;
    [SerializeField] public GameObject bouncePaint_player;

    [SerializeField] public GameObject letterClear_letterTile;
    [SerializeField] public GameObject letterClear_wordTile;


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
