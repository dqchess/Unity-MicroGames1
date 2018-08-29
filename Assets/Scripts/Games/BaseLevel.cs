using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** All Level.cs classes should extend this. So we can have tons of level-based games without having to copy lots of code! */
public abstract class BaseLevel : MonoBehaviour {
	// Properties
	[HideInInspector] public bool IsAnimating; // set this to true if we're animating in OR out.
	private int levelIndex;
	// Components
	[SerializeField] protected RectTransform myRectTransform=null;
	// References
	private BaseLevelGameController myGameController;

	// Getters (Public)
	public bool IsGameStatePlaying { get { return myGameController.IsGameStatePlaying; } }
	public Canvas Canvas { get { return myGameController.Canvas; } }
	public int LevelIndex { get { return levelIndex; } }
	// Getters (Protected)
	protected ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void BaseInitialize(BaseLevelGameController _myGameController, Transform tf_parent, int _levelIndex) {
		myGameController = _myGameController;
		levelIndex = _levelIndex;

		gameObject.name = "Level " + levelIndex;
		GameUtils.ParentAndReset(this.gameObject, tf_parent);
		myRectTransform.SetAsFirstSibling(); // put me behind all other UI.
		myRectTransform.anchoredPosition = Vector2.zero;
	}


	// ----------------------------------------------------------------
	//  Abstract Methods
	// ----------------------------------------------------------------
	abstract protected void AddLevelComponents();





}
