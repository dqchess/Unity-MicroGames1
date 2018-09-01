using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
    // References!
    [SerializeField] public GameObject imageLine;

    [SerializeField] public GameObject alphaTapOrder_tapSpace;

    [SerializeField] public GameObject bouncePaint_levelTile;
    [SerializeField] public GameObject bouncePaint_block;
    [SerializeField] public GameObject bouncePaint_blockNumHitsReqText;
    [SerializeField] public GameObject bouncePaint_level;
	[SerializeField] public GameObject bouncePaint_player;

	[SerializeField] public GameObject circlePop_levelTile;
	[SerializeField] public GameObject circlePop_level;
	[SerializeField] public GameObject circlePop_collisionIcon;
	[SerializeField] public GameObject circlePop_growerCircle;
	[SerializeField] public GameObject circlePop_growerRect;
	[SerializeField] public GameObject circlePop_growerComposite;
    [SerializeField] public GameObject circlePop_wallCircle;
    [SerializeField] public GameObject circlePop_wallRect;

	[SerializeField] public GameObject extrudeMatch_boardSpaceView;
	[SerializeField] public GameObject extrudeMatch_boardView;
	[SerializeField] public GameObject extrudeMatch_boardController;
	[SerializeField] public GameObject extrudeMatch_tileView;

	[SerializeField] public GameObject letterClear_letterTile;
	[SerializeField] public GameObject letterClear_wordTile;

	[SerializeField] public GameObject waveTap_level;
	[SerializeField] public GameObject waveTap_bar;
	[SerializeField] public GameObject waveTap_player;


	// Instance
	static private ResourcesHandler instance;
	static public ResourcesHandler Instance {
        get {
            // Safety check for runtime compile.
            if (instance == null) { instance = FindObjectOfType<ResourcesHandler>(); }
            return instance;
        }
    }


    // Setting Instance (both in Awake AND after scripts recompile!)
	private void Awake () {
        SetInstance();
    }
    //[UnityEditor.Callbacks.DidReloadScripts]
    //private static void OnScriptsReloaded() {
    //    SetInstance();
    //}

    private void SetInstance() {
        // There can only be one (instance)!
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy (this);
        }
    }


}
