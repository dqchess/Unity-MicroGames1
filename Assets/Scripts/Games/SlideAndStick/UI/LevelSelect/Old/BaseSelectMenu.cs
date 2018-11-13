/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlideAndStick {
    abstract public class BaseSelectMenu : MonoBehaviour {
    	// References
    	protected LevSelController levSelController;
    
    	// Getters
    	protected LevelsManager lm { get { return LevelsManager.Instance; } }
    	protected LevelAddress selectedAddress { get { return lm.selectedAddress; } }
    
    
    	// ----------------------------------------------------------------
    	//  Doers
    	// ----------------------------------------------------------------
    	public void SetLevSelControllerRef(LevSelController _levSelController) {
    		this.levSelController = _levSelController;
    	}
    	virtual public void PositionTiles() { }
    
    	public void OnBackButtonClick() {
    		//levSelController.PopMenu();
    	}
    
    	virtual public void Show () { // Override me!
    		this.gameObject.SetActive(true);
    	}
    	virtual public void Hide() { // Override me!
    		this.gameObject.SetActive(false);
    	}
    
    
    }
}
*/