using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** All Level.cs classes should extend this. So we can have tons of level-based games without having to copy lots of code! */
public abstract class BaseLevel : MonoBehaviour {
	// Properties
	public bool IsAnimatingIn;
	private int levelIndex;
	// Components
	[SerializeField] protected RectTransform myRectTransform=null;

	// Getters
	protected ResourcesHandler resourcesHandler { get { return ResourcesHandler.Instance; } }
	public int LevelIndex { get { return levelIndex; } }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	protected void BaseInitialize(Transform tf_parent, int _levelIndex) {
		levelIndex = _levelIndex;

		gameObject.name = "Level " + levelIndex;
		myRectTransform.SetParent(tf_parent);
		myRectTransform.SetAsFirstSibling(); // put me behind all other UI.
		myRectTransform.anchoredPosition = Vector2.zero;
		myRectTransform.localScale = Vector2.one;
		myRectTransform.localEulerAngles = Vector3.zero;

		AddLevelComponents();
	}


	// ----------------------------------------------------------------
	//  Abstract Methods
	// ----------------------------------------------------------------
	abstract protected void AddLevelComponents();





}
